using WebGoatCore.Models;
using System;
using System.Linq;

namespace WebGoatCore.Data
{
    public class CustomerRepository
    {
        private readonly NorthwindContext _context;

        public CustomerRepository(NorthwindContext context)
        {
            _context = context;
        }
        public string CreateCustomer(string companyName, string contactName, string? address = null, string? city = null, string? region = null, string? postalCode = null, string? country = null)
        {
            try
            {
                var customerId = GenerateCustomerId(companyName);
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

                _context.Customers.Add(customer);
                _context.SaveChanges();
                return customerId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not create customer", ex);
            }
        }

        public Customer? GetCustomerByUsername(string username)
        {
            var customerEntity = _context.Customers.FirstOrDefault(c => c.ContactName == username);

            return customerEntity;
        }

        public Customer GetCustomerByCustomerId(string customerId)
        {
            return _context.Customers.Single(c => c.CustomerId == customerId);
        }

        public bool CustomerIdExists(string customerId)
        {
            var con = _context.Customers.Any(c => c.CustomerId == customerId);
            return con;
        }

        public void SaveCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
        }

        /// <summary>Returns an unused CustomerId based on the company name</summary>
        /// <param name="companyName">What we want to base the CompanyId on.</param>
        /// <returns>An unused CustomerId.</returns>
        private string GenerateCustomerId(string companyName)
        {
            var characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();

            // Fjern mellemrum og begræns længden
            var id = companyName.Replace(" ", "");
            id = id.Length >= 5 ? id.Substring(0, 5) : id.PadRight(5, 'X');  // fyld ud så den altid er 5 tegn

            // Maks forsøg før vi giver op
            for (int i = 0; i < 50; i++)
            {
                if (!CustomerIdExists(id))
                    return id;

                id = id.Substring(0, 4) + characters[random.Next(characters.Length)];
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