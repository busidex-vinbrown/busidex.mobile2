using System;
using System.IO;

namespace Busidex3.Services.Utils
{
    public class Serialization
    {
        public static T LoadData<T> (string path) where T : class, new()
        {
            return loadDataFromFile<T> (path);
        }

        static T loadDataFromFile<T> (string path) where T : class, new()
        {
            var jsonData = string.Empty;
            try {
                jsonData = loadFromFile (path);
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

        static string loadFromFile (string fullFilePath)
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
    }
}
