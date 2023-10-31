using System.Collections.ObjectModel;
using AlarmPlus.Models;
using AlarmPlus.Data;
using System.Linq;
using Newtonsoft.Json;
//using Xamarin.Google.Crypto.Tink.Subtle;
//using static Android.Content.ClipData;

namespace AlarmPlus.Views;

public partial class Reminders : ContentPage
{
    AlarmPlus.Data.AlarmPlusDatabase database;
    public ObservableCollection<Models.Reminder> reminders { get; set; } = new();
    IDispatcherTimer timer;
    List<Models.Reminder> alarms = new List<Models.Reminder>();

    public Reminders(AlarmPlusDatabase db)
    {
        InitializeComponent();
        database = db;
        BindingContext = this; // this needs explaination. read abou it.

        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(10000);// 25 seconds
        timer.Tick += Timer_Tick;
        timer.Start();

        StartService(reminders.ToList());

    }

    private bool IsExpired(Models.Reminder r)
    {
        //if (r.EndTime is not null && r.EndTime < r.Time) return true;
        if (r.EndDate.CompareTo(r.TimeString) < 0) return true;
        return false;
    }

    // to be called from android platform
    public static List<Models.Reminder> CheckCurrentReminders(List<Models.Reminder> reminders, int seconds)
    {
        List<Models.Reminder> alarms = new List<Models.Reminder>();
        foreach (var r in reminders)
        {
            if (r.ReminderMonths.Length > 0 && r.ReminderMonths.Contains(Constants.monthNum2Name[DateTime.Today.Month.ToString()]))
            {
                if (r.ReminderTime.CompareTo(DateTime.Now.TimeOfDay) > 0 && r.ReminderTime - DateTime.Now.TimeOfDay <= new TimeSpan(0, 0, seconds))
                {
                    alarms.Add(r);
                    continue;
                }

            }
            else if (r.ReminderWeekDays.Length > 0 && r.ReminderWeekDays.Contains(FindDayOfWeek().Substring(0, 3)))
            {
                //if (r.ReminderTime.CompareTo(DateTime.Now.TimeOfDay) > 0 && r.ReminderTime - DateTime.Now.TimeOfDay <= new TimeSpan(0, 0, seconds))
                //{
                    alarms.Add(r);
                    continue;
                //}

            }

            if (r.ReminderDate < DateTime.Today) continue;
            if (Math.Abs((r.ReminderTime - DateTime.Now.TimeOfDay).TotalSeconds) > seconds) continue;
            //if reached here then it is time to sound the alarm
            if (r.ReminderDate == DateTime.Today)
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

    private async void CheckCurrentReminders(int seconds)
    {
        // if month if month != curr continue
        // if day if day !=today continue
        // if date != today continue ??
        // if time > 60 sec of No continue
        // Activate alarm

        //List<Models.Reminder> alarms = new List<Models.Reminder>();
        alarms.Clear();

        foreach(var r in reminders)
        {
            if (r.ReminderMonths.Length > 0 && r.ReminderMonths.Contains(Constants.monthNum2Name[DateTime.Today.Month.ToString()]))
            {
                if (r.ReminderTime.CompareTo(DateTime.Now.TimeOfDay) > 0 && r.ReminderTime - DateTime.Now.TimeOfDay <= new TimeSpan(0, 0, seconds))
                {
                    alarms.Add(r);
                    continue;
                }

            }
            else if (r.ReminderWeekDays.Length > 0 && r.ReminderWeekDays.Contains(FindDayOfWeek().Substring(0,3)))
            {
                if (r.ReminderTime.CompareTo(DateTime.Now.TimeOfDay) >0 && r.ReminderTime - DateTime.Now.TimeOfDay <= new TimeSpan(0,0,seconds))
                {
                    alarms.Add(r);
                    continue;
                }
               
            }

           
            if (r.ReminderDate < DateTime.Today) continue;
            if (Math.Abs((r.ReminderTime - DateTime.Now.TimeOfDay).TotalSeconds) > seconds) continue;
            //if reached here then it is time to sound the alarm
            if(r.ReminderDate.Date == DateTime.Today)
                alarms.Add(r);

        }
        if(alarms.Count > 0)
        {
            var alarmR = alarms[0];
            alarmR.Extras = "";
            foreach (var r in alarms)
            {
                alarmR.Extras = string.Concat(alarmR.Extras, " , ", r.Name);
            }
            if (DateTime.Compare(alarms[0].ActivatedAt, alarms[0].ReminderDate.Add(alarms[0].ReminderTime)) != 0)
            {
                var d1 = alarms[0].ActivatedAt;
                var d = alarms[0].ReminderDate.Add(alarms[0].ReminderTime);
            }

#if ANDROID
                //StartService(alarms);
                StartService(reminders.ToList());
            timer.Stop();

           
#else
        // check in the alarm has been activated. do not alarm again in that case.

        if(DateTime.Compare(alarms[0].ActivatedAt, alarms[0].ReminderDate.Add(alarms[0].ReminderTime)) !=0)
        {
        await Shell.Current.GoToAsync(nameof(Alarm), true, new Dictionary<string, object>
             {
                ["alarmkey"] = alarms[0]
            });
        }
#endif
        }


    }
   
    private void Timer_Tick(object sender, EventArgs e)
    {

        CheckCurrentReminders(30);
        

    }
    private static string FindDayOfWeek()
    {
        //DateTime.Today.ToLongDateString() = "Tuesday, August 15, 2023"
        return DateTime.Today.ToLongDateString().Split(",", StringSplitOptions.TrimEntries)[0];
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        try
        {
            base.OnNavigatedTo(args);

            var items = await database.GetRemindersAsync(1, 100, true); // making user id = 1. this is for one user so we do not really need user id.
                                                                        // we also have to restirct the reminders to the latest 5.
                                                                        //format days and months

            MainThread.BeginInvokeOnMainThread(() =>
            {
                reminders.Clear();

                foreach (var item in items)
                {

                    if (item.EndDate > DateTime.Today)
                    {
                        // if not repeated
                        if (item.ReminderMonths == "" && item.ReminderWeekDays == "")
                        {

                            if (item.ReminderDate.Date.Add(item.ReminderTime) < DateTime.Now) continue;

                            reminders.Add(item);
                        }
                        else
                        {
                            // format the days
                            var ds = item.ReminderWeekDays.Split(",", StringSplitOptions.RemoveEmptyEntries);
                            var dsDays = "";
                            foreach (string d in ds)
                            {
                                dsDays = dsDays + " " + Constants.daysNum2Name[d];
                            }
                            item.ReminderWeekDays = dsDays;

                            var ms = item.ReminderMonths.Split(",", StringSplitOptions.RemoveEmptyEntries);
                            var msMonths = "";
                            foreach (string m in ms)
                            {
                                msMonths = msMonths + " " + Constants.monthNum2Name[m];
                            }
                            item.ReminderMonths = msMonths;
                            reminders.Add(item);
                        }
                    }



                }

            });
            StopService();
            StartService(reminders.ToList());
        }catch(Exception ex)
        {
            string err = ex.Message;
        }
    }

    async void RemsCLV_SelectionChanged(System.Object sender, Microsoft.Maui.Controls.SelectionChangedEventArgs e)
    {
        //var device = DeviceInfo.Current;


        if (e.CurrentSelection.FirstOrDefault() is not Models.Reminder item)
            return;

        await Shell.Current.GoToAsync(nameof(Reminder), true, new Dictionary<string, object>
        {
            ["reminderkey"] = item
        });
    }

    async void btnAddReminder_Clicked(System.Object sender, System.EventArgs e)
    {
        //Testreminders();
        //SendEmail();

        await Shell.Current.GoToAsync(nameof(AlarmPlus.Views.Reminder), true, new Dictionary<string, object>
        {
            ["reminderkey"] = new Models.Reminder()
        });


    }

    async void btnShowAllReminders_Clicked(System.Object sender, System.EventArgs e)
    {
        if (btnShowAllReminders.Text == "Show All Reminders")
        { 
        var items = await database.GetRemindersAsync(1, 100, false); // making user id = 1. this is for one user so we do not really need user id.
        // we also have to restirct the reminders to the latest 5.
        MainThread.BeginInvokeOnMainThread(() =>
        {
            reminders.Clear();
            foreach (var item in items)
            {
                // format the days
                var ds = item.ReminderWeekDays.Split(",", StringSplitOptions.RemoveEmptyEntries);
                var dsDays = "";
                foreach (string d in ds)
                {
                    dsDays = dsDays + " " + Constants.daysNum2Name[d];
                }
                item.ReminderWeekDays = dsDays;

                var ms = item.ReminderMonths.Split(",", StringSplitOptions.RemoveEmptyEntries);
                var msMonths = "";
                foreach (string m in ms)
                {
                    msMonths = msMonths + " " + Constants.monthNum2Name[m];
                }
                item.ReminderMonths = msMonths;
                reminders.Add(item);
            }

            btnShowAllReminders.Text = "Show Active Reminders";
        });
    }
        else
        {
            var items = await database.GetRemindersAsync(1, 100, true); // making user id = 1. this is for one user so we do not really need user id.
                                                                       // we also have to restirct the reminders to the latest 5.
                                                                       //format days and months

            MainThread.BeginInvokeOnMainThread(() =>
            {
                reminders.Clear();

                foreach (var item in items)
                {

                    if (item.EndDate > DateTime.Today)
                    {
                        // if not repeated
                        if (item.ReminderMonths == "" && item.ReminderWeekDays == "")
                        {

                            if (item.ReminderDate.Date.Add(item.ReminderTime) < DateTime.Now) continue;

                            reminders.Add(item);
                        }
                        else
                        {
                            // format the days
                            var ds = item.ReminderWeekDays.Split(",", StringSplitOptions.RemoveEmptyEntries);
                            var dsDays = "";
                            foreach (string d in ds)
                            {
                                dsDays = dsDays + " " + Constants.daysNum2Name[d];
                            }
                            item.ReminderWeekDays = dsDays;

                            var ms = item.ReminderMonths.Split(",", StringSplitOptions.RemoveEmptyEntries);
                            var msMonths = "";
                            foreach (string m in ms)
                            {
                                msMonths = msMonths + " " + Constants.monthNum2Name[m];
                            }
                            item.ReminderMonths = msMonths;
                            reminders.Add(item);
                        }
                    }



                }
                btnShowAllReminders.Text = "Show All Reminders";
            });
        }
    }


   /* void pageRems_Appearing(System.Object sender, System.EventArgs e)
    {

        if (!timer.IsRunning) timer.Start();
    }*/

    void SendEmail()
    {
        System.Net.Mail.SmtpClient mail = new System.Net.Mail.SmtpClient("smtp.oncloudeleven.com");
        mail.Port = 587;
        mail.UseDefaultCredentials = true; //  new System.Net.NetworkCredential()
        mail.EnableSsl = true;
        mail.Send("info@oncloudeleven.com", "abhinawsharma@gmail.com", "test smtp maui", "Hello World!MAUI!");
    }
    async void SendEmail2()
    {
        try
        {
            EmailMessage msg = new EmailMessage
            {
                To = new List<string> { "abhinawsharma@gmail.com" },
                Body = "Hello Email from MAUI Alarm Plus",
                Subject = "Email subject here - Alarm Plus"
            };
            await Microsoft.Maui.ApplicationModel.Communication.Email.Default.ComposeAsync(msg);
        }
        catch(FeatureNotSupportedException fex)
        {
            await Shell.Current.DisplayAlert("Email",fex.Message,"OK");
        }
        catch(Exception ex)
        {
            await Shell.Current.DisplayAlert("Email", ex.Message, "OK");
        }
    }

    //*******  ANDROID SERVICE CODE *************
    private void StartService(List<Models.Reminder> rms)
    {
#if ANDROID
        //string Message;
        var sr = Serizlize(rms);
        if (!Platforms.Android.AndroidServiceManager.IsRunning)
        {
            Platforms.Android.AndroidServiceManager.StartMyService(sr);
        }
#endif
    }

    private void StopService()
    {
#if ANDROID
        Platforms.Android.AndroidServiceManager.StopMyService();
#endif
    }
    public string Serizlize(Models.Reminder r)
    {
        return JsonConvert.SerializeObject(r);
    }
    public string Serizlize(List<Models.Reminder> r)
    {
        return JsonConvert.SerializeObject(r);
    }
    public Models.Reminder Deserialize(string sr)
    {
        return JsonConvert.DeserializeObject<Models.Reminder>(sr);
    }


    async void tbAbout_Clicked(System.Object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(About));
    }

    void tbImport_Clicked(System.Object sender, System.EventArgs e)
    {
        if (string.IsNullOrEmpty(edtImportR.Text))
        {
            frmEdtImport.IsVisible = true;
            //edtImportR.IsVisible = true;
        }

        //await Shell.Current.GoToAsync(nameof(Reminder));

    }

    async void edtImportR_Unfocused(System.Object sender, Microsoft.Maui.Controls.FocusEventArgs e)
    {
        
        /*if (!string.IsNullOrEmpty(edtImportR.Text))
        {
            Models.Reminder r = Deserialize(edtImportR.Text);
            r.ID = 0; // this is needed to add a new reminder.
            await database.SaveReminderAsync(r);
            edtImportR.IsVisible = false;
        }*/
    }

    async void edtImportR_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(edtImportR.Text))
        {
            Models.Reminder r = Deserialize(edtImportR.Text);
            r.ID = 0; // this is needed to add a new reminder.
            await database.SaveReminderAsync(r);
            //edtImportR.IsVisible = false;
            frmEdtImport.IsVisible = false;
        }
    }
}
