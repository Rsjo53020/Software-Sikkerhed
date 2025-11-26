using System;

namespace WebGoatCore.Models
{
/// <summary>
/// Customer Domain Entity – repræsenterer en kunde i systemet.
/// Sikkerhed:
///  - Ingen setters → immutability (Secure By Design kap. 4 og 6)
///  - Konsistens ved konstruktion
///  - Strong typing via Domain Primitives
/// </summary>
 public sealed class Customer
    {
        public CustomerId   CustomerId   { get; }
        public CompanyName  CompanyName  { get; private set; }
        public ContactName  ContactName  { get; private set; }

        public ContactTitle? ContactTitle { get; private set; }
        public Address?      Address      { get; private set; }
        public City?         City         { get; private set; }
        public Region?       Region       { get; private set; }
        public PostalCode?   PostalCode   { get; private set; }
        public Country?      Country      { get; private set; }
        public PhoneNumber?  Phone        { get; private set; }
        public FaxNumber?    Fax          { get; private set; }

        public Customer(
            CustomerId customerId,
            CompanyName companyName,
            ContactName contactName,
            ContactTitle? contactTitle,
            Address? address,
            City? city,
            Region? region,
            PostalCode? postalCode,
            Country? country,
            PhoneNumber? phone,
            FaxNumber? fax)
        {
            CustomerId  = customerId  ?? throw new ArgumentNullException(nameof(customerId));
            CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
            ContactName = contactName ?? throw new ArgumentNullException(nameof(contactName));

            ContactTitle = contactTitle;
            Address = address;
            City = city;
            Region = region;
            PostalCode = postalCode;
            Country = country;
            Phone = phone;
            Fax = fax;
        }

        /// <summary>
        /// Domænemetode der opdaterer kundens konto-info.
        /// Her holder vi alle regler ét sted, i domænet – ikke i controlleren.
        /// </summary>
        public void ChangeAccountInfo(
            CompanyName? companyName,
            ContactTitle? contactTitle,
            Address? address,
            City? city,
            Region? region,
            PostalCode? postalCode,
            Country? country)

        {
            if (companyName is not null)
                CompanyName = companyName;

            ContactTitle = contactTitle ?? ContactTitle;
            Address = address ?? Address;
            City = city ?? City;
            Region = region ?? Region;
            PostalCode = postalCode ?? PostalCode;
            Country = country ?? Country;
        }
    }
}