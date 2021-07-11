using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace AttentionApp.Models
{
    public class Contact
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public List<ContactPhone> Numbers = new List<ContactPhone>();
    }
}
