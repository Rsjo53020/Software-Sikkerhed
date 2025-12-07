using WebGoatCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace WebGoatCore.Data
{
    public class OrderRepository
    {
        private readonly NorthwindContext _context;
        private readonly CustomerRepository _customerRepository;

        public OrderRepository(NorthwindContext context, CustomerRepository customerRepository)
        {
            _context = context;
            _customerRepository = customerRepository;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            // 🔹 VIGTIGT: Sørg for at eksisterende Product/Category ikke bliver inserted
            foreach (var detail in order.OrderDetails)
            {
                if (detail.Product != null)
                {
                    // Sørg for at FK er sat
                    detail.ProductId = detail.Product.ProductId;

                    // Fortæl EF at produktet allerede findes
                    _context.Entry(detail.Product).State = EntityState.Unchanged;

                    if (detail.Product.Category != null)
                    {
                        _context.Entry(detail.Product.Category).State = EntityState.Unchanged;
                    }
                }
            }

            if (order.Shipment != null)
            {
                // Hvis Shipment har Shipper-navigation, så sørg for at den også er Unchanged
                if (order.Shipment.Shipper != null)
                {
                    _context.Entry(order.Shipment.Shipper).State = EntityState.Unchanged;
                }
            }
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.OrderId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task CreateOrderPaymentAsync(int orderId, decimal amountPaid, string creditCardNumber, DateTime expirationDate, string approvalCode)
        {
            var orderPayment = new OrderPayment()
            {
                AmountPaid = Convert.ToDouble(amountPaid),
                CreditCardNumber = creditCardNumber,
                ApprovalCode = approvalCode,
                ExpirationDate = expirationDate,
                OrderId = orderId,
                PaymentDate = DateTime.Now
            };

            await _context.OrderPayments.AddAsync(orderPayment);
            await _context.SaveChangesAsync();

        }

        public async Task<IReadOnlyList<Order>> GetAllOrdersByCustomerIdAsync(string customerId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.OrderId)
                .ToListAsync();
        }
    }
}