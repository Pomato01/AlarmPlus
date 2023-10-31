using Newtonsoft.Json;

namespace AlarmPlus.Utilities;

public class Common : ContentPage
{
	public Common()
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Helpful common functions"
				}
			}
		};
	}

    public static string Serizlize(Models.Reminder r)
    {
        return JsonConvert.SerializeObject(r);
    }
    public static string Serizlize(List<Models.Reminder> r)
    {
        return JsonConvert.SerializeObject(r);
    }
    public static Models.Reminder Deserialize(string sr)
    {
        return JsonConvert.DeserializeObject<Models.Reminder>(sr);
    }

    public async static Task SendEmail2(List<string> emails, string sub, string body,string [] attachFileNames)
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
                if (attachFileNames != null && attachFileNames.Length > 0)
                {
                    foreach (var f in attachFileNames)
                    {

                        msg.Attachments.Add(new EmailAttachment(f));
                    }
                }
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

    public static async Task SendSMSAsync(string ph, string txt)
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

    public static void CallPhone()
    {
        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open("000-000-0000");
    }
}
