using System;
using System.IO;

namespace ClockIt.Mobile.Droid
{
	public class FileAccessHelper
	{
		public static string GetLocalFilePath(string filename)
		{
			var localPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(localPath, filename);
		}
	}
}