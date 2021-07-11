using AttentionApp.Models;
using AttentionApp.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AttentionApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactsView : ContentPage
    {
        List<Models.Contact> contacts;
        List<Models.Contact> filteredContacts;

        public ContactsView()
        {
            InitializeComponent();
            Setup();
        }

        private async void Setup()
        {
            var status = await CheckAndRequestReadContactPermission();

            if(status == PermissionStatus.Granted)
            {
                DisplayContacts();
            }
            else
            {
                await Dialog.Show("Error", "Permission Must Be Allowed In Order To Use The App", "Ok");
                await Navigation.PopAsync();
            }
        }

        private async void DisplayContacts()
        {
            contacts = new List<Models.Contact>();
            var tempContacts = await Xamarin.Essentials.Contacts.GetAllAsync();

            if (tempContacts != null)
            {
                foreach (var contact in tempContacts)
                {
                    Models.Contact temp = new Models.Contact();
                    temp.Name = contact.DisplayName;
                    if (contact.Phones.Count == 0)
                    {
                        temp.Number = "";
                    }
                    else if (contact.Phones.Count == 1)
                    {
                        temp.Number = contact.Phones[0].ToString();
                        temp.Numbers = contact.Phones;
                    }
                    else
                    {
                        string tempS = "";
                        tempS += contact.Phones[0].ToString();
                        for (int i = 1; i < contact.Phones.Count; i++)
                        {
                            tempS += "\n" + contact.Phones[i].ToString();
                        }
                        temp.Number = tempS;
                        temp.Numbers = contact.Phones;
                    }
                    contacts.Add(temp);
                }
            }

            listContacts.ItemsSource = null;
            listContacts.ItemsSource = contacts;
        }

        protected override bool OnBackButtonPressed()
        {
            Back();
            return true;
        }

        private void btnBack_Clicked(object sender, EventArgs e)
        {
            Back();
        }

        private async void Back()
        {
            await Navigation.PopAsync();
        }

        private async void ViewCell_Tapped(object sender, EventArgs e)
        {
            var vc = (ViewCell)sender;
            listContacts.SelectedItem = null;

            Models.Contact temp = contacts.Find(a => a.Number == vc.ClassId);

            Settings settings = FileManager.LoadSettings();

            if(temp.Numbers.Count == 1)
            {
                settings.LastContact = temp;
                FileManager.SaveSettings(settings);
                await Navigation.PopAsync();
            }
            else if( temp.Numbers.Count > 1)
            {
                List<string> sNumbers = new List<string>();
                foreach (var number in temp.Numbers)
                {
                    sNumbers.Add(number.ToString());
                }
                string action = await DisplayActionSheet("Select A Number From The Contact " + temp.Name, "Cancel", null, sNumbers.ToArray());
                if(string.IsNullOrWhiteSpace(action))
                {
                    return;
                }
                else
                {
                    temp.Number = action;
                    settings.LastContact = temp;
                    FileManager.SaveSettings(settings);
                    await Navigation.PopAsync();
                }
            }
        }

        public async Task<PermissionStatus> CheckAndRequestReadContactPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();

            if (status == PermissionStatus.Granted)
                return status;

            status = await Permissions.RequestAsync<Permissions.ContactsRead>();

            return status;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string temp = "";
            temp = txtSearch.Text;
            if (string.IsNullOrWhiteSpace(temp))
            {
                listContacts.ItemsSource = null;
                listContacts.ItemsSource = contacts;
            }
            else
            {
                filteredContacts = new List<Models.Contact>();
                foreach (var contact in contacts)
                {
                    if (contact.Name.ToLower().Contains(temp.ToLower()))
                    {
                        filteredContacts.Add(contact);
                    }
                }
                listContacts.ItemsSource = null;
                listContacts.ItemsSource = filteredContacts;
            }
        }
    }
}