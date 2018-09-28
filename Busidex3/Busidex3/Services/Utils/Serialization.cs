using System;
using System.IO;

namespace Busidex3.Services.Utils
{
    public class Serialization
    {
        public static T LoadData<T> (string path) where T : class, new()
        {
            return LoadDataFromFile<T> (path);
        }

        static T LoadDataFromFile<T> (string path) where T : class, new()
        {
            var jsonData = string.Empty;
            try {
                jsonData = LoadFromFile (path);
                if(string.IsNullOrEmpty(jsonData)){
                    return default(T);
                }
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T> (jsonData);
                return result;
            } catch (Exception ex) {
                Xamarin.Insights.Report (new Exception("Error loading jsonData from " + path + ". DATA: " + jsonData, ex));
                return default(T);
            }
        }

        static string LoadFromFile (string fullFilePath)
        {
            string fileJson = string.Empty;
            if (File.Exists (fullFilePath)) {
                using (var file = File.OpenText (fullFilePath)) {
                    fileJson = file.ReadToEnd ();
                    file.Close ();
                }
            }
            return fileJson;
        }
        
        public static void SaveResponse (string response, string fileName)
        {
            var fullFilePath = Path.Combine (Resources.DocumentsPath, fileName);
            try {
                if (File.Exists (fullFilePath)) {
                    if (!IsFileInUse (new FileInfo (fullFilePath))) {
                        File.WriteAllText (fullFilePath, response);
                    }
                } else {
                    File.WriteAllText (fullFilePath, response);
                }
            } catch (Exception ex) {
                Xamarin.Insights.Report (new Exception("Error in SaveResponse saving " + fileName + " with response " + response, ex));
            }
        }

        private static bool IsFileInUse (FileInfo file)
        {
            FileStream stream = null;

            try {
                if (!File.Exists (file.FullName)) {
                    return false;
                }

                stream = file.Open (FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            } catch (IOException) {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            } finally
            {
                stream?.Close ();
            }
            return false;
        }

        public static T GetCachedResult<T> (string fileName) where T : new()
        {
            try {
                if (!File.Exists (fileName)) {
                    return new T ();
                }

                using (var file = File.OpenText (fileName)) {
                    var fileJson = file.ReadToEnd ();
                    file.Close ();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T> (fileJson);
                }
            } catch (IOException) {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return new T ();
            }
        }
    }
}
