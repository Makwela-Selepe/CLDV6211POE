using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Event_Ease.Models;
using Event_Ease.Data;

public class VenueController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly BlobService _blobService;

    public VenueController(ApplicationDbContext context, BlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    // GET: Venue
    public async Task<IActionResult> Index()
    {
        return View(await _context.Venue.ToListAsync());
    }

    // GET: Venue/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var venue = await _context.Venue.FindAsync(id);
        if (venue == null) return NotFound();

        return View(venue);
    }

    // GET: Venue/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Venue/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Venue venue)
    {
        if (!ModelState.IsValid) return View(venue);

        // Normalize name before checking or saving
        venue.VenueName = venue.VenueName?.Trim();
        venue.VenueLocation = venue.VenueLocation?.Trim();

        // Check for uniqueness using SQL collation for accuracy
        bool exists = await _context.Venue
            .AnyAsync(v =>
                EF.Functions.Collate(v.VenueName.Trim(), "SQL_Latin1_General_CP1_CI_AS") ==
                venue.VenueName);

        if (exists)
        {
            ModelState.AddModelError("VenueName", "A venue with this name already exists.");
            return View(venue);
        }

        // Handle image upload if provided
        if (venue.ImageFile != null)
        {
            venue.ImageUrl = await _blobService.UploadFileAsync(venue.ImageFile);
        }

        try
        {
            _context.Add(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("UNIQUE KEY constraint") == true)
            {
                ModelState.AddModelError("VenueName", "A venue with this name already exists.");
                return View(venue);
            }
            throw;
        }
    }

    // GET: Venue/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var venue = await _context.Venue.FindAsync(id);
        if (venue == null) return NotFound();

        return View(venue);
    }

    // POST: Venue/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Venue venue)
    {
        if (id != venue.VenueId) return NotFound();
        if (!ModelState.IsValid) return View(venue);

        venue.VenueName = venue.VenueName?.Trim();
        venue.VenueLocation = venue.VenueLocation?.Trim();

        var exists = await _context.Venue
            .AnyAsync(v =>
                v.VenueId != venue.VenueId &&
                EF.Functions.Collate(v.VenueName.Trim(), "SQL_Latin1_General_CP1_CI_AS") == venue.VenueName &&
                EF.Functions.Collate(v.VenueLocation.Trim(), "SQL_Latin1_General_CP1_CI_AS") == venue.VenueLocation);

        if (exists)
        {
            ModelState.AddModelError(string.Empty, "Another venue with the same name and location already exists.");
            return View(venue);
        }

        try
        {
            _context.Update(venue);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Venue.Any(e => e.VenueId == id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Venue/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var venue = await _context.Venue.FindAsync(id);
        if (venue == null) return NotFound();

        return View(venue);
    }

    // POST: Venue/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var venue = await _context.Venue.FindAsync(id);
        if (venue != null)
        {
            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
