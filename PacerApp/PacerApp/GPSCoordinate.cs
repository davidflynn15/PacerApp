using System;
using System.Collections.Generic;
using System.Text;

namespace PacerAppUI
{
    public class GPSCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double ElapsedTimeInSec { get; set; }
        public string message { get; set; }

        //not using constructor at this time
        // public GPSCoordinate(double latitude, double longitude) { }
    }
}
