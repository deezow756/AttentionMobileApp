using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using AttentionApp.Themes;
using Android.Content.Res;
using Xamarin.Forms;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using Android;
using AndroidX.Core.App;
using Google.Android.Material.Snackbar;

namespace AttentionApp.Droid
{
    [Activity(Label = "AttentionApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            var theme = Xamarin.Essentials.AppInfo.RequestedTheme;
            switch (theme)
            {
                case Xamarin.Essentials.AppTheme.Unspecified:
                    SetAppTheme(ThemeType.Light);
                    break;
                case Xamarin.Essentials.AppTheme.Light:
                    SetAppTheme(ThemeType.Light);
                    break;
                case Xamarin.Essentials.AppTheme.Dark:
                    SetAppTheme(ThemeType.Dark);
                    break;
                default:
                    break;
            }

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            RequestedOrientation = ScreenOrientation.Portrait;

            MessagingCenter.Subscribe<Xamarin.Forms.Application>(this, "Restart", sender =>
            {
                Restart();
            });

            MessagingCenter.Subscribe<Xamarin.Forms.Application, ThemeType>(this, "ChangeTheme", async (sender, arg) =>
            {
                SetAppTheme(arg);
            });

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadContacts) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadContacts }, 0);
            }
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.SendSms) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.SendSms }, 0);
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            ThemeHelper.ChangeTheme(true);
        }

        void SetAppTheme(ThemeType arg)
        {
            if (arg == ThemeType.Dark)
            {
                SetTheme(Resource.Style.NightTheme);
                Delegate.SetLocalNightMode(AppCompatDelegate.ModeNightYes);
            }
            else
            {
                SetTheme(Resource.Style.DayTheme);
                Delegate.SetLocalNightMode(AppCompatDelegate.ModeNightNo);
            }
        }

        void Restart()
        {
            Recreate();
        }
    }
}