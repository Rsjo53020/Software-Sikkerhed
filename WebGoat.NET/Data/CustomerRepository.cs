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

        public CustomerDM? GetCustomerByUsername(string username)
        {
            var customerEntity = _context.Customers.FirstOrDefault(c => c.ContactName == username);

            // map from customerEntity to CustomerDM
            var domainCustomer = new CustomerDM(
       new CustomerId(customerEntity.CustomerId),
       new CompanyName(customerEntity.CompanyName),
       new ContactName(customerEntity.ContactName),
       CreateIfNotNull<ContactTitle>(customerEntity.ContactTitle),
       CreateIfNotNull<Address>(customerEntity.Address),
       CreateIfNotNull<City>(customerEntity.City),
       CreateIfNotNull<Region>(customerEntity.Region),
       CreateIfNotNull<PostalCode>(customerEntity.PostalCode),
       CreateIfNotNull<Country>(customerEntity.Country),
       CreateIfNotNull<PhoneNumber>(customerEntity.Phone),
       CreateIfNotNull<FaxNumber>(customerEntity.Fax)
   );
            return domainCustomer;

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
        public void SaveCustomer(CustomerDM customer)
        {
            // Map CustomerDM back to Customer entity
            var customerEntity = new Customer
            {
                CustomerId = customer.CustomerId.Value,
                CompanyName = customer.CompanyName.Value,
                ContactName = customer.ContactName.Value,
                ContactTitle = customer.ContactTitle?.Value,
                Address = customer.Address?.Value,
                City = customer.City?.Value,
                Region = customer.Region?.Value,
                PostalCode = customer.PostalCode?.Value,
                Country = customer.Country?.Value,
                Phone = customer.Phone?.Value,
                Fax = customer.Fax?.Value
            };

            _context.Update(customerEntity);
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