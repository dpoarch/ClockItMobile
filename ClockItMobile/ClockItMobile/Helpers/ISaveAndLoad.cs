using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockIt.Mobile.Helpers
{
    public interface ISaveAndLoad
    {
        Task SaveTextAsync(string filename, string text);
        Task<string> LoadTextAsync(string filename);
        bool FileExists(string filename);
        string CreatePathToFile(string filename);
        IEmailMessage SendEmailWithAttachment(string filename,string emailAddress);
        void NotifySound(string title, string body);
    }
}
