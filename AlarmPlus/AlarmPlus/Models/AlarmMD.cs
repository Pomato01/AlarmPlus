using System;
using SQLite;
namespace AlarmPlus.Models
{
    public class Alarm
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public DateTime TimeCreated { get; set; }

    }
    
	public abstract class AlarmMD
	{
		// "Sound", "Email","Text"
		public string Name { get; set; } = "Sound";
		public DateTime Time { get; set; }

		public abstract void SetAlarm(string name, DateTime time);
	}

	public class SoundAlarm:AlarmMD
	{
        public string SoundType { get; set; }
        public override void SetAlarm(string name, DateTime time)
        {
            Name = name;
            Time = time;
            
        }
    }

	public class TextAlarm:AlarmMD
	{
        public string phone { get; set; }
        public override void SetAlarm(string name, DateTime time)
        {
            Name = name;
            Time = time;
        }
    }

	public class EmailAlarm:AlarmMD
	{
        public string email { get; set; }
        public override void SetAlarm(string name, DateTime time)
        {
            Name = name;
            Time = time;
        }

    }


}

