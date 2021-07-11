using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using AttentionApp.Models;

namespace AttentionApp.Utilities
{
    public class FileManager
    {
        private static string loginFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "settings.json");
        public static Settings LoadSettings()
        {
            if(File.Exists(loginFilePath))
            {
                Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(loginFilePath));
                return settings;
            }
            else
            {
                Settings settings = new Settings() { LastContact = new Contact() { Name = "Please Select A Contact In Contacts", Number = "" }, Messages = new List<Message>() { new Message() { Name = "Give Me Attention!!!"} } };
                File.WriteAllText(loginFilePath, JsonConvert.SerializeObject(settings, Formatting.None));
                return settings;
            }
        }

        public static void SaveSettings(Settings settings)
        {
            File.WriteAllText(loginFilePath, JsonConvert.SerializeObject(settings, Formatting.None));
        }
    }
}
