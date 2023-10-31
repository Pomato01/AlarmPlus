using System.Collections.Generic;
using AlarmPlus.Data;

namespace AlarmPlus.Views;

[QueryProperty("reminder", "alarmkey")]
public partial class Alarm : ContentPage
{
    //IDispatcherTimer timer;
    AlarmPlusDatabase Database;
    public Models.Reminder reminder
    {
        get => BindingContext as Models.Reminder;

        set => BindingContext = value;
    }

    public Alarm(AlarmPlusDatabase db)
    {
        InitializeComponent();
        Database = db;

        /*timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(10000); // 10 seconds
        timer.Tick += Timer_Tick;
        timer.Start();
        */
    }


    void ContentPage_Unloaded(System.Object sender, System.EventArgs e)
    {
        //timer.Stop();
        mediaelement.Handler?.DisconnectHandler();
        CancelSpeech();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        CheckReminders(reminder);
    }
    private string FindDayOfWeek()
    {
        //DateTime.Today.ToLongDateString() = "Tuesday, August 15, 2023"
        return DateTime.Today.ToLongDateString().Split(",", StringSplitOptions.TrimEntries)[0];
    }
    private int FindDayOfWeekNum()
    {
        //DateTime.Today.ToLongDateString() = "Tuesday, August 15, 2023"
        var dayName = DateTime.Today.ToLongDateString().Split(",", StringSplitOptions.TrimEntries)[0];
        IDictionary<string, int> days = new Dictionary<string, int>()
        {
            ["Sunday"] = 0,
            ["Monday"] = 1,
            ["Tueday"] = 2,
            ["Wednesday"] = 3,
            ["Thursday"] = 4,
            ["Friday"] = 5,
            ["Saturday"] = 6
        };
        return days[dayName];

    }
    private async void CheckReminders(Models.Reminder r)
    {
        this.Title = "";
        //this.Title = "Alarm!";
        this.Title = reminder.Extras?.Remove(0, 2); // removing preceeding comma.
        // modify the reminder date and time if it is a repeated reminder
        if (reminder.ReminderMonths.Length > 0)
        {
            // find the next month
            var remmonths = reminder.ReminderMonths.Split(" ", StringSplitOptions.RemoveEmptyEntries); // Jan Feb Mar
            int currM = DateTime.Today.Month;
            int mMax = Constants.monthName2Num[remmonths[remmonths.Length - 1]];// last element in the sorted array
            int mMin = Constants.monthName2Num[remmonths[0]]; // first element in the sorted array

            // finding the next day
            for (int i = 0; i < remmonths.Length; i++)
            {
                if (currM == mMax)
                {
                    reminder.ReminderDate = DateTime.Today.AddMonths(12 - mMax + mMin);
                    break;
                }
                if (currM == Constants.monthName2Num[remmonths[i]])
                {
                    int diff = Constants.monthName2Num[remmonths[i + 1]] - Constants.monthName2Num[remmonths[i]];
                    reminder.ReminderDate = reminder.ReminderDate.AddMonths(diff);
                }
            }


            var ms = reminder.ReminderMonths.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var msMonths = "";
            foreach (string m in ms)
            {
                msMonths = msMonths + "," + Constants.monthName2Num[m];
            }
            reminder.ReminderMonths = msMonths;
            await Database.SaveReminderAsync(reminder);
        }
        else if (reminder.ReminderWeekDays.Length > 0)
        {
            var remdays = reminder.ReminderWeekDays.Split(" ",StringSplitOptions.RemoveEmptyEntries); // Sun Mon Tue
            int currDay = Constants.daysName2Num[FindDayOfWeek().Substring(0,3)];

            var t = remdays[remdays.Length - 1];
            int dMax = Constants.daysName2Num[t];// last element in the sorted array
            int dMin = Constants.daysName2Num[remdays[0]]; // first element in the sorted array

            // finding the next day
            for(int i = 0; i<remdays.Length;i++)
            {
                if (currDay == dMax)
                {
                    reminder.ReminderDate = DateTime.Today.AddDays(7- dMax + dMin);
                    break;
                }
                if (currDay == Constants.daysName2Num[remdays[i]])
                {
                    int diff = Constants.daysName2Num[remdays[i+1]] - Constants.daysName2Num[remdays[i]];
                    reminder.ReminderDate = reminder.ReminderDate.AddDays(diff);

                }
            }

            var ds = reminder.ReminderWeekDays.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var dsDays = "";
            foreach (string d in ds)
            {
                dsDays = dsDays + "," + Constants.daysName2Num[d];
            }
            reminder.ReminderWeekDays = dsDays;
            reminder.ActivatedAt = reminder.ReminderDate.Add(reminder.ReminderTime);
            await Database.SaveReminderAsync(reminder);
        }

        if (string.IsNullOrWhiteSpace(reminder.SoundFile))
        {
            var utl = new Utilities.Sound(mediaelement);
            utl.PlaySoud("Glass.aiff");

        }
        else
        {
            await SpeakNowDefaultSettingsAsync();
            //var utl = new Utilities.Sound(mediaelement);

            //utl.PlaySoud(reminder.SoundFile);
        }

        /* if (!string.IsNullOrWhiteSpace(reminder.Emails))
         {
             var emailUtl = new Utilities.clsEmail();
             await emailUtl.SendEmailAsync(reminder.Name, reminder.Desc, reminder.Emails.Split(','));
         }
         if (!string.IsNullOrWhiteSpace(reminder.Phone))
         {

             await new Utilities.clsSendSMS().SendSMSAsync(reminder.Phone,reminder.Desc);
         }
        */

    }

