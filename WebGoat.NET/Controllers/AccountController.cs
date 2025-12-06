using WebGoatCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebGoatCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using WebGoatCore.Models;

namespace WebGoatCore.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly CustomerRepository _customerRepository;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            CustomerRepository customerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _customerRepository = customerRepository;
        }

        // Helper til at hente nuværende brugernavn
        private string? CurrentUserName => _userManager.GetUserName(User);

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // Redirect – brug ReturnUrl hvis den er lokal, ellers til Home
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            HttpContext.Session.Set("Cart", new Cart());

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            await _signInManager.SignOutAsync();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email is already registered.");
                return View(model);
            }

            var user = new IdentityUser(model.Username)
            {
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            _customerRepository.CreateCustomer(
                model.CompanyName,
                model.Username,
                model.Address,
                model.City,
                model.Region,
                model.PostalCode,
                model.Country);

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult MyAccount() => View();

        public IActionResult ViewAccountInfo()
        {
            var username = CurrentUserName;
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError(string.Empty, "We don't recognize your login. Please log in and try again.");
                return View(new ChangeAccountInfoViewModel());
            }

            var customer = _customerRepository.GetCustomerByUsername(username);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "We don't recognize your customer Id. Please log in and try again.");
                return View(new ChangeAccountInfoViewModel());
            }

            var customerViewModel = new ChangeAccountInfoViewModel
            {
                CompanyName = customer.CompanyName,
                ContactTitle = customer.ContactTitle,
                Address = customer.Address,
                City = customer.City,
                Region = customer.Region,
                PostalCode = customer.PostalCode,
                Country = customer.Country
            };

            return View(customerViewModel);
        }

        [HttpGet]
        public IActionResult ChangeAccountInfo()
        {
            var username = CurrentUserName;
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError(string.Empty, "We don't recognize your login. Please log in and try again.");
                return View(new ChangeAccountInfoViewModel());
            }

            var customer = _customerRepository.GetCustomerByUsername(username);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "We don't recognize your customer Id. Please log in and try again.");
                return View(new ChangeAccountInfoViewModel());
            }

            return View(new ChangeAccountInfoViewModel
            {
                CompanyName = customer.CompanyName,
                ContactTitle = customer.ContactTitle,
                Address = customer.Address,
                City = customer.City,
                Region = customer.Region,
                PostalCode = customer.PostalCode,
                Country = customer.Country
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeAccountInfo(ChangeAccountInfoViewModel model)
        {
            var username = CurrentUserName;
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError(string.Empty, "We don't recognize your login. Please log in and try again.");
                return View(model);
            }

            var customer = _customerRepository.GetCustomerByUsername(username);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "We don't recognize your customer Id. Please log in and try igen.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            customer.CompanyName = model.CompanyName ?? customer.CompanyName;
            customer.ContactTitle = model.ContactTitle ?? customer.ContactTitle;
            customer.Address = model.Address ?? customer.Address;
            customer.City = model.City ?? customer.City;
            customer.Region = model.Region ?? customer.Region;
            customer.PostalCode = model.PostalCode ?? customer.PostalCode;
            customer.Country = model.Country ?? customer.Country;

            _customerRepository.SaveCustomer(customer);
            model.UpdatedSucessfully = true;

            // Evt. PRG pattern: RedirectToAction("ChangeAccountInfo") i stedet for at vise samme view
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "We don't recognize your login. Please log in and try again.");
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return View("ChangePasswordSuccess");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddUserTemp()
        {
            return View(new AddUserTempViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserTemp(AddUserTempViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUserByName = await _userManager.FindByNameAsync(model.NewUsername);
            if (existingUserByName != null)
            {
                ModelState.AddModelError(nameof(model.NewUsername), "A user with that username already exists.");
                return View(model);
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(model.NewEmail);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(nameof(model.NewEmail), "A user with that email address already exists.");
                return View(model);
            }

            var user = new IdentityUser(model.NewUsername)
            {
                Email = model.NewEmail
            };

            var result = await _userManager.CreateAsync(user, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            if (model.MakeNewUserAdmin)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            model.CreatedUser = true;
            return View(model);
        }
    }
}