using System;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace AlarmPlus.Utilities
{
    public class Sound
    {
        MediaElement me;
        public Sound(MediaElement mediaele)
        {
            me = mediaele;
        }
        public void PlaySoud(string src)
        {
            //var ms = UriMediaSource.FromUri("https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4");
            var ms = ResourceMediaSource.FromResource(src);
            me.Source = ms;
            me.MinimumHeightRequest = 100;
            me.MinimumWidthRequest = 100;
            me.IsVisible = true;
            me.Play();
        }
    }
    public class clsEmail
    {
        public clsEmail()
        {

        }

        public async Task SendEmailAsync(string Sub, string Body,string[] EmailList)
        {
            if (Email.Default.IsComposeSupported)
            {
                string subject = Sub;// "Hello friends!";
                string body = Body;// "It was great to see you last weekend.";
                List<string> recipients = new List<string>(EmailList);// new[] { "abhinawsharma@gmail.com" };

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = recipients
                };
                //string picturePath = Path.Combine(FileSystem.CacheDirectory, "memories.jpg");
                //message.Attachments.Add(new EmailAttachment(picturePath));
                await Email.Default.ComposeAsync(message);
            }
        }

    }

   /* public class clsSendSMS
    {
        public async Task SendSMSAsync(string ph,string txt)
        {
            

            if (Sms.Default.IsComposeSupported)
            {
                string[] recipients = new[] { ph };
                string text = txt;
                var message = new SmsMessage
                {
                    Body = text,
                    Recipients = new List<string>(recipients)
                };

                await Sms.Default.ComposeAsync(message);
            }
        }

        public  void CallPhone()
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open("000-000-0000");
        }
    }*/
}

