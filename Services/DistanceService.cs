// Services/DistanceService.cs
namespace TransportApi.Services
{
    public static class DistanceService
    {
        /// <summary>
        /// Haversine formula — returns distance in meters between two lat/lng points
        /// </summary>
        public static double GetDistanceMeters(
            double lat1, double lon1,
            double lat2, double lon2)
        {
            const double R = 6371000; // Earth radius in meters
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRad(double deg) => deg * Math.PI / 180;
    }
}
