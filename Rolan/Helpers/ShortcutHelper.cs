using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Rolan.Helpers
{
    public static class ShortcutHelper
    {
        [ComImport]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellLinkW
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLink { }

        public static ShortcutInfo ResolveShortcut(string shortcutPath)
        {
            if (!File.Exists(shortcutPath))
                return null;

            IShellLinkW shellLink = (IShellLinkW)new ShellLink();

            try
            {
                ((IPersistFile)shellLink).Load(shortcutPath, 0);

                StringBuilder targetPath = new StringBuilder(1024);
                shellLink.GetPath(targetPath, targetPath.Capacity, out _, 0);

                StringBuilder arguments = new StringBuilder(1024);
                shellLink.GetArguments(arguments, arguments.Capacity);

                StringBuilder workingDir = new StringBuilder(1024);
                shellLink.GetWorkingDirectory(workingDir, workingDir.Capacity);

                StringBuilder iconPath = new StringBuilder(1024);
                shellLink.GetIconLocation(iconPath, iconPath.Capacity, out int iconIndex);

                StringBuilder description = new StringBuilder(1024);
                shellLink.GetDescription(description, description.Capacity);

                return new ShortcutInfo
                {
                    TargetPath = targetPath.ToString(),
                    Arguments = arguments.ToString(),
                    WorkingDirectory = workingDir.ToString(),
                    IconPath = iconPath.ToString(),
                    IconIndex = iconIndex,
                    Description = description.ToString()
                };
            }
            finally
            {
                Marshal.ReleaseComObject(shellLink);
            }
        }

        public static UrlInfo ResolveUrlFile(string urlPath)
        {
            if (!File.Exists(urlPath))
                return null;

            try
            {
                string content = File.ReadAllText(urlPath);
                var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                string url = string.Empty;
                string iconFile = string.Empty;
                string description = string.Empty;

                foreach (var line in lines)
                {
                    if (line.StartsWith("URL=", StringComparison.OrdinalIgnoreCase))
                    {
                        url = line.Substring(4).Trim();
                    }
                    else if (line.StartsWith("IconFile=", StringComparison.OrdinalIgnoreCase))
                    {
                        iconFile = line.Substring(9).Trim();
                    }
                    else if (line.StartsWith("Description=", StringComparison.OrdinalIgnoreCase))
                    {
                        description = line.Substring(12).Trim();
                    }
                }

                return new UrlInfo
                {
                    Url = url,
                    IconFile = iconFile,
                    Description = description
                };
            }
            catch
            {
                return null;
            }
        }

        [ComImport]
        [Guid("0000010B-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersistFile
        {
            void GetClassID(out Guid pClassID);
            [PreserveSig]
            int IsDirty();
            void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, int dwMode);
            void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, bool fRemember);
            void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
            void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
        }

        public class ShortcutInfo
        {
            public string TargetPath { get; set; } = string.Empty;
            public string Arguments { get; set; } = string.Empty;
            public string WorkingDirectory { get; set; } = string.Empty;
            public string IconPath { get; set; } = string.Empty;
            public int IconIndex { get; set; } = 0;
            public string Description { get; set; } = string.Empty;
        }

        public class UrlInfo
        {
            public string Url { get; set; } = string.Empty;
            public string IconFile { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}
