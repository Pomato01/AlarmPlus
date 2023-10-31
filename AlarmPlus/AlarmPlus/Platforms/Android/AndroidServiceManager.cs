namespace AlarmPlus.Platforms.Android
{
    public static class AndroidServiceManager
	{
        public static MainActivity MainActivity { get; set; }
        public static bool IsRunning { get; set; }

        public static void StartMyService(string srs)
        {

            if (MainActivity == null) return;
            MainActivity.StartService(srs);
        }

        public static void StopMyService()
        {
            if (MainActivity == null) return;
            MainActivity.StopService();
            IsRunning = false;
        }
    }
}

