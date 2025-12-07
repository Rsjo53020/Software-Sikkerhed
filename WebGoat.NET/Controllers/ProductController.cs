using WebGoatCore.Models;
using WebGoatCore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WebGoatCore.ViewModels;
using System.Threading.Tasks;

namespace WebGoatCore.Controllers
{
    [Route("[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly SupplierRepository _supplierRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            ProductRepository productRepository,
            IWebHostEnvironment webHostEnvironment,
            CategoryRepository categoryRepository,
            SupplierRepository supplierRepository,
            ILogger<ProductController> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string? nameFilter, int? selectedCategoryId)
        {
            if (selectedCategoryId.HasValue &&
                await _categoryRepository.GetByIdAsync(selectedCategoryId.Value) == null)
            {
                _logger.LogWarning("Invalid category id {CategoryId} in Search; resetting filter.",
                    selectedCategoryId.Value);
                selectedCategoryId = null;
            }

            var products = (await _productRepository
                .FindNonDiscontinuedProductsAsync(nameFilter, selectedCategoryId))
                .Select(p => new ProductListViewModel.ProductItem
                {
                    Product = p,
                    ImageUrl = GetImageUrlForProduct(p),
                })
                .ToList();

            var model = new ProductListViewModel
            {
                Products = products,
                ProductCategories = await _categoryRepository.GetAllCategoriesAsync(),
                SelectedCategoryId = selectedCategoryId,
                NameFilter = nameFilter
            };

            return View(model);
        }

        [HttpGet("{productId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int productId, short quantity = 1)
        {
            if (quantity <= 0)
            {
                quantity = 1;
            }

            var model = new ProductDetailsViewModel();

            try
            {
                var product = await _productRepository.GetProductByIdAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning("Details requested for non-existing product id {ProductId}.", productId);
                    model.ErrorMessage = "Product not found.";
                    return View(model);
                }

                model.Product = product;
                model.CanAddToCart = true; // evt. baseret på lagerstatus
                model.ProductImageUrl = GetImageUrlForProduct(product);
                model.Quantity = quantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting product details for id {ProductId}.", productId);
                model.ErrorMessage = "An unexpected error has occurred while loading the product.";
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var model = new ProductAddOrEditViewModel
            {
                AddsNew = true,
                ProductCategories = (System.Collections.Generic.IList<Category>)await _categoryRepository.GetAllCategoriesAsync(),
                Suppliers = await _supplierRepository.GetAllSuppliersAsync(),
                Product = new Product()
            };

            return View("AddOrEdit", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product product)
        {
            if (!ModelState.IsValid)
            {
                var vm = new ProductAddOrEditViewModel
                {
                    AddsNew = true,
                    ProductCategories = (System.Collections.Generic.IList<Category>)await _categoryRepository.GetAllCategoriesAsync(),
                    Suppliers = await _supplierRepository.GetAllSuppliersAsync(),
                    Product = product
                };

                return View("AddOrEdit", vm);
            }

            try
            {
                await _productRepository.AddAsync(product);
                _logger.LogInformation("Product {ProductId} created.", product.ProductId);

                return RedirectToAction(nameof(Edit), new { id = product.ProductId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding product.");

                ModelState.AddModelError(string.Empty, "An error occurred while saving the product.");
                var vm = new ProductAddOrEditViewModel
                {
                    AddsNew = true,
                    ProductCategories = (System.Collections.Generic.IList<Category>)await _categoryRepository.GetAllCategoriesAsync(),
                    Suppliers = await _supplierRepository.GetAllSuppliersAsync(),
                    Product = product
                };

                return View("AddOrEdit", vm);
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Edit requested for non-existing product id {ProductId}.", id);
                return NotFound();
            }

            var model = new ProductAddOrEditViewModel
            {
                AddsNew = false,
                ProductCategories = (System.Collections.Generic.IList<Category>) await _categoryRepository.GetAllCategoriesAsync(),
                Suppliers = await _supplierRepository.GetAllSuppliersAsync(),
                Product = product
            };

            return View("AddOrEdit", model);
        }

        [HttpPost("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductId)
            {
                _logger.LogWarning("Product id mismatch in Edit. Route id: {RouteId}, Model id: {ModelId}",
                    id, product.ProductId);
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var vmInvalid = new ProductAddOrEditViewModel
                {
                    AddsNew = false,
                    ProductCategories = (System.Collections.Generic.IList<Category>) await _categoryRepository.GetAllCategoriesAsync(),
                    Suppliers = await _supplierRepository.GetAllSuppliersAsync(),
                    Product = product
                };

                return View("AddOrEdit", vmInvalid);
            }

            try
            {
                var updatedProduct = await _productRepository.UpdateAsync(product);
                _logger.LogInformation("Product {ProductId} updated.", product.ProductId);

                var vm = new ProductAddOrEditViewModel
                {
                    AddsNew = false,
                    ProductCategories = (System.Collections.Generic.IList<Category>) await _categoryRepository.GetAllCategoriesAsync(),
                    Suppliers = await _supplierRepository.GetAllSuppliersAsync(),
                    Product = updatedProduct
                };

                return View("AddOrEdit", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating product {ProductId}.", product.ProductId);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");

                var vmError = new ProductAddOrEditViewModel
                {
                    AddsNew = false,
                    ProductCategories = (System.Collections.Generic.IList<Category>) await _categoryRepository.GetAllCategoriesAsync(),
                    Suppliers = await _supplierRepository.GetAllSuppliersAsync(),
                    Product = product
                };

                return View("AddOrEdit", vmError);
            }
        }

        private string GetImageUrlForProduct(Product? product)
        {
            if (product == null)
            {
                return "/Images/ProductImages/NoImage.jpg";
            }

            var relativePath = $"/Images/ProductImages/{product.ProductId}.jpg";
            var fileInfo = _webHostEnvironment.WebRootFileProvider.GetFileInfo(relativePath);

            if (!fileInfo.Exists)
            {
                _logger.LogDebug("No image found for product {ProductId}, using default image.", product.ProductId);
                return "/Images/ProductImages/NoImage.jpg";
            }

            return relativePath;
        }
    }
}