using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AttentionApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasscodePopup : ContentView
    {
        private bool returnResult = false;
        private bool result;

        private readonly string passcode = "300594";
        public PasscodePopup()
        {
            InitializeComponent();
        }

        public Task<bool> GetResult()
        {
            return Task.Run(() =>
            {
                do
                {
                    if (returnResult)
                    {
                        return result;
                    }
                } while (true);
            });
        }

        private void btn_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            string temp = txtPasscode.Text;
            if (!string.IsNullOrWhiteSpace(temp))
            {
                if (temp.Length >= 10)
                    return;
            }
            temp += btn.Text;
            txtPasscode.Text = temp;
        }

        private void btnBackSpace_Clicked(object sender, EventArgs e)
        {
            btnBackSpace.InputTransparent = true;
            string temp = txtPasscode.Text;
            if(!string.IsNullOrWhiteSpace(temp))
            {
                string newTemp = "";
                for (int i = 0; i < temp.Length - 1; i++)
                {
                    newTemp += temp[i].ToString();
                }
                txtPasscode.Text = newTemp;
            }
            btnBackSpace.InputTransparent = false;
        }

        private void btnSubmit_Clicked(object sender, EventArgs e)
        {
            string temp = txtPasscode.Text;
            if(!string.IsNullOrWhiteSpace(temp))
            {
                if(temp == passcode)
                {
                    result = true;
                    returnResult = true;
                }
                else
                {
                    ShakeScreen();
                }
            }
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            result = false;
            returnResult = true;
        }

        private async void ShakeScreen()
        {
            uint timeout = 50;

            await layoutPopup.TranslateTo(-15, 0, timeout);

            await layoutPopup.TranslateTo(15, 0, timeout);

            await layoutPopup.TranslateTo(-10, 0, timeout);

            await layoutPopup.TranslateTo(10, 0, timeout);

            await layoutPopup.TranslateTo(-5, 0, timeout);

            await layoutPopup.TranslateTo(5, 0, timeout);

            layoutPopup.TranslationX = 0;
        }
    }
}