using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.iOS;
using Foundation;
using Plugin.Messaging;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveAndLoad_iOS))]
namespace ClockIt.Mobile.iOS
{
    public class SaveAndLoad_iOS : ISaveAndLoad
    {
        #region ISaveAndLoad implementation
        public bool FileExists(string filename)
        {
            return File.Exists(CreatePathToFile(filename));
        }

        public async Task<string> LoadTextAsync(string filename)
        {
            var path = CreatePathToFile(filename);
            using (StreamReader sr = File.OpenText(path))
                return await sr.ReadToEndAsync();
        }

        public async Task SaveTextAsync(string filename, string text)
        {
            var path = CreatePathToFile(filename);
            using (StreamWriter sw = File.CreateText(path))
                await sw.WriteAsync(text);
        }
        public string CreatePathToFile(string filename)
        {
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(docsPath, filename);
        }

        public IEmailMessage SendEmailWithAttachment(string filename, string emailAddress)
        {
            NSUrl file = new NSUrl(filename, false);
            var email = new EmailMessageBuilder()
              .To(emailAddress)
              .Subject("Xamarin Messaging Plugin")
              .Body("Well hello there from Xam.Messaging.Plugin")
              .WithAttachment(file).Build();
            return email;
        }

        public void NotifySound(string title, string body)
        {
            ((AVAudioPlayer)App.Player).Play();
        }
        #endregion

    }
}