using AlarmPlus.Models;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using static Android.Renderscripts.ScriptGroup;

namespace AlarmPlus.Platforms.Android
{
    [Service]
    public class MyBackgroundService : Service
    {
        Timer timer = null;
        int myId = Math.Abs( (new object()).GetHashCode());
        int BadgeNumber = 0;
        int counter = 0;
        int counter2 = 0;
        Reminder m_r;
        List<Reminder> m_rs;
        List<Reminder> m_alarms;
        public MyBackgroundService()
        {
        }

        private static string FindDayOfWeek()
        {
            return DateTime.Today.ToLongDateString().Split(",", StringSplitOptions.TrimEntries)[0];
        }
    

        public  List<Reminder> CheckCurrentReminders(List<Reminder> reminders, int seconds)
        {
            if (m_rs == null) return null;
            List<Models.Reminder> alarms = new List<Models.Reminder>();
            foreach (var r in m_rs)
            {
                if (r.ReminderMonths.Length > 0 && r.ReminderMonths.Contains(Constants.monthNum2Name[DateTime.Today.Month.ToString()]))
                {
                    var t = DateTime.Now.TimeOfDay - r.ReminderTime;
                    if (DateTime.Now.TimeOfDay.CompareTo(r.ReminderTime) >= 0 && t <= new TimeSpan(0, 0, 60))
                    {
                        alarms.Add(r);
                        continue;
                    }

                }
                else if (r.ReminderWeekDays.Length > 0 && r.ReminderWeekDays.Contains(FindDayOfWeek().Substring(0, 3)))
                {
                    var t = DateTime.Now.TimeOfDay - r.ReminderTime;
                    if (DateTime.Now.TimeOfDay.CompareTo(r.ReminderTime) >= 0 && t <= new TimeSpan(0, 0, 60))
                    {
                        alarms.Add(r);
                        continue;
                    }

                }

                if (r.ReminderDate < DateTime.Today) continue;
                var t1 = DateTime.Now.TimeOfDay - r.ReminderTime;
                if (DateTime.Now.TimeOfDay.CompareTo(r.ReminderTime) >= 0 && t1 <= new TimeSpan(0, 0, 60))
                    alarms.Add(r);
            }
            if (alarms.Count > 0)
            {
                var alarmR = alarms[0];
                alarmR.Extras = "";
                foreach (var r in alarms)
                {
                    alarmR.Extras = string.Concat(alarmR.Extras, " , ", r.Name);
                }
            }

            return alarms;
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            var input = intent.GetStringExtra("inputExtra");
            //var rems =  (List<Reminder>)  intent.GetSerializableExtra("bundlersSer");
            var srs = intent.GetStringExtra("remindersSZ");
            m_rs = DeserializeList(srs);
           
            var notification = new NotificationCompat.Builder
                (this, MainApplication.ChannelId)
                .SetContentText(input)
                .SetSmallIcon(Resource.Drawable.appIcon);
            //.SetContentIntent(pendingIntent);

            //notification.SetAutoCancel(false);
            //notification.SetOngoing(true);


            timer = new Timer(Timer_Elapsed, notification, 0, 10000);

            return StartCommandResult.Sticky;
        }

        string CombineMultipleDesc(List<Reminder> lst)
        {
            string desc="";
            foreach(var r in lst)
            {
                desc += r.Desc;

            }
            return desc;
        }
        string CombineMultipleSound(List<Reminder> lst)
        {
            string desc = "";
            string header = "";
            for (int i=0; i<lst.Count;i++)
            {
                if (i == 0) header = "First Reminder";
                if (i == 1) header = "Second Reminder";
                desc += header+  lst[i].SoundFile;

            }
            return desc;
        }
        void Timer_Elapsed(object state)
        {
            m_alarms = CheckCurrentReminders(m_rs, 20); // Views.Reminders.CheckCurrentReminders(m_rs, 30);
            var notification = (NotificationCompat.Builder)state;
            notification.SetContentTitle("Alarm Plus");

            if (counter == 0)
            {
                StartForeground(myId, notification.Build());
                counter++;
            }

            if (m_alarms.Count > 0)
            {
                AndroidServiceManager.IsRunning = true;
                var notificationIntent = new Intent(this, typeof(AndroidAlarm));
                //notificationIntent.Extras?.Clear();
                notification.SetContentTitle(m_alarms[0].Extras);

                //Bundle bndl = new Bundle();
                //bndl.PutStringArray("exAlarmVals", new string[] { "one", "two" });
                //notificationIntent.PutExtra("bndl", bndl);
                //notificationIntent.PutExtras(bndl);

                notificationIntent.PutExtra("exRN", m_alarms[0].Name);
                notificationIntent.PutExtra("exRT", m_alarms[0].ReminderTime.ToString());
                if(m_alarms.Count > 1)
                    notificationIntent.PutExtra("exRSound", CombineMultipleSound(m_alarms));
                else
                    notificationIntent.PutExtra("exRSound", m_alarms[0].SoundFile);
                notificationIntent.PutExtra("exRDesc", m_alarms[0].Desc);
                notificationIntent.PutExtra("exEmails", m_alarms[0].Emails);
                //notificationIntent.ReplaceExtras((Bundle)null);

                //notificationIntent.PutExtra("exRSound", "ha ha ha");
                var pendingIntent = PendingIntent.GetActivity(this, new Random().Next(1,10000), notificationIntent,
                PendingIntentFlags.Immutable);
               
                notification.SetContentIntent(pendingIntent);
                m_alarms.Clear();
                //m_rs.Clear();
                BadgeNumber++;
                notification.SetNumber(BadgeNumber);

                //notification.SetAutoCancel(true);
                notification.SetOnlyAlertOnce(true);

                try
                {
                        StartForeground(myId, notification.Build());
                }
                catch(ForegroundServiceStartNotAllowedException notAllowedEx)
                {
                    string err = notAllowedEx.Message;
                }
                catch ( Exception ex)
                {
                    string err = ex.Message;

                }

            }

        }
        public Reminder Deserialize(string sr)
        {
            if (string.IsNullOrEmpty(sr)) return null;
            return JsonConvert.DeserializeObject<Reminder>(sr);
        }
        public List<Reminder> DeserializeList(string sr)
        {
            if (string.IsNullOrEmpty(sr)) return null;
            return JsonConvert.DeserializeObject<List<Reminder>>(sr);
        }
        public string Serizlize(Reminder r)
        {
            return JsonConvert.SerializeObject(r);
        }
        public string Serizlize(List<Reminder> r)
        {
            return JsonConvert.SerializeObject(r);
        }
    }
}

