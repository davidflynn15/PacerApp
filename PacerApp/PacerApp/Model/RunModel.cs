using GPSTracker;
using PacerAppUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;

namespace PacerApp
{
    public class RunModel
    {
        //properties
        public DateTime StartTime { get; set; }     //date & time I started run
        public double ElapsedTime { get; set; }     //in seconds
        public double Distance { get; set; }        //in miles
        public string PaceGoal { get; set; }        //minutes/mile
        public double PaceActual { get; set; }
        public List<GPSCoordinate> Locations { get; set; } = new List<GPSCoordinate>();

        //------------------------------------------------------
        //constructor
        //public Run()
        //{
        //    Locations = new List<GPSCoordinates>();
        //}
    }
}
