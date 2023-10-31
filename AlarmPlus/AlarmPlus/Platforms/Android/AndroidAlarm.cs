using AlarmPlus.Views;
using Android.App;
using Android.Content;
using Android.OS;
using androidos = Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using static Android.Views.View;
using Android.Media;

namespace AlarmPlus.Platforms.Android;

[Activity(Label = "AndroidAlarm")]
public class AndroidAlarm : Activity
{

    /*
         * Primitive data can be passed as string-based query parameters when 
         * performing URI-based programmatic navigation.
         * This is achieved by appending ? after a route, 
         * followed by a query parameter id, =, and a value:
            async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                string elephantName = (e.CurrentSelection.FirstOrDefault() as Animal).Name;
                await Shell.Current.GoToAsync($"elephantdetails?name={elephantName}");
            }
     */

    // List<Reminder> rs = new List<Reminder>();

    MediaPlayer mp;
    string RSound;
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        int view = Resource.Layout.android_alarm;
        SetContentView(view);

        //var RN = Intent.GetStringExtra("exRN");
        //var RT = Intent.GetStringExtra("exRT");
        //var RDesc = Intent.GetStringExtra("exRDesc");

        //var bl = Intent.GetBundleExtra("bndl");
        //bl = Intent.GetBundleExtra()
        //var sa = bl.GetStringArray("exAlarmVals");

        RSound = Intent.GetStringExtra("exRSound");

        var btn = FindViewById(Resource.Id.btnAlarm);
        var btnEmail = FindViewById(Resource.Id.btnEmail);
        btn.Click += Btn_Click;
        btnEmail.Click += btnEmail_Click;
        btn.KeyPress += Btn_KeyPress; // just for experiment to see what it is . not used

        //var btnEmail = FindViewById(Resource.Id.btnEmailAlarm);
        //view


        ((TextView)FindViewById(Resource.Id.tv_rt)).Text = Intent.GetStringExtra("exRT");
        ((TextView)FindViewById(Resource.Id.tv_nrame)).Text = Intent.GetStringExtra("exRN");
        ((TextView)FindViewById(Resource.Id.tv_rdesc)).Text = Intent.GetStringExtra("exRDesc");
        ((TextView)FindViewById(Resource.Id.tv_rSound)).Text = RSound;
        if (string.IsNullOrEmpty(RSound))
        {

            PlaydefaultSound();
        }
        else
        {
            SpeakNowDefaultSettingsAsync(RSound);
        }

        // if(!string.IsNullOrEmpty (Intent.GetStringExtra("exEmails")))
        //{
        //  SendEmail2(new List<string> { Intent.GetStringExtra("exEmails") }, Intent.GetStringExtra("exRN"), Intent.GetStringExtra("exRDesc"));
        // }


    }

    public override global::Android.Views.View OnCreateView(string name, Context context, IAttributeSet attrs)
    {


        return base.OnCreateView(name, context, attrs);
    }
    public List<Reminder> DeserializeList(string sr)
    {
        return JsonConvert.DeserializeObject<List<Reminder>>(sr);
    }
    CancellationTokenSource cts;

    private  void SpeakNowDefaultSettingsAsync(string soundText)
    {
        cts =  new CancellationTokenSource();
        SpeakMultiple(soundText);

    }
    bool isBusy = false;


    private void PlaydefaultSound()
    {
        var tone = RingtoneManager.GetDefaultUri(RingtoneType.Alarm);
        //var dm = Directory.GetFiles(androidos.Environment.DirectoryRingtones);
        mp = MediaPlayer.Create(this, tone);
        mp.Start();
    }
    public void SpeakMultiple(string txt)
    {
        isBusy = true;
        var speechTasks = new List<Task>();
        for (int i = 0; i <= 10; i++)
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

    private void Btn_KeyPress(object sender, KeyEventArgs e)
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// need to enable this event in the properties.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Btn_Click(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(About),true);
        if (!string.IsNullOrEmpty(RSound)) CancelSpeech();
        if (string.IsNullOrEmpty(RSound)) mp?.Stop();

        /* await Shell.Current.GoToAsync(nameof(Alarm), true, new Dictionary<string, object>
         {
             ["alarmkey"] = rs[0]
         });*/


    }
    private void btnEmail_Click(object sender, EventArgs e)
    {
        SendEmail2(new List<string> { Intent.GetStringExtra("exEmails") }, Intent.GetStringExtra("exRN"),
            $"[{Intent.GetStringExtra("exRDesc")}]:{RSound}");
    }

    async void SendEmail2(List<string> emails, string sub, string body)
    {
        try
        {
            EmailMessage msg = new EmailMessage
            {
                To = emails,// new List<string> { "abhinawsharma@gmail.com" },
                Body = body,// "Hello Email from MAUI Alarm Plus",
                Subject = sub,//"Email subject here - Alarm Plus"
            };
            await Email.Default.ComposeAsync(msg);
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
    /// <summary>
    /// this is by default. no need to enable it
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
    {

        return base.OnKeyDown(keyCode, e);
    }



}


