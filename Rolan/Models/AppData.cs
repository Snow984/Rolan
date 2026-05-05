
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Rolan.Models
{
    public class AppData
    {
        [JsonPropertyName("Groups")]
        public ObservableCollection<LauncherGroup> Groups { get; set; } = new ObservableCollection<LauncherGroup>();

        [JsonPropertyName("Settings")]
        public AppSettings Settings { get; set; } = new AppSettings();

        [JsonPropertyName("Version")]
        public string Version { get; set; } = "1.0.0";
    }
}
