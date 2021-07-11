using AttentionApp.Models;
using AttentionApp.Resources.Fonts;
using AttentionApp.Themes;
using AttentionApp.Utilities;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AttentionApp
{
    public partial class App : Application
    {
        public App()
        {
            Device.SetFlags(new string[] { "AppTheme_Experimental" });
            InitializeComponent();
            ThemeHelper.ChangeTheme(false);
            FontHelper.ChangeFont(FontType.Tech);
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
