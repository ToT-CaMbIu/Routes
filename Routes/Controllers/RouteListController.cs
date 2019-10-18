using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Routes.Models;
using Microsoft.EntityFrameworkCore;

namespace Routes.Controllers
{
    public class RouteListController : Controller
    {
        UserManager<User> _userManager;
        RouteContext _context;
        public RouteListController(UserManager<User> userManager, RouteContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index() => View(_context.Routes.ToList());

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [HttpGet]
        public async Task<int?> GetCurrentUserId()
        {
            User user = await GetCurrentUserAsync();
            return user?.Id;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var route = await _context.Routes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            Route route = await _context.Routes.FindAsync(id);
            if (route != null)
            {
                var result = _context.Routes.Remove(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes
                .FirstOrDefaultAsync(m => m.Id == id);
            var places = _context.Places.ToList();
            if (route == null)
            {
                return NotFound();
            }
            var tuple = new Tuple<Route, List<Place>>(route, places);
            return View(tuple);
        }

        public async Task<IActionResult> Edit()
        {
            User user = await GetCurrentUserAsync();
            if (user != null) {
                Route route = new Route
                {
                    Name = "test3",
                    isForPremium = false,
                    CountOfViews = 0,
                    RouteUserId = user.Id,
                };
                _context.Routes.Add(route);
                await _context.SaveChangesAsync();
                int id = route.Id;
                Place place = new Place
                {
                    Lt = 32.32,
                    Lg = 32.32,
                    InfoAbout = "test",
                    RouteId = id
                };
                Place place2 = new Place
                {
                    Lt = 33.33,
                    Lg = 33.33,
                    InfoAbout = "test",
                    RouteId = id
                };
                _context.Places.Add(place);
                await _context.SaveChangesAsync();

                var tmp = await _context.Routes
                .FirstOrDefaultAsync(m => m.Id == id);

                tmp.Places.Add(place);
                tmp.Places.Add(place2);

                await _context.SaveChangesAsync();

                List<Route> currentUser = await _context.Routes.Include(host => host.Places).ToListAsync();

                return Ok(currentUser);
            }
            return NotFound();
        }
    }
}