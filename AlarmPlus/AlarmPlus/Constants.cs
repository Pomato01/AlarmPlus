using System;
namespace AlarmPlus
{
	public static class Constants
	{

        public const string DatabaseFilename = "AlarmPlusDB.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public enum RepeatFrequency
        {
            Daily,
            Weekly,
            Monthly,
            Yearly
            
        }
        public static readonly IDictionary<string, int> daysName2Num = new Dictionary<string, int>()
        {
            ["Sun"] = 0,
            ["Mon"] = 1,
            ["Tue"] = 2,
            ["Wed"] = 3,
            ["Thu"] = 4,
            ["Fri"] = 5,
            ["Sat"] = 6
        };
        public static readonly IDictionary<string, string> daysNum2Name = new Dictionary<string, string>()
        {
            ["0"] = "Sun",
            ["1"] = "Mon",
            ["2"] = "Tue",
            ["3"] = "Wed",
            ["4"] = "Thu",
            ["5"] = "Fri",
            ["6"] = "Sat"
        };
        public static readonly IDictionary<string, int> monthName2Num = new Dictionary<string, int>()
        {
            ["Jan"] = 1,
            ["Feb"] = 2,
            ["Mar"] = 3,
            ["Apr"] = 4,
            ["May"] = 5,
            ["Jun"] = 6,
            ["Jul"] = 7,
            ["Aug"] = 8,
            ["Sep"] = 9,
            ["Oct"] = 10,
            ["Nov"] = 11,
            ["Dec"] = 12
        };
        public static readonly IDictionary<string, string> monthNum2Name = new Dictionary<string, string>()
        {
            ["1"] = "Jan",
            ["2"] = "Feb",
            ["3"] = "Mar",
            ["4"] = "Apr",
            ["5"] = "May",
            ["6"] = "Jun",
            ["7"] = "Jul",
            ["8"] = "Aug",
            ["9"] = "Sep",
            ["10"] = "Oct",
            ["11"] = "Nov",
            ["12"] = "Dec"
        };
    }
}

