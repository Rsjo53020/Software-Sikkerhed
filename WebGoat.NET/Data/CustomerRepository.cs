using WebGoatCore.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WebGoatCore.Data
{
    public class CustomerRepository
    {
        private readonly NorthwindContext _context;

        public CustomerRepository(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<string> CreateCustomerAsync(string companyName, string contactName, string? address = null, string? city = null, string? region = null, string? postalCode = null, string? country = null)
        {
            try
            {
                var customerId = await GenerateCustomerIdAsync(companyName);
                var customer = new Customer
                {
                    CustomerId = customerId,
                    CompanyName = companyName,
                    ContactName = contactName,
                    Address = address,
                    City = city,
                    Region = region,
                    PostalCode = postalCode,
                    Country = country
                };

                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();

                return customerId;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Could not create customer due to a database error.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not create customer.", ex);
            }
        }

        public async Task<Customer?> GetCustomerByUsernameAsync(string username)
        {
            return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ContactName == username);

        }

        public async Task<Customer?> GetCustomerByCustomerIdAsync(string customerId)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<bool> CustomerIdExistsAsync(string customerId)
        {
            return await _context.Customers
                .AsNoTracking()
                .AnyAsync(c => c.CustomerId == customerId);
        }

        public async Task SaveCustomerAsync(CustomerDM customerDM)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerDM.CustomerId.ToString());

            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            customer.CompanyName = customerDM.CompanyName.ToString();
            customer.ContactName = customerDM.ContactName.ToString();
            customer.ContactTitle = customerDM.ContactTitle?.ToString();
            customer.Address = customerDM.Address?.ToString();
            customer.City = customerDM.City?.ToString();
            customer.Region = customerDM.Region?.ToString();
            customer.PostalCode = customerDM.PostalCode?.ToString();
            customer.Country = customerDM.Country?.ToString();
            customer.Phone = customerDM.Phone?.ToString();
            customer.Fax = customerDM.Fax?.ToString();

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
        /// <summary>Returns an unused CustomerId based on the company name</summary>
        /// <param name="companyName">What we want to base the CompanyId on.</param>
        /// <returns>An unused CustomerId.</returns>
        private async Task<string> GenerateCustomerIdAsync(string companyName)
        {
            var characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var baseId = companyName
                .Replace(" ", string.Empty)
                .ToUpperInvariant();

            baseId = baseId.Length >= 5
                ? baseId.Substring(0, 5)
                : baseId.PadRight(5, 'X');

            var id = baseId;

            // Maks forsøg før vi giver op
            for (int i = 0; i < 50; i++)
            {
                if (! await CustomerIdExistsAsync(baseId))
                    return baseId;

                id = id.Substring(0, 4) + characters[Random.Shared.Next(characters.Length)];
            }

            throw new InvalidOperationException("Could not generate unique CustomerID.");
        }

        private T? CreateIfNotNull<T>(string? value) where T : class
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return Activator.CreateInstance(typeof(T), value) as T;
        }
    }
}