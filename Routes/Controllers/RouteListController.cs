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
            var tuple = new Tuple<Route, List<List<string>>>(route, strPlaces); //делать сразу с листом мест
            return View(tuple);
        }

        public async Task<IActionResult> Edit()
        {
            User user = await GetCurrentUserAsync();
            if (user != null) {
                Route route = new Route
                {
                    Name = "JustForTesting2",
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
                    InfoAbout = "Go v Destiny",
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
                _context.Places.Add(place2);
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
/*
 * 
 * try {
                //var locations = JSON.parse('@Html.Raw(JsonConvert.SerializeObject(@Model.Item2))')

                var locations = [
                    [-33.890542, 151.274856, '4'],
                    [-33.923036, 151.259052, '5'],
                    [-34.028249, 151.157507, 'Cronulla Beach'],
                    [-33.80010128657071, 151.28747820854187, 'Manly Beach',],
                    [-33.950198, 151.259302, 'Maroubra Beach']
                ];

                var map = new google.maps.Map(document.getElementById('map'), {
                    zoom: 10,
                    center: new google.maps.LatLng(-33.92, 151.25),
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                });


                var infowindow = new google.maps.InfoWindow();

                var marker, i;

                for (i = 0; i < locations.length; i++) {
                    marker = new google.maps.Marker({
                        position: new google.maps.LatLng(parseFloat(locations[i][0]), parseFloat(locations[i][1])),
                        map: map
                    });
                }
                google.maps.event.addListener(marker, 'click', (function (marker, i) {
                    return function () {
                        infowindow.setContent(locations[i][2]);
                        infowindow.open(map, marker);
                    }
                })(marker, i));
            }
            catch (e) {

            }
*/