    CancellationTokenSource cts;        

    private async Task SpeakNowDefaultSettingsAsync()
    {
        cts = new CancellationTokenSource();
        SpeakMultiple(reminder.SoundFile);
        //cts.CancelAfter(20000);
        //await TextToSpeech.Default.SpeakAsync(reminder.SoundFile, cancelToken: cts.Token);
        //CancelSpeech();
        

        //await TextToSpeech.Default.SpeakAsync(reminder.SoundFile);
        //SpeakMultiple(reminder.SoundFile);
        // This method will block until utterance finishes.
    }
    bool isBusy = false;

    public void SpeakMultiple(string txt)
    {
        isBusy = true;
        var speechTasks = new List<Task>();
        for(int i = 0;i<=10;i++)
        {
            speechTasks.Add(TextToSpeech.Default.SpeakAsync(txt, cancelToken: cts.Token));
        }
        Task.WhenAll(speechTasks).ContinueWith((t) => { isBusy = false; }, TaskScheduler.FromCurrentSynchronizationContext());
        /*Task.WhenAll(

            
            TextToSpeech.Default.SpeakAsync(txt),
            TextToSpeech.Default.SpeakAsync(txt),
            TextToSpeech.Default.SpeakAsync(txt))
            .ContinueWith((t) => { isBusy = false; }, TaskScheduler.FromCurrentSynchronizationContext());*/
    }

    // Cancel speech if a cancellation token exists & hasn't been already requested.
    public void CancelSpeech()
    {
        if (cts?.IsCancellationRequested ?? true)
            return;

        cts.Cancel();
    }
    void ContentPage_Loaded(System.Object sender, System.EventArgs e)
    {

        CheckReminders(reminder);
        edtDescAlarm.Text = reminder.Desc;
        edtSound.Text = reminder.SoundFile;
        
    }

    void btnAlarmCancel_Clicked(System.Object sender, System.EventArgs e)
    {
        mediaelement.Handler?.DisconnectHandler();
        CancelSpeech();
    }
    async void SendEmail2(List<string> emails, string sub, string body)
    {
        try
        {
            if (Email.Default.IsComposeSupported)
            {
                EmailMessage msg = new EmailMessage
                {
                    To = emails,
                    Body = body,
                    Subject = sub,
                };
                await Email.Default.ComposeAsync(msg);
            }
            else
            {
                await Shell.Current.DisplayAlert("Email", "Email Compose not supported", "OK");
            }
        }
        catch (FeatureNotSupportedException fex)
        {
            await Shell.Current.DisplayAlert("Email", fex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Email", ex.Message, "OK");
        }
    }
    void btnEmailmaui_Clicked(System.Object sender, System.EventArgs e)
    {
        List<string> emails = new List<string>();
        if (reminder.Emails.Contains(","))
        {
            emails = reminder.Emails.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        }
        else
            emails = new List<string> { reminder.Emails?.Trim() };
        SendEmail2(emails,reminder.Name ,"[" + reminder.Desc + "] " + reminder.SoundFile);
        SendEmail();


    }
    async void SendEmail()
    {
        try
        {
            System.Net.Mail.SmtpClient mail = new System.Net.Mail.SmtpClient("smtp.oncloudeleven.com");
            mail.Port = 587;
            mail.UseDefaultCredentials = true; //  new System.Net.NetworkCredential()
            mail.EnableSsl = true;
            mail.Send("info@oncloudeleven.com", "abhinawsharma@gmail.com", "test smtp maui", "Hello World!MAUI!");
        }catch(Exception ex)
        {
            await Shell.Current.DisplayAlert("Email", ex.Message, "OK");
        }
    }
}
