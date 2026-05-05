using System.Text.Json.Serialization;

namespace Rolan.Models
{
    public enum LayoutMode
    {
        Horizontal,
        Vertical,
        SinglePage
    }

    public enum ThemeMode
    {
        Light,
        Dark,
        System
    }

    public class AppSettings
    {
        [JsonPropertyName("HotKey")]
        public string HotKey { get; set; } = "Alt+`";

        [JsonPropertyName("Theme")]
        public ThemeMode Theme { get; set; } = ThemeMode.Light;

        [JsonPropertyName("Layout")]
        public LayoutMode Layout { get; set; } = LayoutMode.Vertical;

        [JsonPropertyName("AutoHide")]
        public bool AutoHide { get; set; } = true;

        [JsonPropertyName("Transparency")]
        public int Transparency { get; set; } = 80;

        [JsonPropertyName("IconSize")]
        public int IconSize { get; set; } = 48;

        [JsonPropertyName("AutoStart")]
        public bool AutoStart { get; set; } = false;

        [JsonPropertyName("HideOnLaunch")]
        public bool HideOnLaunch { get; set; } = true;

        [JsonPropertyName("StayOnTop")]
        public bool StayOnTop { get; set; } = false;

        [JsonPropertyName("SearchDelay")]
        public int SearchDelay { get; set; } = 200;
    }
}
