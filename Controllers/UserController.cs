using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleLogisticSystem.Data;
using SimpleLogisticSystem.Interfaces;
using SimpleLogisticSystem.Models;
using SimpleLogisticSystem.ViewModels;
using SimpleLogisticSystem.ViewModels.Users;
using System.Security.Claims;

namespace SimpleLogisticSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(IUserRepository userRepository, ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userRepository = userRepository;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /User/Index
        // Retrieves user details by user ID.
        public IActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = _context.Users.Where(u => u.Id == id).Select(u => new UserDetailViewModel
            {
                UserName = u.UserName,
                AccountType = u.AccountType,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Address = u.Address,
                Warehouses = _context.Warehouses.Where(w => w.AppUserId == u.Id).ToList(),
                Items = _context.Items.Where(i => i.AppUserId == u.Id).ToList(),
                Orders = _context.Orders.Where(o => o.AppUserId == u.Id).ToList(),
                AppUserId = u.Id
            }).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: /User/Edit
        // Dsiplays the edit user details form.
        [HttpGet]
        public IActionResult EditDetail(string id)
        {
            var currentUser = _context.Users.Where(u => u.Id == id).Select(u => new EditUserDetailViewModel
            {
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Address = u.Address,
                AppUserId = u.Id
            }).FirstOrDefault();

            if (currentUser == null) return NotFound();

            return View(currentUser);
        }

        // POST: /User/Edit
        // Handles the submission of the edit user details form.
        [HttpPost]
        public IActionResult EditDetail(EditUserDetailViewModel eumodelVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit user details!");
                return View("Edit", eumodelVM);
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == eumodelVM.AppUserId);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = eumodelVM.UserName;
            user.FirstName = eumodelVM.FirstName;
            user.LastName = eumodelVM.LastName;
            user.Address = eumodelVM.Address;

            _context.Users.Update(user);

            return RedirectToAction("Index", new { id = eumodelVM.AppUserId });
        }

        // GET: /User/ChangePassword
        // Displays the change password form.
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /User/ChangePassword
        // Handles the submission of the change password form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangeUserPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Attempt to change the user's password.
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                // Add errors to the model state if the password change fails.
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // Refresh the sign-in cookie.
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("ChangePasswordConfirmation", "User");
        }

        // GET: /User/ChangePasswordConfirmation
        // Displays the change password confirmation page.
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }
    }
}
