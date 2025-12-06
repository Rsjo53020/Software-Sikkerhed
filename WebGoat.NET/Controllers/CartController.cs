using WebGoatCore.Models;
using WebGoatCore.Data;
using Microsoft.AspNetCore.Mvc;

namespace WebGoatCore.Controllers
{
    [Route("[controller]/[action]")]
    public class CartController : Controller
    {
        private readonly ProductRepository _productRepository;

        public CartController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        private Cart GetCart()
        {
            if (!HttpContext.Session.TryGet<Cart>("Cart", out var cart) || cart == null)
            {
                cart = new Cart();
                // Sørg for at en ny kurv altid bliver gemt
                HttpContext.Session.Set("Cart", cart);
            }

            return cart;
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.Set("Cart", cart);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost("{productId:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrder(int productId, short quantity)
        {
            if (quantity <= 0)
            {
                TempData["Error"] = "Quantity must be greater than zero.";
                return RedirectToAction("Details", "Product", new { productId });
            }

            var product = _productRepository.GetProductById(productId);
            
            if (product == null)
            {
                TempData["Error"] = $"Product {productId} was not found.";
                return RedirectToAction(nameof(Index));
            }

            var cart = GetCart();

            if (!cart.OrderDetails.TryGetValue(productId, out var orderDetail))
            {
                orderDetail = new OrderDetail
                {
                    Discount = 0.0F,
                    ProductId = productId,
                    Quantity = quantity,
                    Product = product,
                    UnitPrice = product.UnitPrice
                };

                cart.OrderDetails.Add(productId, orderDetail);
            }
            else
            {
                // Beskyt mod overflow – og udtryk tydeligt at vi lægger til
                checked
                {
                    orderDetail.Quantity += quantity;
                }
            }

            SaveCart(cart);

            TempData["Message"] = "Product added to cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("{productId:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveOrder(int productId)
        {
            var cart = GetCart();

            if (!cart.OrderDetails.ContainsKey(productId))
            {
                TempData["Error"] = $"Product {productId} was not found in your cart.";
                return RedirectToAction(nameof(Index));
            }

            cart.OrderDetails.Remove(productId);
            SaveCart(cart);

            TempData["Message"] = "Product removed from cart.";
            return RedirectToAction(nameof(Index));
        }
    }
}