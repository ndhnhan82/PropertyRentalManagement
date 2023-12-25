using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentalManagement.Data;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    [Authorize(Roles = "Owner,Administrator, Manager")]
    public class BuildingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BuildingsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Buildings
        public async Task<IActionResult> Index()
        {
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View(_context.Buildings.Include(b => b.PropertyOwner).ToList());
        }

        // GET: Buildings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings
                .Include(b => b.PropertyOwner)
                .FirstOrDefaultAsync(m => m.BuildingId == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // GET: Buildings/Create
        public async Task<IActionResult> CreateAsync()
        {
            var owners = await _userManager.GetUsersInRoleAsync("Owner");

            ViewData["PropertyOwnerId"] = new SelectList(owners, "Id", "UserName");

            return View();
        }

        // POST: Buildings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BuildingId,Name,Address,Description,PropertyOwnerId")] Building building)
        {
            if (ModelState.IsValid)
            {
                _context.Add(building);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var owners = await _userManager.GetUsersInRoleAsync("Owner");

            ViewData["PropertyOwnerId"] = new SelectList(owners, "Id", "UserName", building.PropertyOwnerId);
            return View(building);
        }

        // GET: Buildings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            var owners = await _userManager.GetUsersInRoleAsync("Owner");

            ViewData["PropertyOwnerId"] = new SelectList(owners, "Id", "UserName", building.PropertyOwnerId);
            return View(building);
        }

        // POST: Buildings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BuildingId,Name,Address,Description,PropertyOwnerId")] Building building)
        {

            if (id != building.BuildingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(building);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildingExists(building.BuildingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var owners = await _userManager.GetUsersInRoleAsync("Owner");

            ViewData["PropertyOwnerId"] = new SelectList(owners, "Id", "UserName", building.PropertyOwnerId);
            return View(building);
        }

        // GET: Buildings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var building = await _context.Buildings
                .Include(b => b.PropertyOwner)
                .FirstOrDefaultAsync(m => m.BuildingId == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if the building has any associated apartments
            if (BuildingHasApartments(id))
            {
                TempData["ErrorMessage"] = "Cannot delete building with associated apartments. Delete all apartment belong to this building before delete it.";
                return RedirectToAction(nameof(Index));
            }

            var building = await _context.Buildings.FindAsync(id);
            if (building != null)
            {
                _context.Buildings.Remove(building);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BuildingHasApartments(int buildingId)
        {
            // Check if the building has any associated apartments
            return _context.Apartments.Any(a => a.BuildingId == buildingId);
        }

        private bool BuildingExists(int id)
        {
            return _context.Buildings.Any(e => e.BuildingId == id);
        }


    }
}
