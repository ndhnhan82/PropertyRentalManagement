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
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessagesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            //// Count unread messages for the current user
            //var unreadMessagesCount = await CountUnreadMessages(userId);

            //// Pass the count to the view
            //ViewBag.UnreadMessagesCount = unreadMessagesCount;

            var appDbContext = _context.Messages
                .Include(m => m.ReadStatus)
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .Where(m => m.SenderId == userId || m.ReceiverId == userId);

            return View(await appDbContext.ToListAsync());
        }

     



        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.ReadStatus)
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageId == id);

            if (message == null)
            {
                return NotFound();
            }

            // Check if the current user is the receiver of the message
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && message.ReceiverId == currentUser.Id)
            {
                // Set the message status to "Read" if the current user is the receiver
                message.ReadStatus = _context.Statuses.FirstOrDefault(s => s.Description == "Read");

                // Update the database
                await _context.SaveChangesAsync();
            }

            return View(message);
        }


        // GET: Messages/Create
        public IActionResult Create()
        {
            var sender = _userManager.GetUserName(User); // Get the ID of the current logged-in user

            // Set default values for other properties
            ViewData["ReadStatusId"] = "Unread"; // Set the default value for ReadStatusId
            ViewData["ReceiverId"] = new SelectList(_context.AppUsers, "Id", "UserName");

            // Create a list with a single SelectListItem representing the sender
            var senderList = new List<SelectListItem>
    {
        new SelectListItem { Value = sender, Text = sender }
    };

            // Set the SenderId using the list
            ViewData["SenderId"] = new SelectList(senderList, "Value", "Text");

            return View();
        }


        // POST: Messages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,SenderId,ReceiverId,MessageContent,Timestamp,ReadStatusId")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.SenderId = _userManager.GetUserId(User); // Set the ID of the current logged-in user as the sender
                message.Timestamp = DateTime.Now; // Set the Timestamp to the current time
                var status = await _context.Statuses.FirstOrDefaultAsync(s => s.Description.Equals("Unread")); ;
                message.ReadStatusId = status.StatusId;
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Set default values for other properties
            ViewData["ReadStatusId"] = "Unread";
            ViewData["ReceiverId"] = new SelectList(_context.AppUsers, "Id", "UserName", message.ReceiverId);

            // You can directly set the SenderId without using SelectList
            ViewData["SenderId"] = message.SenderId;

            return View(message);
        }






        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["ReadStatusId"] = new SelectList(_context.Statuses, "StatusId", "Description", message.ReadStatusId);
            ViewData["ReceiverId"] = new SelectList(_context.AppUsers, "Id", "UserName", message.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.AppUsers, "Id", "UserName", message.SenderId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,SenderId,ReceiverId,MessageContent,Timestamp,ReadStatusId")] Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
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
            ViewData["ReadStatusId"] = new SelectList(_context.Statuses, "StatusId", "Description", message.ReadStatusId);
            ViewData["ReceiverId"] = new SelectList(_context.AppUsers, "Id", "UserName", message.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.AppUsers, "Id", "UserName", message.SenderId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.ReadStatus)
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
        

    }
}
