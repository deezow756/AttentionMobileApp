using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AttentionApp
{
    public interface ISMS
    {
        Task SendMessage(string number, string message);
    }
}
