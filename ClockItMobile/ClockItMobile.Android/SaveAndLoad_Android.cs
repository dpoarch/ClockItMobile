using System;
using Xamarin.Forms;
using System.IO;
using System.Threading.Tasks;
using ClockIt.Mobile.Helpers;
using ClockIt.Mobile.Droid;
using Plugin.Messaging;
using Android.App;
using Android.Media;
using System.Runtime.Remoting.Contexts;

[assembly: Dependency(typeof(SaveAndLoad_Android))]
namespace ClockIt.Mobile.Droid
{
    public class SaveAndLoad_Android : ISaveAndLoad
    {
        #region ISaveAndLoad implementation

        public async Task SaveTextAsync(string filename, string text)
        {
            var path = CreatePathToFile(filename);
            using (StreamWriter sw = File.CreateText(path))
                await sw.WriteAsync(text);
        }

        public async Task<string> LoadTextAsync(string filename)
        {
            var path = CreatePathToFile(filename);
            using (StreamReader sr = File.OpenText(path))
                return await sr.ReadToEndAsync();
        }

        public bool FileExists(string filename)
        {
            return File.Exists(CreatePathToFile(filename));
        }

        public string CreatePathToFile(string filename)
        {
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(docsPath, filename);
        }

        public IEmailMessage SendEmailWithAttachment(string filename,string emailAddress)
        {
            var file = new Java.IO.File(filename);
            var email = new EmailMessageBuilder()
              .To(emailAddress)
              .Subject("Xamarin Messaging Plugin")
              .Body("Well hello there from Xam.Messaging.Plugin")
              .WithAttachment(file).Build();
            return email;

        }

        public void NotifySound(string title, string body) {
            ((MediaPlayer)App.Player).Start();
        }

        #endregion

    }
}