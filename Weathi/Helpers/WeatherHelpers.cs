using Weathi.Services;

namespace Weathi.Helpers
{
    public static class WeatherHelpers
    {
        public static string CalculateDirection(int windDirection, Android.Content.Res.Resources res)
        {
            var result = "N/A";

            if (windDirection == 0)
            {
                result = res.GetString(Resource.String.NorthLabel);
            }
            if (windDirection > 0 && windDirection < 90)
            {
                result = res.GetString(Resource.String.NorthEastLabel);
            }
            if (windDirection == 90)
            {
                result = res.GetString(Resource.String.EastLabel);
            }
            if (windDirection > 90 && windDirection < 180)
            {
                result = res.GetString(Resource.String.SouthEastLabel);
            }
            if (windDirection == 180)
            {
                result = res.GetString(Resource.String.SouthLabel);
            }
            if (windDirection > 180 && windDirection < 270)
            {
                result = res.GetString(Resource.String.SouthWestLabel);
            }
            if (windDirection == 270)
            {
                result = res.GetString(Resource.String.WestLabel);
            }
            if (windDirection > 270 && windDirection < 360)
            {
                result = res.GetString(Resource.String.NorthWestLabel);
            }

            return result;
        }

        public static bool CheckIsFirstTime()
        {
            var dbhelper = new SqliteService();

            dbhelper.CreateDatabase();
            return dbhelper.FindNumberRecordWeathers() == 0 ? true : false ;
        }

        
    }
}