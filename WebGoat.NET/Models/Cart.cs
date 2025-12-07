using System.Collections.Generic;
using System.Linq;

namespace WebGoatCore.Models
{
    public class Cart
    {
        public IDictionary<int, CartItemDTO> Items { get; set; }
            = new Dictionary<int, CartItemDTO>();

        public double SubTotal => Items.Values.Sum(i => i.LineTotal);
    }
}
