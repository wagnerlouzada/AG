/// Mohamed Ali NOUIRA
/// http://www.sweetmit.com
/// http://www.mohamedalinouira.com
/// https://github.com/medalinouira
/// Copyright © Mohamed Ali NOUIRA. All rights reserved.

using System.IO;
using Xamarin.Forms;
using Windows.Storage;
//using agsqlite.UWP.Helpers;
using agsqlite.Helpers;
using agsqlite.UWP.Helpers;

[assembly: Dependency(typeof(FileHelper))]
namespace agsqlite.UWP.Helpers
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, filename);
        }
    }
}
