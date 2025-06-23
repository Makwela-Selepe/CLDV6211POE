using Event_Ease.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Event_Ease.Data;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Event_Ease.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Event.Include(e => e.Venue).ToListAsync();
            return View(events);
        }

        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Event eventObj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventObj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] = new SelectList(_context.Venue ,"VenueId", "VenueName", eventObj.VenueId);
            return View(eventObj);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var eventObj = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (eventObj == null) return NotFound();

            return View(eventObj);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var eventObj = await _context.Event.FindAsync(id);
            if (eventObj == null) return NotFound();

            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", eventObj.VenueId);
            return View(eventObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, Event eventObj)
        {
            if (id != eventObj.EventId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventObj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Event.Any(e => e.EventId == eventObj.EventId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", eventObj.VenueId);
            return View(eventObj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventObj = await _context.Event
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventObj == null)
                return NotFound();

            // Check for existing bookings
            bool hasBookings = await _context.Booking.AnyAsync(b => b.EventId == id);

            if (hasBookings)
            {
                // Optionally, add a ModelState error or TempData message
                TempData["Error"] = "Cannot delete this event because there are bookings associated with it.";
                return RedirectToAction(nameof(Delete), new { id }); // Return to confirmation page with error
            }

            _context.Event.Remove(eventObj);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
