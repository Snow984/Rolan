
using System.Text.Json.Serialization;

namespace Rolan.Models
{
    public enum ItemType
    {
        Application,
        URL,
        Folder,
        Command
    }

    public class LauncherItem
    {
        [JsonPropertyName("ItemType")]
        public ItemType ItemType { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("TargetPath")]
        public string TargetPath { get; set; } = string.Empty;

        [JsonPropertyName("Arguments")]
        public string Arguments { get; set; } = string.Empty;

        [JsonPropertyName("WorkingDirectory")]
        public string WorkingDirectory { get; set; } = string.Empty;

        [JsonPropertyName("IconPath")]
        public string IconPath { get; set; } = string.Empty;

        [JsonPropertyName("IconIndex")]
        public int IconIndex { get; set; } = 0;

        [JsonPropertyName("RunAsAdmin")]
        public bool RunAsAdmin { get; set; } = false;

        [JsonPropertyName("UseRelativePath")]
        public bool UseRelativePath { get; set; } = false;

        [JsonPropertyName("SortOrder")]
        public int SortOrder { get; set; } = 0;
    }
}
