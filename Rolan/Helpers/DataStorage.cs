using Rolan.Models;
using System.IO;
using System.Text.Json;

namespace Rolan.Helpers
{
    public static class DataStorage
    {
        private static readonly string DataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "Rolan");
        
        private static readonly string DataFilePath = Path.Combine(DataFolder, "data.json");
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static AppData LoadData()
        {
            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }

            if (!File.Exists(DataFilePath))
            {
                return CreateDefaultData();
            }

            try
            {
                string json = File.ReadAllText(DataFilePath);
                return JsonSerializer.Deserialize<AppData>(json, JsonOptions) ?? CreateDefaultData();
            }
            catch
            {
                return CreateDefaultData();
            }
        }

        public static void SaveData(AppData data)
        {
            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }

            string json = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(DataFilePath, json);
        }

        private static AppData CreateDefaultData()
        {
            var data = new AppData
            {
                Version = "1.0.0",
                Settings = new AppSettings()
            };

            return data;
        }

        public static string GetDataFolderPath()
        {
            return DataFolder;
        }
    }
}
