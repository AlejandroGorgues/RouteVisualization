using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace RouteVisualization
{
    class CustomMap : Map
    {
        public List<Position> RouteCoordinates { get; set; }

        public CustomMap()
        {
            RouteCoordinates = new List<Position>();
        }
    }
}
