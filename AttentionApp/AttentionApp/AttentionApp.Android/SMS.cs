using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using AttentionApp.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SMS))]
namespace AttentionApp.Droid
{
    public class SMS : ISMS
    {
        public async Task SendMessage(string number, string message)
        {
            var _smsManager = SmsManager.Default;

            _smsManager.SendTextMessage(number, null, message, null, null);

            Toast.MakeText(Android.App.Application.Context, "Message Sent", ToastLength.Short).Show();
        }
    }
}