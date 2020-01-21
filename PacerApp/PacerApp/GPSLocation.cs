using PacerAppUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PacerApp
{
    public class GPSLocation
    {
        public static async Task<GPSCoordinate> GetCoordinatesAsync(double elapsedTime)
        {
            GPSCoordinate coordinate = new GPSCoordinate();
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var myLocation = await Geolocation.GetLocationAsync(request);
                //var location = await Task.Run( ()=> Geolocation.GetLastKnownLocationAsync());
                if (myLocation != null)
                {
                    Console.WriteLine($"Latitude: {myLocation.Latitude}, Longitude: {myLocation.Longitude}, Altitude: {myLocation.Altitude}");
                    coordinate.Latitude = myLocation.Latitude;
                    coordinate.Longitude = myLocation.Longitude;
                    coordinate.ElapsedTimeInSec = elapsedTime;
                }

                return coordinate;
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
                coordinate.Latitude = 0;
                coordinate.Longitude = 0;
                coordinate.ElapsedTimeInSec = elapsedTime;
                coordinate.message = "Error - unable to get location";

                return coordinate;
            }
        }

        //calcuate distance between the last two coordinates
        public static double CalcDistance(GPSCoordinate previous, GPSCoordinate current)
        {
            Location previousLoc = new Location(previous.Latitude, previous.Longitude);
            Location currentLoc = new Location(current.Latitude, current.Longitude);

            return Location.CalculateDistance(previousLoc, currentLoc, DistanceUnits.Miles);
        }

    }
}
