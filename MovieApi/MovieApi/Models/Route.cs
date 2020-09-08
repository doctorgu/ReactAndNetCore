using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Models
{
    public class Route
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Component { get; set; }
        public IEnumerable<Route> Routes { get; set; }
    }
}
