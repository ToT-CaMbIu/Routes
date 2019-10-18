using Routes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Routes.Components
{
    public class BestRouteViewComponent
    {
        RouteContext _context;
        public BestRouteViewComponent(RouteContext context)
        {
            _context = context;
        }
        public string Invoke()
        {
            var item = _context.Users.OrderByDescending(x => x.Rating).Take(1).FirstOrDefault();

            return $"BestUser: {item.UserName}  Rating: {item.Rating}";
        }
    }
}
