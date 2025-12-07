using WebGoatCore.Models;
using WebGoatCore.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> AddOrderAsync(int productId, short quantity)
        {
            if (quantity <= 0)
            {
                TempData["Error"] = "Quantity must be greater than zero.";
                return RedirectToAction("Details", "Product", new { productId });
            }

            var product = await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
            {
                TempData["Error"] = $"Product {productId} was not found.";
                return RedirectToAction(nameof(Index));
            }

            var cart = GetCart();

            if (!cart.Items.TryGetValue(productId, out var cartItem))
            {
                cartItem = new CartItemDTO
                {
                    ProductId = productId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice,
                    Quantity = quantity
                };

                cart.Items.Add(productId, cartItem);
            }
            else
            {
                // Beskyt mod overflow – og udtryk tydeligt at vi lægger til
                checked
                {
                    cartItem.Quantity += quantity;
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

            if (!cart.Items.ContainsKey(productId))
            {
                TempData["Error"] = $"Product {productId} was not found in your cart.";
                return RedirectToAction(nameof(Index));
            }

            cart.Items.Remove(productId);
            SaveCart(cart);

            TempData["Message"] = "Product removed from cart.";
            return RedirectToAction(nameof(Index));
        }
    }
}