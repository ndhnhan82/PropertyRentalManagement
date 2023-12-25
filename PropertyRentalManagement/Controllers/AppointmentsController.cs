using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PropertyRentalManagement.Data;
using PropertyRentalManagement.Models;

namespace PropertyRentalManagement.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            IQueryable<Appointment> appDbContext;

            if (User.IsInRole("Tenant"))
            {
                // If the user is a tenant, filter appointments for that tenant only
                var currentUser = await _userManager.GetUserAsync(User);
                appDbContext = _context.Appointments
                    .Where(a => a.PotentialTenantId == currentUser.Id)
                    .Include(a => a.PotentialTenant)
                    .Include(a => a.PropertyManager)
                    .Include(a => a.Status);
            }
            else
            {
                // If the user is not a tenant, show all appointments
                appDbContext = _context.Appointments
                    .Include(a => a.PotentialTenant)
                    .Include(a => a.PropertyManager)
                    .Include(a => a.Status);
            }

            return View(await appDbContext.ToListAsync());
        }


        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.PotentialTenant)
                .Include(a => a.PropertyManager)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> CreateAsync()
        {
            var tenants = await _userManager.GetUsersInRoleAsync("Tenant");

            var managers = await _userManager.GetUsersInRoleAsync("Manager");
            if (User.IsInRole("Tenant"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                tenants.Clear();
                tenants.Add(currentUser);
            }

            ViewData["PotentialTenantId"] = new SelectList(tenants, "Id", "UserName");
            ViewData["PropertyManagerId"] = new SelectList(managers, "Id", "UserName");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "Description");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,PropertyManagerId,PotentialTenantId,ScheduledDate,Description,StatusId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PotentialTenantId"] = new SelectList(_context.AppUsers, "Id", "UserName", appointment.PotentialTenantId);
            ViewData["PropertyManagerId"] = new SelectList(_context.AppUsers, "Id", "UserName", appointment.PropertyManagerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "Description", appointment.StatusId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["PotentialTenantId"] = new SelectList(_context.AppUsers, "Id", "UserName", appointment.PotentialTenantId);
            ViewData["PropertyManagerId"] = new SelectList(_context.AppUsers, "Id", "UserName", appointment.PropertyManagerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "Description", appointment.StatusId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,PropertyManagerId,PotentialTenantId,ScheduledDate,Description,StatusId")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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
            ViewData["PotentialTenantId"] = new SelectList(_context.AppUsers, "Id", "UserName", appointment.PotentialTenantId);
            ViewData["PropertyManagerId"] = new SelectList(_context.AppUsers, "Id", "UserName", appointment.PropertyManagerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "Description", appointment.StatusId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.PotentialTenant)
                .Include(a => a.PropertyManager)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
