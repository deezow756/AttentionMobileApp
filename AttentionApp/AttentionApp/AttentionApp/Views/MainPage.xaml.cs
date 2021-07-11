using AttentionApp.Models;
using AttentionApp.Utilities;
using AttentionApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AttentionApp
{
    public partial class MainPage : ContentPage
    {
        private Settings settings;
        private Models.Contact curContact;
        private Message curMessage;
        private readonly int countRestriction = 100;
        private readonly int countReset = 60;
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Setup();
        }

        private async void Setup()
        {
            settings = FileManager.LoadSettings();
            curContact = settings.LastContact;
            txtName.Text = settings.LastContact.Name;
            txtNumber.Text = settings.LastContact.Number;

            listMessages.ItemsSource = null;
            listMessages.ItemsSource = settings.Messages;

            bool found = false;
            foreach (var Message in settings.Messages)
            {
                if(Message.Selected)
                {
                    curMessage = Message;
                    found = true;
                    break;
                }
            }
            if (!found)
                curMessage = null;
            if (settings.Locked)
            {
                btnLock.Style = Application.Current.Resources["LockedImageButton"] as Style;
                if (settings.Restricted)
                {
                    if ((DateTime.Now - settings.RestrictionTime).TotalMinutes > countReset)
                    {
                        settings.counter = 0;
                        settings.Restricted = false;
                        FileManager.SaveSettings(settings);
                        layoutTimer.IsVisible = false;
                        layoutButton.IsVisible = true;
                    }
                    else
                    {
                        //double temp = (DateTime.Now - settings.RestrictionTime).TotalMinutes;
                        //await Dialog.Show("Oops", "Unfortunately, You Still Need To Wait Another " + (countReset - Convert.ToInt32(temp)).ToString() + " Minutes Before Being Able To Send A Message Again", "Ok");
                        layoutButton.IsVisible = false;
                        layoutTimer.IsVisible = true;
                        Task.Run(StartCountDown);
                    }
                }
            }
            else
            {
                btnLock.Style = Application.Current.Resources["UnlockedImageButton"] as Style;
                settings.counter = 0;
                settings.Restricted = false;
                FileManager.SaveSettings(settings);
                layoutTimer.IsVisible = false;
                layoutButton.IsVisible = true;
            }
        }

        private async void btnHelp_Clicked(object sender, EventArgs e)
        {
            await Dialog.Show("", "To Get Started Select A Contact From The Contacts Section In The Top Right Corner Which Will Have All The Contacts From Your Phones Contacts, Then Select A Message Or Simply Make Your Own Message By Tapping The Plus Button, You Can Also Delete A Message By Selecting A Message And Tapping The Bin." +
                " Once A Contact And A Message Have Been Selected, Pressing The Big Red Button Will Send The Contact The Selected Message, By Repeatedly Pressing The Button, It Will Send Alot More Messages.\n\nEnjoy Annoying People!\n\nWarning: You Can Only Send 50 Messages Per Hour So Use Your Messages Wisely!", "Close");
        }

        private async void btnContacts_Clicked(object sender, EventArgs e)
        {
            await LoadingPopup();
            btnContacts.InputTransparent = true;            
            await Navigation.PushAsync(new ContactsView());
            ClosePopup();
            btnContacts.InputTransparent = false;
        }

        private async void btnAttention_Clicked(object sender, EventArgs e)
        {
            btnAttention.InputTransparent = true;

            if (settings.Locked)
            {
                if (settings.Restricted)
                {
                    if ((DateTime.Now - settings.RestrictionTime).TotalMinutes > countReset)
                    {
                        settings.counter = 0;
                        settings.Restricted = false;
                        FileManager.SaveSettings(settings);
                    }
                    else
                    {
                        double temp = (DateTime.Now - settings.RestrictionTime).TotalMinutes;
                        await Dialog.Show("Oops", "Unfortunately, You Still Need To Wait Another " + (countReset - Convert.ToInt32(temp)).ToString() + " Minutes Before Being Able To Send A Message Again", "Ok");
                        btnAttention.InputTransparent = false;
                        return;
                    }
                }
            }

            if(curContact == null)
            {
                await Dialog.Show("Error", "Please Select A Contact From Contacts Section First", "Ok");
                btnAttention.InputTransparent = false;
                return;
            }
            else
            {
                if(string.IsNullOrWhiteSpace(curContact.Number))
                {
                    await Dialog.Show("Error", "Please Select A Contact From Contacts Section First", "Ok");
                    btnAttention.InputTransparent = false;
                    return;
                }
            }

            if(curMessage == null)
            {
                await Dialog.Show("Error", "Please Select A Message To Send First", "Ok");
                btnAttention.InputTransparent = false;
                return;
            }
            else
            {
                if(string.IsNullOrWhiteSpace(curMessage.Name))
                {
                    await Dialog.Show("Error", "Please Select A Message To Send First", "Ok");
                    btnAttention.InputTransparent = false;
                    return;
                }
            }

            var status = await CheckAndRequestSendSmsPermission();

            if (status == PermissionStatus.Granted)
            {
                await SMS.SendMessage(curContact.Number, curMessage.Name);
            }
            else
            {
                await Dialog.Show("Error", "Send Sms Permission Must Be Allowed In Order To Send A Message", "Ok");
                btnAttention.InputTransparent = false;
                return;
            }

            if (settings.Locked)
            {
                settings.counter += 1;
                if (settings.counter >= countRestriction)
                {
                    settings.Restricted = true;
                    settings.RestrictionTime = DateTime.Now;
                    layoutButton.IsVisible = false;
                    layoutTimer.IsVisible = true;
                    Task.Run(StartCountDown);
                }
                FileManager.SaveSettings(settings);
            }

            btnAttention.InputTransparent = false;
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            var vc = (ViewCell)sender;
            listMessages.SelectedItem = null;

            if(settings.Messages.Count > 0)
            {
                foreach (var Message in settings.Messages)
                {
                    Message.Selected = false;
                }
                settings.Messages.Find(a => a.Name == vc.ClassId).Selected = true;
                curMessage = settings.Messages.Find(a => a.Name == vc.ClassId);
                listMessages.ItemsSource = null;
                listMessages.ItemsSource = settings.Messages;
                FileManager.SaveSettings(settings);
            }
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Adding A New Message", "Enter New Message");
            if(result != null)
            {
                bool found = false;
                foreach (var Message in settings.Messages)
                {
                    if(Message.Name.ToLower() == result.ToLower())
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    await Dialog.Show("Error", "This Message Already Exists\n\nTry Again With A Message You Dont Already Have", "Ok");
                    return;
                }
                settings.Messages.Add(new Message() { Name = result });
                FileManager.SaveSettings(settings);
                listMessages.ItemsSource = null;
                listMessages.ItemsSource = settings.Messages;
            }
        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            if(settings.Messages.Count > 0)
            {
                Message msg = settings.Messages.Find(a => a.Selected == true);
                if(msg == null)
                {
                    await Dialog.Show("Error", "Please First Select A Message To Delete", "Ok");
                    return;
                }

                bool result = await Dialog.Show("Delete", "Are You Sure You Want To Delete The Message?\n\n" + msg.Name, "Yes", "No");
                if(result)
                {
                    settings.Messages.Remove(msg);
                    FileManager.SaveSettings(settings);
                    listMessages.ItemsSource = null;
                    listMessages.ItemsSource = settings.Messages;
                    curMessage = null;
                }
            }
        }

        public async Task<PermissionStatus> CheckAndRequestSendSmsPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Sms>();

            if (status == PermissionStatus.Granted)
                return status;

            status = await Permissions.RequestAsync<Permissions.Sms>();

            return status;
        }

        private async Task LoadingPopup()
        {
            arcFrame.IsVisible = true; //visible the frame
            //Scroll.ScrollToAsync(arcFrame, ScrollToPosition.Center, true); //scrolls so that the frame is at the center of the list
            MainContent.Opacity = 0.3; //set the main grid opacity to low
            MainContent.InputTransparent = true; //set the main grid not touchable
            LoadingPopup page = new LoadingPopup();
            frameArcContentView.Content = page;
        }

        private void ClosePopup()
        {
            arcFrame.IsVisible = false; //hide the frame
            MainContent.Opacity = 1; //make back the opacity of main grid
            MainContent.InputTransparent = false; //make main grid touchable
        }

        private async Task StartCountDown()
        {
            double temp = 0;
            if ((DateTime.Now - settings.RestrictionTime).TotalMinutes > countReset)
            {
                settings.counter = 0;
                settings.Restricted = false;
                FileManager.SaveSettings(settings);
                return;
            }
            else
            {
                temp = (DateTime.Now - settings.RestrictionTime).TotalSeconds;
                temp = (countReset * 60) - Convert.ToInt32(temp);
                TimeSpan _TimeSpan = TimeSpan.FromSeconds(temp);
                string time = string.Format("{0:00}:{1:00}", _TimeSpan.Minutes, _TimeSpan.Seconds);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    txtTimer.Text = time;
                });
            }

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                temp--;
                if ((DateTime.Now - settings.RestrictionTime).TotalMinutes > countReset)
                {
                    settings.counter = 0;
                    settings.Restricted = false;
                    FileManager.SaveSettings(settings);
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        layoutTimer.IsVisible = false;
                        layoutButton.IsVisible = true;
                    });
                }
                if (layoutTimer.IsVisible == false)
                {
                    return false;
                }
                TimeSpan _TimeSpan = TimeSpan.FromSeconds(temp);
                string time = string.Format("{0:00}:{1:00}", _TimeSpan.Minutes, _TimeSpan.Seconds);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    txtTimer.Text = time;
                });
                return true;
            });
        }

        private async void btnLock_Clicked(object sender, EventArgs e)
        {
            btnLock.InputTransparent = true;
            if(settings.Locked)
            {
                var result = await ShowPasscodePopupView().GetResult();
                HidePasscodePopupView();
                if(result)
                {
                    btnLock.Style = Application.Current.Resources["UnlockedImageButton"] as Style;
                    settings.counter = 0;
                    settings.Restricted = false;                    
                    settings.Locked = false;
                    FileManager.SaveSettings(settings);
                    layoutTimer.IsVisible = false;
                    layoutButton.IsVisible = true;
                }
            }
            else
            {
                btnLock.Style = Application.Current.Resources["LockedImageButton"] as Style;
                settings.Locked = true;
                FileManager.SaveSettings(settings);
            }
            btnLock.InputTransparent = false;
        }

        private PasscodePopup ShowPasscodePopupView()
        {
            passcodePopupFrame.IsVisible = true; //visible the frame
            //Scroll.ScrollToAsync(arcFrame, ScrollToPosition.Center, true); //scrolls so that the frame is at the center of the list
            MainContent.Opacity = 0.3; //set the main grid opacity to low
            MainContent.InputTransparent = true; //set the main grid not touchable
            PasscodePopup page = new PasscodePopup();
            passcodePopupContentView.Content = page;
            return page;
        }

        private void HidePasscodePopupView()
        {
            passcodePopupFrame.IsVisible = false; //hide the frame
            passcodePopupContentView.Content = null;
            MainContent.Opacity = 1; //make back the opacity of main grid
            MainContent.InputTransparent = false; //make main grid touchable
        }
    }
}
