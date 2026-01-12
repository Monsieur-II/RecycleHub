namespace RecycleHub.Utils;

public static class UtilityConstants
{
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371; // Radius of the Earth in kilometers

        var dLat = (lat2 - lat1) * Math.PI / 180;   // Degree to radians
        var dLon = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distance = earthRadiusKm * c; // Distance in kilometers

        return distance;
    } 
}
