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

        public Customer? GetCustomerByUsername(string username)
        {
            return _context.Customers.FirstOrDefault(c => c.ContactName.Value == username);
        }

        public Customer GetCustomerByCustomerId(string customerId)
        {
            return _context.Customers.Single(c => c.CustomerId.Value == customerId);
        }

        public void SaveCustomer(Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();
        }

        //TODO: Add try/catch logic
        public string CreateCustomer(string companyName, string contactName, string? address, string? city, string? region, string? postalCode, string? country)
        {
            try
            {
                var customerId = GenerateCustomerId(companyName);
                
                // Strongly typed domain primitives
                var newCompanyName = new CompanyName(companyName);
                var newContactName = new ContactName(contactName);

                Address? newAddress = CreateIfNotNull<Address>(address);
                City? newCity = CreateIfNotNull<City>(city);
                Region? newRegion = CreateIfNotNull<Region>(region);
                PostalCode? newPostalCode = CreateIfNotNull<PostalCode>(postalCode);
                Country? newCountry = CreateIfNotNull<Country>(country);
                
                var customer = new Customer(
                    new CustomerId(customerId),
                    newCompanyName,
                    newContactName,
                    null,
                    newAddress,
                    newCity,
                    newRegion,
                    newPostalCode,
                    newCountry,
                    null,
                    null
                    );
            
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return customerId;
        }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not create customer", ex);
            }
        }

        public bool CustomerIdExists(string customerId)
        {
            return _context.Customers.Any(c => c.CustomerId.Value == customerId);
        }

        /// <summary>Returns an unused CustomerId based on the company name</summary>
        /// <param name="companyName">What we want to base the CompanyId on.</param>
        /// <returns>An unused CustomerId.</returns>
        private string GenerateCustomerId(string companyName)
        {
            var random = new Random();
            var customerId = companyName.Replace(" ", "");
            customerId = (customerId.Length >= 5) ? customerId.Substring(0, 5) : customerId;
            while (CustomerIdExists(customerId))
            {
                customerId = customerId.Substring(0, 4) + "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"[random.Next(35)];
            }
            return customerId;
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
