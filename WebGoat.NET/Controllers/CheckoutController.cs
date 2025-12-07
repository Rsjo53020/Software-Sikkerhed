using WebGoatCore.Models;
using WebGoatCore.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using WebGoatCore.ViewModels;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using WebGoatCore.Exceptions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebGoatCore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomerRepository _customerRepository;
        private readonly ShipperRepository _shipperRepository;
        private readonly OrderRepository _orderRepository;
        private CheckoutViewModel? _model;
        private string _resourcePath;

        public CheckoutController(UserManager<IdentityUser> userManager, CustomerRepository customerRepository, IHostEnvironment hostEnvironment, IConfiguration configuration, ShipperRepository shipperRepository, OrderRepository orderRepository)
        {
            _userManager = userManager;
            _customerRepository = customerRepository;
            _shipperRepository = shipperRepository;
            _orderRepository = orderRepository;
            _resourcePath = configuration.GetValue(Constants.WEBGOAT_ROOT, hostEnvironment.ContentRootPath);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (_model == null)
            {
                await InitializeModelAsync();
            }

            return View(_model);
        }

        private async Task InitializeModelAsync()
        {
            _model = new CheckoutViewModel();

            _model.Cart = HttpContext.Session.Get<Cart>("Cart");
            _model.AvailableExpirationYears = Enumerable
            .Range(1, 5)
            .Select(i => DateTime.Now.Year + i)
            .ToList();

            _model.ShippingOptions = await _shipperRepository.GetShippingOptionsAsync(Convert.ToDecimal(_model.Cart?.SubTotal ?? 0));

            if (_model.Cart == null || _model.Cart.Items.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "You have no items in your cart.");
            }

            var customerDM = await GetCustomerOrAddErrorAsync();
            if (customerDM != null)
            {
                //map from CustomerDM to CustomerEntity
                var customerEntity = new Customer()
                {
                    CustomerId = customerDM.CustomerId.Value,
                    CompanyName = customerDM.CompanyName.Value,
                    ContactName = customerDM.ContactName.Value,
                    ContactTitle = customerDM.ContactTitle?.Value,
                    Address = customerDM.Address?.Value,
                    City = customerDM.City?.Value,
                    Region = customerDM.Region?.Value,
                    PostalCode = customerDM.PostalCode?.Value,
                    Country = customerDM.Country?.Value,
                    Phone = customerDM.Phone?.Value,
                    Fax = customerDM.Fax?.Value
                };
                var creditCard = GetCreditCardForUser();

                creditCard.GetCardForUser();

                _model.CreditCard = creditCard.Number;
                _model.ExpirationMonth = creditCard.Expiry.Month;
                _model.ExpirationYear = creditCard.Expiry.Year;
                _model.ShipTarget = customerEntity.CompanyName;
                _model.Address = customerEntity.Address ?? string.Empty;
                _model.City = customerEntity.City ?? string.Empty;
                _model.Region = customerEntity.Region ?? string.Empty;
                _model.PostalCode = customerEntity.PostalCode ?? string.Empty;
                _model.Country = customerEntity.Country ?? string.Empty;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            model.Cart = HttpContext.Session.Get<Cart>("Cart")!;

            var customerDM = await GetCustomerOrAddErrorAsync();
            if (customerDM == null)
            {
                return View(model);
            }

            var creditCard = GetCreditCardForUser();
            try
            {
                creditCard.GetCardForUser();
            }
            catch (WebGoatCreditCardNotFoundException)
            {
            }

            //Get form of payment
            //If form specified card number, try to use it instead one stored for user
            if (model.CreditCard != null && model.CreditCard.Length >= 13)
            {
                creditCard.Number = model.CreditCard;
                creditCard.Expiry = new DateTime(model.ExpirationYear, model.ExpirationMonth, 1);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The card number specified is too short.");
                _model = model;
                return View(_model);
            }

            //Authorize payment through our bank or Authorize.net or someone.
            if (!creditCard.IsValid())
            {
                ModelState.AddModelError(string.Empty, "That card is not valid. Please enter a valid card.");
                _model = model;
                return View(_model);
            }

            if (model.RememberCreditCard)
            {
                creditCard.SaveCardForUser();
            }

            //map from CustomerDM to CustomerEntity
            var customerEntity = new Customer()
            {
                CustomerId = customerDM.CustomerId.Value,
                CompanyName = customerDM.CompanyName.Value,
                ContactName = customerDM.ContactName.Value,
                ContactTitle = customerDM.ContactTitle?.Value,
                Address = customerDM.Address?.Value,
                City = customerDM.City?.Value,
                Region = customerDM.Region?.Value,
                PostalCode = customerDM.PostalCode?.Value,
                Country = customerDM.Country?.Value,
                Phone = customerDM.Phone?.Value,
                Fax = customerDM.Fax?.Value
            };

            var order = new Order
            {
                ShipVia = model.ShippingMethod,
                ShipName = model.ShipTarget,
                ShipAddress = model.Address,
                ShipCity = model.City,
                ShipRegion = model.Region,
                ShipPostalCode = model.PostalCode,
                ShipCountry = model.Country,
                OrderDetails = MapCartToOrderDetails(model.Cart),
                CustomerId = customerEntity.CustomerId,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now.AddDays(7),
                Freight = Math.Round((await _shipperRepository.GetShipperByShipperIdAsync(model.ShippingMethod)).GetShippingCost(Convert.ToDecimal(model.Cart.SubTotal)), 2),
                EmployeeId = 1,
            };

            var approvalCode = creditCard.ChargeCard(order.Total);

            order.Shipment = new Shipment()
            {
                ShipmentDate = DateTime.Today.AddDays(1),
                ShipperId = order.ShipVia,
                TrackingNumber = _shipperRepository.GetNextTrackingNumber(await _shipperRepository.GetShipperByShipperIdAsync(order.ShipVia)),
            };

            //Create the order itself.
            var orderId = await _orderRepository.CreateOrderAsync(order);

            //Create the payment record.
            await _orderRepository.CreateOrderPaymentAsync(orderId, order.Total, creditCard.Number, creditCard.Expiry, approvalCode);

            HttpContext.Session.SetInt32("OrderId", orderId);
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Receipt");
        }

        public async Task<IActionResult> Receipt(int? id)
        {
            var orderId = HttpContext.Session.GetInt32("OrderId");
            if (id != null)
            {
                orderId = id;
            }

            if (orderId == null)
            {
                ModelState.AddModelError(string.Empty, "No order specified. Please try again.");
                return View();
            }

            Order order;
            try
            {
                order = await _orderRepository.GetOrderByIdAsync(orderId.Value);
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError(string.Empty, string.Format("Order {0} was not found.", orderId));
                return View();
            }

            return View(order);
        }

        public async Task<IActionResult> Receipts()
        {
            var customerDM = await GetCustomerOrAddErrorAsync();
            if (customerDM == null)
            {
                return View();
            }
            //map from CustomerDM to Customer
            var customerEntity = new Customer()
            {
                CustomerId = customerDM.CustomerId.Value,
                CompanyName = customerDM.CompanyName.Value,
                ContactName = customerDM.ContactName.Value,
                ContactTitle = customerDM.ContactTitle?.Value,
                Address = customerDM.Address?.Value,
                City = customerDM.City?.Value,
                Region = customerDM.Region?.Value,
                PostalCode = customerDM.PostalCode?.Value,
                Country = customerDM.Country?.Value,
                Phone = customerDM.Phone?.Value,
                Fax = customerDM.Fax?.Value
            };

            return View(await _orderRepository.GetAllOrdersByCustomerIdAsync(customerEntity.CustomerId));
        }

        public async Task<IActionResult> PackageTracking(string? carrier, string? trackingNumber)
        {
            var model = new PackageTrackingViewModel()
            {
                SelectedCarrier = carrier,
                SelectedTrackingNumber = trackingNumber,
            };

            var customerDM = await GetCustomerOrAddErrorAsync();
            if (customerDM != null)
            {
                //map from CustomerDM to Customer
                var customerEntity = new Customer()
                {
                    CustomerId = customerDM.CustomerId.Value,
                    CompanyName = customerDM.CompanyName.Value,
                    ContactName = customerDM.ContactName.Value,
                    ContactTitle = customerDM.ContactTitle?.Value,
                    Address = customerDM.Address?.Value,
                    City = customerDM.City?.Value,
                    Region = customerDM.Region?.Value,
                    PostalCode = customerDM.PostalCode?.Value,
                    Country = customerDM.Country?.Value,
                    Phone = customerDM.Phone?.Value,
                    Fax = customerDM.Fax?.Value
                };
                model.Orders = await _orderRepository.GetAllOrdersByCustomerIdAsync(customerEntity.CustomerId);
            }

            return View(model);
        }

        public IActionResult GoToExternalTracker(string carrier, string trackingNumber)
        {
            return Redirect(Order.GetPackageTrackingUrl(carrier, trackingNumber));
        }

        private async Task<CustomerDM?> GetCustomerOrAddErrorAsync()
        {

            var user = await _userManager.GetUserAsync(User);
            var username = user?.UserName;

            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError(string.Empty, "I can't identify you. Please log in and try again.");
                return null;
            }

            var customer = await _customerRepository.GetCustomerByUsernameAsync(username);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "I can't identify you. Please log in and try again.");
                return null;
            }
            //map from CustomerEntity to CustomerDM
            var customerDM = new CustomerDM(new CustomerId(customer.CustomerId),
                                            new CompanyName(customer.CompanyName),
                                            new ContactName(customer.ContactName),
                                            customer.ContactTitle != null ? new ContactTitle(customer.ContactTitle) : null,
                                            customer.Address != null ? new Address(customer.Address) : null,
                                            customer.City != null ? new City(customer.City) : null,
                                            customer.Region != null ? new Region(customer.Region) : null,
                                            customer.PostalCode != null ? new PostalCode(customer.PostalCode) : null,
                                            customer.Country != null ? new Country(customer.Country) : null,
                                            customer.Phone != null ? new PhoneNumber(customer.Phone) : null,
                                            customer.Fax != null ? new FaxNumber(customer.Fax) : null);

            return customerDM;
        }

        private CreditCard GetCreditCardForUser()
        {
            return new CreditCard()
            {
                Filename = Path.Combine(_resourcePath, "StoredCreditCards.xml"),
                Username = _userManager.GetUserName(User) ?? string.Empty
            };
        }

        private List<OrderDetail> MapCartToOrderDetails(Cart cart)
        {
            var list = new List<OrderDetail>();

            foreach (var item in cart.Items.Values)
            {
                list.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = 0f
                });
            }

            return list;
        }
    }
}