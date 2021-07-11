using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace AttentionApp.Models
{
    public class Message
    {
        public string Name { get; set; }

        public bool Selected = false;

        [JsonIgnore]
        public Style Icon
        {
            get
            {
                if (Selected)
                {
                    return Application.Current.Resources["RadioChecked"] as Style;
                }
                else
                {
                    return Application.Current.Resources["RadioUnchecked"] as Style;
                }
            }
        }
    }
}
