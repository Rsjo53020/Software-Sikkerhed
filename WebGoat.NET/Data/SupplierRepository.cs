using WebGoatCore.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WebGoatCore.Data
{
    public class SupplierRepository
    {
        private readonly NorthwindContext _context;

        public SupplierRepository(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers
                .AsNoTracking()
                .OrderBy(s => s.CompanyName)
                .ToListAsync();
        }
    }
}
