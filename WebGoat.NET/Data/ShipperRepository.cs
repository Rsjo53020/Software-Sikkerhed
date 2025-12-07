using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebGoatCore.Models;

namespace WebGoatCore.Data
{
    public class ShipperRepository
    {
        private readonly NorthwindContext _context;
        private static readonly CultureInfo EnUsCulture = CultureInfo.GetCultureInfo("en-US");


        public ShipperRepository(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<int, string>> GetShippingOptionsAsync(decimal orderSubtotal)
        {
            var shippers = await _context.Shippers
                .AsNoTracking()
                .ToListAsync();

            return shippers.ToDictionary(
                s => s.ShipperId,
                s => GetShippingCostString(s, orderSubtotal));
        }

        private string GetShippingCostString(Shipper shipper, decimal orderSubtotal)
        {
            if (shipper == null) throw new ArgumentNullException(nameof(shipper));

            var shippingCost = Math.Round(shipper.GetShippingCost(orderSubtotal), 2)
                .ToString("F2", EnUsCulture); // sikrer 2 decimaler og en-US format

            return $"{shipper.CompanyName} {shipper.ServiceName} - {shippingCost}";
        }

        public async Task<Shipper?> GetShipperByShipperIdAsync(int shipperId)
        {
            return await _context.Shippers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShipperId == shipperId);
        }

        /// <summary>Gets a tracking number for the supplied shipper</summary>
        /// <param name="shipper">The shipper object</param>
        /// <returns>The tracking number</returns>
        /// <remarks>
        /// Simulates getting a tracking number.  In the real world, we'd contact their web service
        /// to get a real number.
        /// Source for tracking number formats: http://answers.google.com/answers/threadview/id/207899.html
        /// </remarks>
        public string GetNextTrackingNumber(Shipper shipper)
        {
            var random = Random.Shared;
            var cN = shipper.CompanyName;

            if (cN.Contains("UPS"))
            {
                return string.Format("1Z{0} {1} {2} {3} {4} {5}",
                random.Next(999).ToString("000"),
                random.Next(999).ToString("000"),
                random.Next(99).ToString("00"),
                random.Next(9999).ToString("0000"),
                random.Next(999).ToString("000"),
                random.Next(9).ToString("0"));
            }
            else if (cN.Contains("FedEx"))
            {
                return string.Format("{0}{1}",
                    random.Next(999999).ToString("000000"),
                    random.Next(999999).ToString("000000"));
            }
            else if (cN.Contains("US Postal Service"))
            {
                return string.Format("{0} {1} {2}",
                    random.Next(9999).ToString("0000"),
                    random.Next(9999).ToString("0000"),
                    random.Next(99).ToString("00"));
            }
            else
            {
                return "Could not get a tracking number";
            }
        }
    }
}
