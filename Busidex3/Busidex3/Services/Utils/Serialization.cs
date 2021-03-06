﻿using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Busidex3.Models;
using Microsoft.AppCenter.Crashes;
using File = System.IO.File;

namespace Busidex3.Services.Utils
{
    public class Serialization
    {
        public static T LoadData<T>(string path) where T : class, new()
        {
            return LoadDataFromFile<T>(path);
        }

        public static string LocalStorageFolder =>
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static string AppDataFolder => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string CopyIcon(string fileName)
        {
            if (String.IsNullOrEmpty(fileName)) return string.Empty;
            try
            {
                var iconFolder = Path.Combine(LocalStorageFolder, "icons");
                if (!Directory.Exists(iconFolder))
                {
                    Directory.CreateDirectory(iconFolder);
                }
                // Create (or open if already exists) subfolder "icons" in application local storage

                /*
                var fld =  await FileSystem.Current.LocalStorage.CreateFolderAsync("icons", CreationCollisionOption.OpenIfExists);
                if (fld == null) return ""; // Failed to create folder

                // Check if the file has not been copied earlier
                if (await fld.CheckExistsAsync(fileName) == ExistenceCheckResult.FileExists)
                    return (await fld.GetFileAsync(fileName))?.Path; // Image copy already exists
                */
                var fullFilePath = Path.Combine(iconFolder, fileName);
                if (File.Exists(fullFilePath))
                {
                    return fullFilePath;
                }

                // Source assembly and embedded resource path
                var imageSrcPath = $"Busidex3.Resources.{fileName}"; // Full resource name
                var assembly = typeof(Serialization).GetTypeInfo().Assembly;

                // Copy image from resources to the file in application local storage
                //var file = await fld.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

                //using (var target = await file.OpenAsync(FileAccess.ReadAndWrite))
                using (var source = assembly.GetManifestResourceStream(imageSrcPath))
                {
                    using (var fileStream = File.Create(fullFilePath))
                    {
                        source?.Seek(0, SeekOrigin.Begin);
                        source?.CopyTo(fileStream);
                    }

                    //await source?.CopyToAsync(target); // Copy file stream
                }

                return fileName; // Result is the path to the new file
            }
            catch
            {
                return string.Empty; // No image displayed when error
            }
        }

        static T LoadDataFromFile<T>(string path) where T : class, new()
        {
            string jsonData = null;
            try
            {
                jsonData = LoadFromFile(path);
                if (string.IsNullOrEmpty(jsonData))
                {
                    return default(T);
                }

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
                return result;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(new Exception("Error loading jsonData from " + path + ". DATA: " + jsonData, ex));
                return default(T);
            }
        }

        static string LoadFromFile(string fullFilePath)
        {
            var fileJson = string.Empty;
            if (!File.Exists(fullFilePath)) return fileJson;

            using (var file = File.OpenText(fullFilePath))
            {
                fileJson = file.ReadToEnd();
                file.Close();
            }

            return fileJson;
        }

        public static void RemoveQuickShareLink()
        {
            var quickShareFile = Path.Combine(LocalStorageFolder, StringResources.QUICKSHARE_LINK);
            if (File.Exists(quickShareFile))
            {
                File.Delete(quickShareFile);
            }
        }

        public static async void SaveResponse(string response, string fileName, string path = null)
        {
            using (var semaphore = new SemaphoreSlim(1, 1))
            {
                await semaphore.WaitAsync();

                var fullFilePath = Path.Combine(path ?? LocalStorageFolder, fileName);
                try
                {
                    if (File.Exists(fullFilePath) && !IsFileInUse(new FileInfo(fullFilePath)))
                    {
                        File.WriteAllText(fullFilePath, response);
                    }
                    else
                    {
                        File.WriteAllText(fullFilePath, response);
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(
                        new Exception("Error in SaveResponse saving " + fileName + " with response " + response, ex));
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public static void SetDataRefreshDate(RefreshItem refreshItem)
        {
            var refreshFileName = Path.Combine(LocalStorageFolder,
                StringResources.BUSIDEX_REFRESH_COOKIE_NAME);
            var lastRefreshFile = LoadData<BusidexRefreshInfo>(refreshFileName) ?? new BusidexRefreshInfo();
            var date = DateTime.Now;

            switch (refreshItem)
            {
                case RefreshItem.MyBusidex:
                {
                    lastRefreshFile.LastMyBusidexRefresh = date;
                    break;
                }
                case RefreshItem.Events:
                {
                    lastRefreshFile.LastEventsRefresh = date;
                    break;
                }
                case RefreshItem.OrganizationList:
                {
                    lastRefreshFile.LastOrganizationListRefresh = date;
                    break;
                }
            }
            SaveResponse(refreshFileName, refreshFileName);
        }

        private static bool IsFileInUse(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                if (!File.Exists(file.FullName))
                {
                    return false;
                }

                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }

            return false;
        }

        public static T GetCachedResult<T>(string fileName) where T : new()
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return new T();
                }

                using (var file = File.OpenText(fileName))
                {
                    var fileJson = file.ReadToEnd();
                    file.Close();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(fileJson);
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return new T();
            }
        }

        public static bool IsImageNameGuid(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            if (val.Length != 40) return false;
            var test = val.Substring(0, 36);

            Guid.TryParse(test, out Guid result);
            return result != null;
        }
    }
}
