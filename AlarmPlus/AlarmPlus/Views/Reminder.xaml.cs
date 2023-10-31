using AlarmPlus.Data;
using AlarmPlus.Models;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace AlarmPlus.Views;

[QueryProperty("reminder", "reminderkey")]
public partial class Reminder : ContentPage
{
    ISet<string> days = new HashSet<string>();
    ISet<int> daysNum = new HashSet<int>();
    //ISet<string> monthsRepeat = new HashSet<string>();
    ISet<int> monthsRepeat = new HashSet<int>();
    AlarmPlusDatabase Database;
    public Models.Reminder reminder
    {
        get => BindingContext as Models.Reminder;

        set => BindingContext = value;
    }


    public Reminder(AlarmPlusDatabase db)
    {
        InitializeComponent();
        Database = db;
    }

    
    async void btnCancel_Clicked(System.Object sender, System.EventArgs e)
    {
        
        //await Shell.Current.GoToAsync("..");
       
        //await Shell.Current.GoToAsync(nameof(AlarmPlus.Views.Alarm), true, new Dictionary<string, object>
        //{
         //   ["alarmkey"] = reminder
        //});
    }

    async void btnSave_Clicked(System.Object sender, System.EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(reminder.Name))
        {
            await DisplayAlert("Name Required", "Please enter a name for the Reminder item.", "OK");
            return;
        }
        // format the time
        
        reminder.ReminderTime = new TimeSpan(reminder.ReminderTime.Hours, reminder.ReminderTime.Minutes,0);
        reminder.TimeCreated = DateTime.Now;
        reminder.UserId = 1;
        reminder.IsActive = true;
        //get the repeat month and days if any
        if (swRepeat.IsToggled) monthsRepeat.Clear();
        else if (swMonthly.IsToggled) daysNum.Clear();
        reminder.ReminderWeekDays = string.Join(",", daysNum.Order());
        reminder.ReminderMonths = string.Join(",", monthsRepeat.Order());

