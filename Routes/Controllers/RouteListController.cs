using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Routes.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(PlacesViewModel route)
        {
            User user = await GetCurrentUserAsync();
            if (user != null)
            {
                Route tmp_route = new Route
                {
                    Name = route.Name,
                    isForPremium = false,
                    CountOfViews = 0,
                    RouteUserId = user.Id,
                };
                _context.Routes.Add(tmp_route);
                int id = tmp_route.Id;
                for (int i = 0; i < route.Places.Count(); i++)
                {
                    if(route.Places[i].Lt == -360.0)
                        return NotFound();
                    Place tmp_place = new Place
                    {
                        Lt = route.Places[i].Lt,
                        Lg = route.Places[i].Lg,
                        InfoAbout = route.Places[i].Info,
                        RouteId = id
                    };
                    _context.Places.Add(tmp_place);
                }

                await _context.SaveChangesAsync();

                List<Route> currentUser = await _context.Routes.Include(host => host.Places).ToListAsync();

                return Ok(currentUser);
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
            List<List<string>> strPlaces = new List<List<string>>();
            foreach (var item in places)
            {
                if(item.RouteId == route.Id)
                {
                    List<string> tmp = new List<string>();
                    tmp.Add(item.Lg.ToString());
                    tmp.Add(item.Lt.ToString());
                    tmp.Add(item.InfoAbout);
                    strPlaces.Add(tmp);
                }
            }
            var tuple = new Tuple<Route, List<List<string>>>(route, strPlaces);
            return View(tuple);
        }
    }
}