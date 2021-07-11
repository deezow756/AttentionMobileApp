using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace AttentionApp.Themes
{
    public enum ThemeType {Light, Dark}
    public class ThemeHelper
    {
        public static ThemeHelper instance;
        public ThemeHelper()
        {
            instance = this;
        }
        public static EventHandler ThemeChanged;
        public static void OnThemeChanged(EventArgs e)
        {
            EventHandler handler = ThemeChanged;
            if(handler != null)
            {
                handler(null, e);
            }
        }

        private static ThemeType currentTheme;
        public static ThemeType CurrentTheme
        {
            get { return currentTheme; }
            set
            {
                currentTheme = value;
                OnThemeChanged(null);
            }
        }
        public static void ChangeTheme(bool ManualChange)
        {
            var curTheme = AppInfo.RequestedTheme;
            switch (curTheme)
            {
                case AppTheme.Unspecified:
                    Application.Current.Resources = new LightTheme();
                    CurrentTheme = ThemeType.Light;
                    break;
                case AppTheme.Light:
                    Application.Current.Resources = new LightTheme();
                    CurrentTheme = ThemeType.Light;
                    break;
                case AppTheme.Dark:
                    Application.Current.Resources = new DarkTheme();
                    CurrentTheme = ThemeType.Dark;
                    break;
                default:
                    break;
            }
            //FontHelper.ChangeFont(FileManager.LoadSettings().Font);
            if(ManualChange)
            {
                MessagingCenter.Send(App.Current, "Restart");
            }
        }
        //public static void ChangeTheme(FontType font, bool ManualChange)
        //{
        //    var curTheme = AppInfo.RequestedTheme;
        //    switch (curTheme)
        //    {
        //        case AppTheme.Unspecified:
        //            Application.Current.Resources = new LightTheme();
        //            CurrentTheme = ThemeType.Light;
        //            break;
        //        case AppTheme.Light:
        //            Application.Current.Resources = new LightTheme();
        //            CurrentTheme = ThemeType.Light;
        //            break;
        //        case AppTheme.Dark:
        //            Application.Current.Resources = new DarkTheme();
        //            CurrentTheme = ThemeType.Dark;
        //            break;
        //        default:
        //            break;
        //    }
        //    FontHelper.ChangeFont(font);
        //    if (ManualChange)
        //    {
        //        MessagingCenter.Send(App.Current, "Restart");
        //    }
        //}

        //public static void ChangeTheme(ThemeType theme, bool ManualChange)
        //{
        //    switch (theme)
        //    {
        //        case ThemeType.Light:
        //            Application.Current.Resources = new LightTheme();
        //            CurrentTheme = ThemeType.Light;                    
        //            break;
        //        case ThemeType.Dark:
        //            Application.Current.Resources = new DarkTheme();
        //            CurrentTheme = ThemeType.Dark;
        //            break;
        //        default:
        //            break;
        //    }
        //    FontHelper.ChangeFont(FileManager.LoadSettings().Font);
        //    if (ManualChange)
        //    {
        //        MessagingCenter.Send(App.Current, "Restart");
        //    }
        //}

        //public static void ChangeTheme(ThemeType theme, FontType font, bool ManualChange)
        //{
        //    switch (theme)
        //    {
        //        case ThemeType.Light:
        //            Application.Current.Resources = new LightTheme();
        //            CurrentTheme = ThemeType.Light;
        //            break;
        //        case ThemeType.Dark:
        //            Application.Current.Resources = new DarkTheme();
        //            CurrentTheme = ThemeType.Dark;
        //            break;
        //        default:
        //            break;
        //    }

        //    FontHelper.ChangeFont(font);
        //    if (ManualChange)
        //    {
        //        MessagingCenter.Send(App.Current, "Restart");
        //    }
        //}
    }
}
