using GPSTracker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;

namespace PacerApp
{
    public class RunViewModel : ObservableObject
    {
        //--------------------------------------------------------------------
        //variables
        public Stopwatch stopwatch;
        
        private const double _speechInterval = 15;  //240 = 4 minutes
        private const double _gpsIntervalInSec = 7;  
        private double _nextTextToSpeech = 0;
        private double _nextGPSReading = 0;

        private double _distance;
        private double _paceActual;

        private string _dispElapsedTime;

        //---------------------------------------------------------------------
        //properties
        public bool IsDone { get; set; }
        public bool IsRestart { get; set; }
        public DateTime StartTime { get; set; }

        public string DispElapsedTime
        {
            get { return _dispElapsedTime; }
            set 
            { 
                _dispElapsedTime = value;
                RaisePropertyChangedEvent(nameof(DispElapsedTime));
            }
        }

        //distance in miles
        public double Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                DispDistance = _distance.ToString("0.00");
                RaisePropertyChangedEvent(nameof(Distance));
                RaisePropertyChangedEvent(nameof(DispDistance));
            }
        }

        public string DispDistance { get; set; }
        
        public string PaceGoal { get; set; }

        public double PaceActual
        {
            get { return _paceActual; }
            set
            {
                _paceActual = value;
                DispPaceActual = _paceActual.ToString("0.00");
                RaisePropertyChangedEvent(nameof(PaceActual));
                RaisePropertyChangedEvent(nameof(DispPaceActual));
            }
        }

        public string DispPaceActual { get; set; }

        public List<GPSCoordinates> Locations { get; set; } = new List<GPSCoordinates>();

        //------------------------------------------------------
        //constructor
        public RunViewModel()
        {
            //init fields
            StartTime = DateTime.Now;
            stopwatch = new Stopwatch();
            _nextTextToSpeech = _speechInterval;

            IsDone = false;
            IsRestart = false;

            //below are for testing
            PaceGoal = "8:00";

            ////sample coordinates for testing
            //Locations.Add(new GPSCoordinates { Latitude = 42.1659944, Longitude = -71.6157592 });
            //Locations.Add(new GPSCoordinates { Latitude = 42.165795,  Longitude = -71.613325  });
            //Locations.Add(new GPSCoordinates { Latitude = 42.170268,  Longitude = -71.606421  });
            //Locations.Add(new GPSCoordinates { Latitude = 42.1659919, Longitude = -71.6157048 });
            //Locations.Add(new GPSCoordinates { Latitude = 42.175532,  Longitude = -71.605368  });
            //Locations.Add(new GPSCoordinates { Latitude = 42.1659944, Longitude = -71.6157592 });
            //Locations.Add(new GPSCoordinates { Latitude = 42.168665,  Longitude = -71.614956  });
            //Locations.Add(new GPSCoordinates { Latitude = 42.166301,  Longitude = -71.612756  });
            //Locations.Add(new GPSCoordinates { Latitude = 42.165795,  Longitude = -71.613325  });
            //Locations.Add(new GPSCoordinates { Latitude = 42.1659944, Longitude = -71.6157592 });
        }

        //-------------------------------------------------------
        //Methods
        public void SerializeRunData()
        {
            Run run = new Run();
            run.StartTime = StartTime;
            run.ElapsedTime = stopwatch.Elapsed.TotalSeconds;
            run.Distance = Distance;
            run.PaceGoal = PaceGoal;
            run.PaceActual = PaceActual;
            run.Locations = Locations;

            //now serialize to json file
            string output = JsonConvert.SerializeObject(run);
            Console.WriteLine(output);
        }

        public async Task StartRun()
        {
            while (!IsDone)
            {
                await GetElapsedTimeAsync();
                await GetLocationAsync();
                await SpeakNowRunnerPromptAsync();
            }

            if (IsDone)
            {
                //get final stats
                Distance += GPSLocation.CalcDistance(
                    Locations[Locations.Count - 1], Locations[Locations.Count - 2]);
                PaceActual = (stopwatch.Elapsed.TotalSeconds / 60) / Distance;
                DispElapsedTime = ($"{stopwatch.Elapsed.Minutes.ToString("0")}:{stopwatch.Elapsed.Seconds.ToString("00")}");
            }
        }

        public async Task GetLocationAsync()
        {
            if ((stopwatch.Elapsed.TotalSeconds / _nextGPSReading) >= 1.0)
            {
                GPSCoordinates coordinates = await GPSLocation.GetCoordinatesAsync(stopwatch.Elapsed.TotalSeconds);
                Locations.Add(coordinates);

                if (Locations.Count > 1)
                {
                    Distance += GPSLocation.CalcDistance(
                        Locations[Locations.Count - 1], Locations[Locations.Count - 2]);
                    PaceActual = (stopwatch.Elapsed.TotalSeconds / 60) / Distance;
                }
                _nextGPSReading += _gpsIntervalInSec;
            }
        }

        public async Task GetElapsedTimeAsync()
        {
            DispElapsedTime = ($"{stopwatch.Elapsed.Minutes.ToString("0")}:{stopwatch.Elapsed.Seconds.ToString("00")}");
            await Task.Delay(1000);
        }

        //-----------------------------------------------------------------
        //speak text to runner based on actual pace every 4 min (240 sec)
        public async Task SpeakNowRunnerPromptAsync()
        {
            if ((stopwatch.Elapsed.TotalSeconds / _nextTextToSpeech) >= 1.0)
            {
                //convert PaceGoal to seconds
                string[] result = (PaceGoal.Split(':'));
                var paceGoalInSecs = (Convert.ToInt32(result[0]) * 60) + Convert.ToInt32(result[1]);

                if ((paceGoalInSecs >= (PaceActual - 5)) && (paceGoalInSecs <= (PaceActual + 5)))
                {
                    await TextToSpeech.SpeakAsync("Perfect");
                }
                else if (paceGoalInSecs > (PaceActual + 5))
                {
                    await TextToSpeech.SpeakAsync("Slow down");
                }
                else
                {
                    await TextToSpeech.SpeakAsync("Speed up");
                }
                
                _nextTextToSpeech += _speechInterval;  //add 4 min to time
            }
        }

        public void ResetValues()
        {
            stopwatch.Reset();

            _nextGPSReading = 0;
            _nextTextToSpeech = 0;
            _distance = 0;
            _paceActual = 0;
            Distance = 0;
            PaceActual = 0;
            DispDistance = string.Empty;
            DispElapsedTime = string.Empty;
            DispPaceActual = string.Empty;

            StartTime = DateTime.Now;
            Locations.Clear();
            IsRestart = false;
            IsDone = false;
        }
    }
}
