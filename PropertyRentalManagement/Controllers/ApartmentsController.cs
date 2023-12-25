using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentalManagement.Data;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    public class ApartmentsController : Controller
    {
        private readonly AppDbContext _context;

        public ApartmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Apartments
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Apartments.Include(a => a.Building).Include(a => a.Size).Include(a => a.Status).Include(a => a.Tenant);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Apartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.Building)
                .Include(a => a.Size)
                .Include(a => a.Status)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }
        [Authorize(Roles ="Owner,Administrator,Manager")]

        // GET: Apartments/Create
        public IActionResult Create()
        {
            ViewData["BuildingId"] = new SelectList(_context.Buildings, "BuildingId", "Address");
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "Description");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "Description");
            ViewData["TenantId"] = new SelectList(_context.AppUsers, "Id", "UserName");
            return View();
        }
        [Authorize(Roles = "Owner,Administrator,Manager")]


        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApartmentId,BuildingId,ApartmentNumber,Description,Rent,StatusId,SizeId,TenantId")] Apartment apartment)
        {
            // Check if the ApartmentNumber already exists
            if (IsApartmentNumberExists(apartment.ApartmentNumber))
            {
                ModelState.AddModelError("ApartmentNumber", "Apartment number " + apartment.ApartmentNumber + " already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(apartment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuildingId"] = new SelectList(_context.Buildings, "BuildingId", "Address", apartment.BuildingId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "Description", apartment.SizeId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "Description", apartment.StatusId);
            ViewData["TenantId"] = new SelectList(_context.AppUsers, "Id", "UserName", apartment.TenantId);
            return View(apartment);
        }


        [Authorize(Roles = "Owner,Administrator,Manager")]

        // GET: Apartments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            ViewData["BuildingId"] = new SelectList(_context.Buildings, "BuildingId", "Address", apartment.BuildingId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "Description", apartment.SizeId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "Description", apartment.StatusId);
            ViewData["TenantId"] = new SelectList(_context.AppUsers, "Id", "UserName", apartment.TenantId);
            return View(apartment);
        }
        [Authorize(Roles = "Owner,Administrator,Manager")]

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApartmentId,BuildingId,ApartmentNumber,Description,Rent,StatusId,SizeId,TenantId")] Apartment apartment)
        {
            if (id != apartment.ApartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApartmentExists(apartment.ApartmentId))
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
            ViewData["BuildingId"] = new SelectList(_context.Buildings, "BuildingId", "Address", apartment.BuildingId);
            ViewData["SizeId"] = new SelectList(_context.Sizes, "SizeId", "Description", apartment.SizeId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "Description", apartment.StatusId);
            ViewData["TenantId"] = new SelectList(_context.AppUsers, "Id", "UserName", apartment.TenantId);
            return View(apartment);
        }
        [Authorize(Roles = "Owner,Administrator,Manager")]


        // GET: Apartments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.Building)
                .Include(a => a.Size)
                .Include(a => a.Status)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        [Authorize(Roles = "Owner,Administrator,Manager")]

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment != null)
            {
                _context.Apartments.Remove(apartment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartments.Any(e => e.ApartmentId == id);
        }
        private bool IsApartmentNumberExists(int aprtNumber)
        {
            return _context.Apartments.Any(e => e.ApartmentNumber == aprtNumber);
        }
    }
}
