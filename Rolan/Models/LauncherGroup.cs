
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Rolan.Models
{
    public enum GroupType
    {
        Normal,
        Folder
    }

    public class LauncherGroup
    {
        [JsonPropertyName("GroupName")]
        public string GroupName { get; set; } = string.Empty;

        [JsonPropertyName("GroupType")]
        public GroupType GroupType { get; set; } = GroupType.Normal;

        [JsonPropertyName("FolderPath")]
        public string FolderPath { get; set; } = string.Empty;

        [JsonPropertyName("Items")]
        public ObservableCollection<LauncherItem> Items { get; set; } = new ObservableCollection<LauncherItem>();

        [JsonPropertyName("SortOrder")]
        public int SortOrder { get; set; } = 0;

        [JsonPropertyName("Color")]
        public string Color { get; set; } = "#0078D4";
    }
}
