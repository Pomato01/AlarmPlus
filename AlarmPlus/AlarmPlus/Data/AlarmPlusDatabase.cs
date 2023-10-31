using SQLite;
using AlarmPlus.Models;
namespace AlarmPlus.Data
{
	public class AlarmPlusDatabase
	{
		SQLiteAsyncConnection Database;
		public AlarmPlusDatabase()
		{
		}
		async Task Init()
		{
            if (Database is not null)
				return;
            try
            {
                Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
                var result = await Database.CreateTableAsync<Alarm>();
                result = await Database.CreateTableAsync<Reminder>();
            }catch(Exception ex)
            {
                string err = ex.Message;
                throw ;
            }
        }
        private bool IsExpired(Models.Reminder r)
        {
            //if (r.EndTime is not null && r.EndTime < r.Time) return true;
            
            if (r.EndDate.CompareTo(r.TimeString) <0 ) return true;
            return false;
        }
        private Models.Reminder AdjustRepeatedTime(Models.Reminder r)
        {
            if(r.IsRepeated)
            {
                if(r.TimeString > DateTime.Now)
                {
                    r.TimeString = r.TimeString.AddDays(Math.Abs( DateTime.Now.DayOfWeek - r.TimeString.DayOfWeek));
                }
            }
            return r;
        }
        public async Task<List<Reminder>> GetRemindersAsync(int userid, int num,bool active,string sortOrder="NewestFirst")
		{
			await Init();
            if (active)
            {
                if (sortOrder != "NewestFirst")
                    return await Database.Table<Reminder>()
                    .Where(r =>
                    r.UserId == userid
                    //&& r.TimeString > DateTime.Now
                    //&& r.ReminderDate.Add(r.ReminderTime.TimeOfDay) >= DateTime.Now
                    //&& !IsExpired(r)
                    //&& r.IsRepeated
                    )
                        .OrderBy(o => o.Time)
                        .Take(num).ToListAsync();
                else
                {
                    //var y = DateTime.Now.ToLocalTime();
                    //return await Database.Table<Reminder>().Where(r => r.UserId == userid && r.Time > DateTime.Now).OrderByDescending(o => o.Time).Take(num).ToListAsync();
                    return await Database.Table<Reminder>()
                        .Where(r =>
                         r.UserId == userid
                         //&& r.ReminderWeekDays.Length == 0
                         //&& r.ReminderMonths.Length ==0
                         && r.EndDate > DateTime.Now
                         //&& r.ReminderTime >= DateTime.Now.TimeOfDay
                         //&& r.ReminderDate >= DateTime.Now
                         //&& r.ReminderTime >= DateTime.Now
                         //&& !IsExpired(r)
                         )

                        .OrderByDescending(o => o.TimeString)
                        .Take(num).ToListAsync();
                }
            }
            else {
                if (sortOrder != "NewestFirst")
                    return await Database.Table<Reminder>().Where(r => r.UserId == userid).OrderBy(o=>o.Time).Take(num).ToListAsync();
                else
                    return await Database.Table<Reminder>().Where(r => r.UserId == userid).OrderByDescending(o=>o.Time).Take(num).ToListAsync();
            }


        }
        
        public async Task<List<Reminder>> GetRemindersAsync(DateTime from, DateTime to, int num = 5,bool active = true,string sortOrder="NewsestFirst")
        {
            await Init();
            if (active)
            {
                if(sortOrder == "NewestFirst")
                    return await Database.Table<Reminder>()
                        .Where(
                        r => r.TimeString >= from
                        && r.TimeString <= to
                        && r.TimeString < DateTime.Now)
                        .OrderBy(o => o.Time)
                        .Take(num).ToListAsync();
                else
                    return await Database.Table<Reminder>()
                        .Where(r => r.TimeString >= from
                        && r.TimeString <= to
                        && r.TimeString < DateTime.Now)
                        .OrderByDescending(o => o.Time)
                        .Take(num).ToListAsync();
            }
            else {
                if (sortOrder == "NewestFirst")
                    return await Database.Table<Reminder>()
                        .Where(r => r.TimeString >= from
                        && r.TimeString<= to)
                        .OrderBy(o => o.Time)
                        .Take(num).ToListAsync();
                else
                    return await Database.Table<Reminder>()
                        .Where(r => r.TimeString >= from
                        && r.TimeString <= to)
                        .OrderByDescending(o => o.Time)
                        .Take(num).ToListAsync();
            }
        }
        public async Task<Reminder> GetReminder(int remId)
		{
			await Init();
			return await Database.Table<Reminder>().Where(r => r.ID == remId).FirstOrDefaultAsync();
		}
        
        public async Task<int> SaveReminderAsync(Reminder reminder)
        {
            await Init();
            if (reminder.ID != 0)
            {
                return await Database.UpdateAsync(reminder);
            }
            else
            {
                return await Database.InsertAsync(reminder);
            }
        }

        public async Task<int> DeleteItemAsync(Reminder reminder)
        {
            await Init();
            return await Database.DeleteAsync(reminder);
        }
    }
}

