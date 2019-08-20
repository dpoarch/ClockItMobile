using System;
using System.IO;

namespace ClockIt.Mobile.iOS
{
	public class FileAccessHelper
	{
		public static string GetLocalFilePath(string filename)
		{
			var localPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var libraryPath = Path.Combine(localPath, "..", "Library");

			Directory.CreateDirectory(libraryPath);

			return Path.Combine(libraryPath, filename);
		}
	}
}
