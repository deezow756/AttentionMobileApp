using AttentionApp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AttentionApp
{
    public class SMS
    {
        public static async Task SendMessage(string number, string message)
        {
            await DependencyService.Get<ISMS>().SendMessage(number, message);
        }
    }
}
