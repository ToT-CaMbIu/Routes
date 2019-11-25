using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Routes.Models;
using Routes.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Routes.Controllers
{
    public class PagedData<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }
    }

    public class RouteListController : Controller
    {
        UserManager<User> _userManager;
        RouteContext _context;
        public RouteListController(UserManager<User> userManager, RouteContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        //public IActionResult Index() => View(_context.Routes.ToList());

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

        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id == null)
                return NotFound();
            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommentConfirmed(int? id)
        {
            Comment comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                var result = _context.Comments.Remove(comment);
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
                    UserName = user.UserName,
                    CountOfViews = 0,
                    RouteUserId = user.Id,
                };
                _context.Routes.Add(tmp_route);
                int id = tmp_route.Id;
                for (int i = 0; i < route.Places.Count(); i++)
                {
                    if(route.Places[i].Lg < -180.0 || route.Places[i].Lg > 180.0 
                        || route.Places[i].Lt < -90.0 || route.Places[i].Lg > 90.0)
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

                List<Route> currentUser = await _context.Routes.Include(host => host.Places).ToListAsync(); //TODO add signalr

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
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == route.RouteUserId);
            user.Rating++;
            await _context.SaveChangesAsync();
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
            List<Comment> comments = new List<Comment>();
            foreach (var item in _context.Comments.ToList())
            {
                if (item.RouteId == route.Id)
                    comments.Add(item);
            }
            var tuple_comments_and_comments = new Tuple<Route, List<Comment>>(route, comments);
            var tuple = new Tuple<Tuple<Route, List<Comment>>, List<List<string>>>(tuple_comments_and_comments, strPlaces);
            return View(tuple);
        }

        [HttpPost]
        public async Task<IActionResult> Details(int id, string NewComment)
        {
            User user = await GetCurrentUserAsync();
            if (user != null)
            {
                Comment tmp_comment = new Comment
                {
                    RouteId = id,
                    CommentUserId = user.Id,
                    Content = NewComment,
                    UserName = user.UserName,
                };
                _context.Comments.Add(tmp_comment);
                await _context.SaveChangesAsync();
                return LocalRedirect("~/RouteList/Details/" + id.ToString());
            }
            return NotFound();
        }

        public const int PageSize = 5;

        public ActionResult Index()
        {
            var routes = new PagedData<Route>();

            routes.Data = _context.Routes.Skip(PageSize * 0).Take(PageSize).ToList();
            routes.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)_context.Routes.Count() / PageSize));

            return View(routes);
        }

        public ActionResult RouteListAjax(int page)
        {
            var routes = new PagedData<Route>();

            routes.Data = _context.Routes.Skip(PageSize * (page - 1)).Take(PageSize).ToList();
            routes.NumberOfPages = Convert.ToInt32(Math.Ceiling((double)_context.Routes.Count() / PageSize));
            routes.CurrentPage = page;

            return PartialView(routes);
        }
    }
}