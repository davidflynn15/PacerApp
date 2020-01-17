using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PacerApp
{
    public class GPSCoordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double ElapsedTimeInSec { get; set; }
        public string message { get; set; }

        //not using constructor at this time
        // public GPSCoordinates(double latitude, double longitude) { }
    }

    public class GPSLocation
    {
        public static async Task<GPSCoordinates> GetCoordinatesAsync(double elapsedTime)
        {
            GPSCoordinates coordinates = new GPSCoordinates();
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var myLocation = await Geolocation.GetLocationAsync(request);
                //var location = await Task.Run( ()=> Geolocation.GetLastKnownLocationAsync());
                if (myLocation != null)
                {
                    Console.WriteLine($"Latitude: {myLocation.Latitude}, Longitude: {myLocation.Longitude}, Altitude: {myLocation.Altitude}");
                    coordinates.Latitude = myLocation.Latitude;
                    coordinates.Longitude = myLocation.Longitude;
                    coordinates.ElapsedTimeInSec = elapsedTime;
                }

                return coordinates;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                throw new NotImplementedException();
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                throw new NotImplementedException();
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                // Unable to get location
                coordinates.Latitude = 0;
                coordinates.Longitude = 0;
                coordinates.ElapsedTimeInSec = elapsedTime;
                coordinates.message = "Error - unable to get location";

                return coordinates;
            }
        }

        //calcuate distance between the last two coordinates
        public static double CalcDistance(GPSCoordinates previous, GPSCoordinates current)
        {
            Location previousLoc = new Location(previous.Latitude, previous.Longitude);
            Location currentLoc = new Location(current.Latitude, current.Longitude);

            return Location.CalculateDistance(previousLoc, currentLoc, DistanceUnits.Miles);
        }

    }
}
