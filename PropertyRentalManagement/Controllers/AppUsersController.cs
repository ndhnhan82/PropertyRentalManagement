using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRentalManagement.Data;
using PropertyRentalManagement.Models;
using System.Linq;
using System.Threading.Tasks;

public class AppUsersController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public AppUsersController(AppDbContext context,UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: AppUsers
    public async Task<IActionResult> Index()
    {
        // Get the current logged-in user
        var currentUser = await _userManager.GetUserAsync(User);

        // Get the list of all users from the database
        var userList = await _context.Users.ToListAsync();

        // Remove the current user from the list
        if (currentUser != null)
        {
            userList = userList.Where(u => u.Id != currentUser.Id).ToList();
        }

        // Return the modified list to the view
        return View(userList);
    }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appUser = await _context.AppUsers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (appUser == null)
        {
            return NotFound();
        }

        return View(appUser);
    }

    // GET: AppUsers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: AppUsers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Address,ProfilePicture")] AppUser appUser)
    {
        if (ModelState.IsValid)
        {
            _context.Add(appUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(appUser);
    }

    // GET: AppUsers/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appUser = await _context.AppUsers.FindAsync(id);
        if (appUser == null)
        {
            return NotFound();
        }
        return View(appUser);
    }

    // POST: AppUsers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Address,ProfilePicture,ConcurrencyStamp")] AppUser appUser)
    {
        if (id != appUser.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingUser = await _context.AppUsers.FindAsync(appUser.Id);

                if (existingUser == null)
                {
                    return NotFound();
                }

                _context.Entry(existingUser).OriginalValues["ConcurrencyStamp"] = appUser.ConcurrencyStamp;

                if (!_context.Entry(existingUser).Property("ConcurrencyStamp").IsModified)
                {
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user. Please reload the page and try again.");
                    return View(appUser);
                }

                _context.Entry(existingUser).CurrentValues.SetValues(appUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (AppUser)entry.Entity;
                var databaseEntry = entry.GetDatabaseValues();

                if (databaseEntry == null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes. The user was deleted by another user.");
                }
                else
                {
                    var databaseValues = (AppUser)databaseEntry.ToObject();

                    if (databaseValues.FirstName != clientValues.FirstName)
                    {
                        ModelState.AddModelError("FirstName", $"Current value: {databaseValues.FirstName}");
                    }

                    // Add similar checks for other properties

                    ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you got the original value. The edit operation was canceled and the current values in the database have been displayed. If you still want to edit this record, click the Save button again.");
                    appUser.ConcurrencyStamp = databaseValues.ConcurrencyStamp;
                    ModelState.Remove("ConcurrencyStamp");
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(appUser);
    }



    // GET: AppUsers/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appUser = await _context.AppUsers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (appUser == null)
        {
            return NotFound();
        }

        return View(appUser);
    }

    // POST: AppUsers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var appUser = await _context.AppUsers.FindAsync(id);
        _context.AppUsers.Remove(appUser);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AppUserExists(string id)
    {
        return _context.AppUsers.Any(e => e.Id == id);
    }
}
