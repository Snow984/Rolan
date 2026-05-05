using Rolan.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace Rolan.Helpers
{
    public static class LauncherHelper
    {
        public static void LaunchItem(LauncherItem item)
        {
            try
            {
                string targetPath = ResolvePath(item.TargetPath);

                if (string.IsNullOrEmpty(targetPath))
                    return;

                ProcessStartInfo psi = new ProcessStartInfo();

                switch (item.ItemType)
                {
                    case ItemType.Application:
                        psi.FileName = targetPath;
                        psi.Arguments = item.Arguments;
                        psi.WorkingDirectory = string.IsNullOrEmpty(item.WorkingDirectory) 
                            ? Path.GetDirectoryName(targetPath) 
                            : ResolvePath(item.WorkingDirectory);
                        psi.Verb = item.RunAsAdmin ? "runas" : string.Empty;
                        break;

                    case ItemType.URL:
                        psi.FileName = targetPath;
                        psi.UseShellExecute = true;
                        break;

                    case ItemType.Folder:
                        psi.FileName = "explorer.exe";
                        psi.Arguments = $"\"{targetPath}\"";
                        break;

                    case ItemType.Command:
                        psi.FileName = "cmd.exe";
                        psi.Arguments = $"/c {targetPath}";
                        break;
                }

                Process.Start(psi);
            }
            catch
            {
            }
        }

        public static void OpenFileLocation(LauncherItem item)
        {
            try
            {
                string targetPath = ResolvePath(item.TargetPath);
                string directory = Path.GetDirectoryName(targetPath);
                
                if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                {
                    Process.Start("explorer.exe", $"\"{directory}\"");
                }
            }
            catch
            {
            }
        }

        private static string ResolvePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            string resolved = Environment.ExpandEnvironmentVariables(path);

            if (Path.IsPathRooted(resolved))
                return resolved;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, resolved);
        }

        public static void ToggleAutoStart(bool enable)
        {
            try
            {
                string appPath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
                string startupPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Startup), 
                    "Rolan.lnk");

                if (enable && !string.IsNullOrEmpty(appPath))
                {
                    CreateShortcut(startupPath, appPath);
                }
                else if (File.Exists(startupPath))
                {
                    File.Delete(startupPath);
                }
            }
            catch
            {
            }
        }

        private static void CreateShortcut(string shortcutPath, string targetPath)
        {
            try
            {
                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);
                dynamic shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.Save();
            }
            catch
            {
            }
        }
    }
}
