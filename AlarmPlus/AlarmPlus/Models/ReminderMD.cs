using System;
using SQLite;

namespace AlarmPlus.Models
{
	public class ReminderMD
	{
		public string Title { get; set; }
		public string Details { get; set; }
		public DateTime Time { get; set; }
		public bool IsActive { get; set; }
		public bool IsCompleted { get; set; }
		public List<AlarmMD> Alarms { get; set; }

	}
    public class Reminder
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int UserId { get; set; }
        public int AlarmId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        public DateTime ReminderDate { get; set; } = DateTime.Now;
        public TimeSpan ReminderTime { get; set; } = DateTime.Now.AddHours(1).TimeOfDay;
       //[Ignore]
        public DateTime ReminderDateTime {get;set;
           
               
        }
        //public TimeOnly AlarmTime { get; set; } = TimeOnly.FromDateTime(DateTime.Now);
        //public DateOnly AlarmDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string Time { get; set; } = DateTime.Now.ToLongDateString() + "    " + DateTime.Now.ToShortTimeString();  //DateTime.Now.ToShortTimeString();
        public DateTime TimeString { get; set; } = DateTime.Now; //.ToShortTimeString();
        //public string DateString { get; set; } = DateTime.Now.ToShortDateString();
        public DateTime TimeCreated { get; set; }
        //public string StartDate { get; set; } = DateTime.Now.ToShortDateString();
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        public bool IsActive { get; set; }
        public string Frequency { get; set; }
        public bool IsRepeated { get; set; }
        /*[Ignore]
        public List<string>ReminderWeekDaysList
        {
            get
            {
                return ReminderWeekDays.Split('-').ToList();
            }

            set { ReminderWeekDaysList = value; }//0-1-2-3-4-5-6
        }*/
        public string ReminderWeekDays { get; set; } = "";//0-1-2-3-4-5-6
        public string ReminderMonths { get; set; } = "";//1-2-3-4-5-6-7-8-9-10-11-12
        public string SoundFile { get; set; }
        public string Phone { get; set; }
        public string Emails { get; set; }
        public string Extras { get; set; }
        public DateTime ActivatedAt { get; set; }


    }
}

