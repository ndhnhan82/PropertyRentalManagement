using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PropertyRentalManagement.Data;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    [Authorize(Roles = "Owner, Administrator, Manager")]
    public class UserRolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
		private IPasswordHasher<AppUser> _passwordHasher;

		public UserRolesController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IPasswordHasher<AppUser> passwordHash)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
			_passwordHasher = passwordHash;

		}
		public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach (AppUser user in users)
            {
                var thisViewModel = new UserRolesViewModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(thisViewModel);
            }
            return View(userRolesViewModel);
        }
        private async Task<List<string>> GetUserRoles(AppUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserRolesViewModel user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.LastName = user.LastName;
            ViewBag.FirstName = user.FirstName;

            // Collect roles in a list
            var roles = await _roleManager.Roles.ToListAsync();

            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                // Use await outside the loop
                var isInRole = await _userManager.IsInRoleAsync(user, role.Name);

                if (isInRole)
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                ProfilePicture = user.ProfilePicture
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // Delete messages associated with the user
            var userMessages = await _context.Messages
                .Where(m => m.ReceiverId == userId || m.SenderId == userId)
                .ToListAsync();

            _context.Messages.RemoveRange(userMessages);

            // Delete the user
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                // Optionally, you can return the same view to display error messages
                return View("Delete", new UserRolesViewModel { UserId = userId });
            }

            return RedirectToAction("Index");
        }


		public async Task<IActionResult> Edit(string id)
		{
			AppUser user = await _userManager.FindByIdAsync(id);
			if (user != null)
				return View(user);
			else
				return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> Edit(string id, string email, string password)
		{
			AppUser user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				if (!string.IsNullOrEmpty(email))
					user.Email = email;
				else
					ModelState.AddModelError("", "Email cannot be empty");

				if (!string.IsNullOrEmpty(password))
					user.PasswordHash = _passwordHasher.HashPassword(user, password);
				else
					ModelState.AddModelError("", "Password cannot be empty");

				if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
				{
					IdentityResult result = await _userManager.UpdateAsync(user);
					if (result.Succeeded)
						return RedirectToAction("Index");
					else
						Errors(result);
				}
			}
			else
				ModelState.AddModelError("", "User Not Found");
			return View(user);
		}

		private void Errors(IdentityResult result)
		{
			foreach (IdentityError error in result.Errors)
				ModelState.AddModelError("", error.Description);
		}
	}

}