        await Database.SaveReminderAsync(reminder);
        await Shell.Current.GoToAsync("..");
    }

    async void btnDelete_Clicked(System.Object sender, System.EventArgs e)
    {
        if (reminder.ID == 0)
            return;
        await Database.DeleteItemAsync(reminder);
        await Shell.Current.GoToAsync("..");
    }

    #region  Clicked Events Check Boxes
    void chM_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value? daysNum.Add(1):daysNum.Remove(1);

    }

    void chTu_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? daysNum.Add(2) : daysNum.Remove(2);
    }

    void chW_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? daysNum.Add(3) : daysNum.Remove(3);
    }

    void chTh_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? daysNum.Add(4) : daysNum.Remove(4);
    }

    void chF_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? daysNum.Add(5) : daysNum.Remove(5);
    }

    void chSat_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? daysNum.Add(6) : daysNum.Remove(6);
    }

    void chSun_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? daysNum.Add(0) : daysNum.Remove(0);
    }
    //Weekly Toggle
    void swRepeat_Toggled(System.Object sender, Microsoft.Maui.Controls.ToggledEventArgs e)
    {
        // disable the Monthly Toggle
        swMonthly.IsToggled = !e.Value;
            foreach(var c in htChecks1.Children)
            {
                if(c.GetType() == typeof(CheckBox) )
                    ((CheckBox)c).IsEnabled = e.Value;
            }
            foreach (var c in htChecks2.Children)
            {
                if (c.GetType() == typeof(CheckBox))
                    ((CheckBox)c).IsEnabled = e.Value;
            }

        }

    void chJan_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(1) : monthsRepeat.Remove(1);
    }

    void chFeb_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(2): monthsRepeat.Remove(2);
    }

    void chMar_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(3): monthsRepeat.Remove(3);
    }

    void chApr_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(4): monthsRepeat.Remove(4);
    }

    void chMay_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(5): monthsRepeat.Remove(5);
    }

    void chJune_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(6): monthsRepeat.Remove(6);
    }

    void chJuly_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(7): monthsRepeat.Remove(7);
    }

    void chAug_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(8) : monthsRepeat.Remove(8);
    }

   

    void chSep_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(9) : monthsRepeat.Remove(9);
    }

    void chOct_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(10): monthsRepeat.Remove(10);
    }

    void chNov_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
            var v = e.Value ? monthsRepeat.Add(11): monthsRepeat.Remove(11);
    }

    void chDec_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
        var v = e.Value ? monthsRepeat.Add(12) : monthsRepeat.Remove(12);
    }
    void swMonthly_Toggled_1(System.Object sender, Microsoft.Maui.Controls.ToggledEventArgs e)
    {
        // Disable the Weekly toggle
        swRepeat.IsToggled = !e.Value;
        foreach (var c in htChecks3.Children)
        {
            if (c.GetType() == typeof(CheckBox))
                ((CheckBox)c).IsEnabled = e.Value;
        }
        foreach (var c in htChecks4.Children)
        {
            if (c.GetType() == typeof(CheckBox))
                ((CheckBox)c).IsEnabled = e.Value;
        }
        foreach (var c in htChecks5.Children)
        {
            if (c.GetType() == typeof(CheckBox))
                ((CheckBox)c).IsEnabled = e.Value;
        }
    }
    #endregion


    void swEndDate_Toggled(System.Object sender, Microsoft.Maui.Controls.ToggledEventArgs e)
    {
        entEndDate.IsEnabled = e.Value;
    }

    void dpick_DateSelected(System.Object sender, Microsoft.Maui.Controls.DateChangedEventArgs e)
    {
        reminder.ReminderDateTime =  e.NewDate;
    }

    void tpick_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(e.PropertyName == "Time" )
        {
            reminder.ReminderDateTime = reminder.ReminderDate.Add(tpick.Time);
            var rr = reminder.ReminderTime;
            //reminder.ReminderTime = new DateTime(tpick.Time.Ticks);
        }

    }

    void ContentPage_Loaded(System.Object sender, System.EventArgs e)
    {
        var weekdays = reminder.ReminderWeekDays;
        var months = reminder.ReminderMonths;
        // find each day
        // check the appropriate check box
        chM.IsChecked = weekdays.Contains("Mon") ? true : false;
        chTu.IsChecked = weekdays.Contains("Tue") ? true : false;
        chW.IsChecked = weekdays.Contains("Wed") ? true : false;
        chTh.IsChecked = weekdays.Contains("Thu") ? true : false;
        chF.IsChecked = weekdays.Contains("Fri") ? true : false;
        chSat.IsChecked = weekdays.Contains("Sat") ? true : false;
        chSun.IsChecked = weekdays.Contains("Sun") ? true : false;
        // months

        chJan.IsChecked = months.Contains("Jan") ? true : false;
        chFeb.IsChecked = months.Contains("Feb") ? true : false;
        chMar.IsChecked = months.Contains("Mar") ? true : false;
        chApr.IsChecked = months.Contains("Apr") ? true : false;    
        chMay.IsChecked = months.Contains("May") ? true : false;
        chJune.IsChecked = months.Contains("Jun") ? true : false;
        chJuly.IsChecked = months.Contains("Jul") ? true : false;
        chAug.IsChecked = months.Contains("Aug") ? true : false;
        chSep.IsChecked = months.Contains("Sep") ? true : false;
        chOct.IsChecked = months.Contains("Oct") ? true : false;
        chNov.IsChecked = months.Contains("Nov") ? true : false;
        chDec.IsChecked = months.Contains("Dec") ? true : false;


    }

    async void tbShare_Clicked(System.Object sender, System.EventArgs e)
    {
        string[] days = reminder.ReminderWeekDays.Split(" ",StringSplitOptions.RemoveEmptyEntries);
        string Days="";
        foreach(string d in days)
        {
            Days += Constants.daysName2Num[d] + ",";
        }
        reminder.ReminderWeekDays = Days;

        // convert months to nummer forms
        string[] months = reminder.ReminderMonths.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        string Months = "";
        foreach(string m in months)
        {
            Months += Constants.monthName2Num[m] + ",";
        }
        reminder.ReminderMonths = Months;

        string r = Utilities.Common.Serizlize(reminder);
        
        await Utilities.Common.SendEmail2(new List<string> { "" }, "Reminder to share", r, null);
        //await Utilities.Common.SendSMSAsync("2142159530", r);
    }

    async void tbCancel_Clicked(System.Object sender, System.EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

