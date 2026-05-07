using Rolan.Helpers;
using Rolan.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rolan
{
    public partial class MainWindow : Window
    {
        private AppData _appData;
        private EdgePosition _currentEdgePosition = EdgePosition.None;
        private bool _isAutoHiding = false;
        private bool _isMouseInWindow = false;
        private bool _isSearchMode = false;
        private string _searchText = string.Empty;
        private LauncherItem? _contextMenuTarget;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _appData = DataStorage.LoadData();
            DataContext = _appData;
        }

        private void ToggleWindowVisibility()
        {
            if (Visibility == Visibility.Visible)
            {
                HideWindow();
            }
            else
            {
                ShowWindow();
            }
        }

        private void ShowWindow()
        {
            Visibility = Visibility.Visible;
            Activate();
            if (_currentEdgePosition != EdgePosition.None)
            {
                EdgeSnapHelper.ShowFromEdge(this, _currentEdgePosition);
                _isAutoHiding = false;
            }
        }

        private void HideWindow()
        {
            if (_currentEdgePosition != EdgePosition.None)
            {
                EdgeSnapHelper.SnapToEdge(this, _currentEdgePosition);
                _isAutoHiding = true;
            }
            else
            {
                Visibility = Visibility.Hidden;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HotkeyHelper.HotkeyPressed += HotkeyHelper_HotkeyPressed;
            HotkeyHelper.RegisterHotkey(this, _appData.Settings.HotKey);
            
            UpdateTransparency();
            UpdateLayout();
            
            StartEdgeMonitoring();
        }

        private void HotkeyHelper_HotkeyPressed(object? sender, EventArgs e)
        {
            ToggleWindowVisibility();
        }

        private void UpdateTransparency()
        {
            Opacity = _appData.Settings.Transparency / 100.0;
        }

        private void UpdateLayout()
        {
            switch (_appData.Settings.Layout)
            {
                case LayoutMode.Horizontal:
                    Width = 600;
                    Height = 150;
                    break;
                case LayoutMode.Vertical:
                    Width = 300;
                    Height = 450;
                    break;
                case LayoutMode.SinglePage:
                    Width = 400;
                    Height = 500;
                    break;
            }
        }

        private void StartEdgeMonitoring()
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) =>
            {
                if (_isAutoHiding && !_isMouseInWindow)
                {
                    if (EdgeSnapHelper.IsMouseNearEdge(_currentEdgePosition))
                    {
                        ShowWindow();
                    }
                }
            };
            timer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            HideWindow();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
                _currentEdgePosition = EdgeSnapHelper.GetSnapPosition(this);
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseInWindow = true;
            if (_isAutoHiding)
            {
                ShowWindow();
            }
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseInWindow = false;
            if (_currentEdgePosition != EdgePosition.None && _appData.Settings.AutoHide)
            {
                HideWindow();
            }
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0)
                return;

            var currentGroup = TabControl.SelectedItem as LauncherGroup;
            if (currentGroup == null)
                return;

            foreach (string file in files)
            {
                AddFileToGroup(file, currentGroup);
            }

            DataStorage.SaveData(_appData);
        }

        private void GroupContent_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void GroupContent_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0)
                return;

            var currentGroup = TabControl.SelectedItem as LauncherGroup;
            if (currentGroup == null)
                return;

            foreach (string file in files)
            {
                AddFileToGroup(file, currentGroup);
            }

            DataStorage.SaveData(_appData);
        }

        private void AddFileToGroup(string filePath, LauncherGroup group)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            
            switch (extension)
            {
                case ".lnk":
                    AddShortcutItem(filePath, group);
                    break;
                case ".url":
                    AddUrlItem(filePath, group);
                    break;
                case ".exe":
                case ".bat":
                case ".cmd":
                    AddApplicationItem(filePath, group);
                    break;
                default:
                    if (Directory.Exists(filePath))
                    {
                        AddFolderItem(filePath, group);
                    }
                    else
                    {
                        AddApplicationItem(filePath, group);
                    }
                    break;
            }
        }

        private void AddShortcutItem(string lnkPath, LauncherGroup group)
        {
            var info = ShortcutHelper.ResolveShortcut(lnkPath);
            if (info == null)
                return;

            group.Items.Add(new LauncherItem
            {
                ItemType = ItemType.Application,
                Name = Path.GetFileNameWithoutExtension(lnkPath),
                TargetPath = info.TargetPath,
                Arguments = info.Arguments,
                WorkingDirectory = info.WorkingDirectory,
                IconPath = string.IsNullOrEmpty(info.IconPath) ? info.TargetPath : info.IconPath,
                IconIndex = info.IconIndex,
                SortOrder = group.Items.Count
            });
        }

        private void AddUrlItem(string urlPath, LauncherGroup group)
        {
            var info = ShortcutHelper.ResolveUrlFile(urlPath);
            if (info == null || string.IsNullOrEmpty(info.Url))
                return;

            group.Items.Add(new LauncherItem
            {
                ItemType = ItemType.URL,
                Name = Path.GetFileNameWithoutExtension(urlPath),
                TargetPath = info.Url,
                IconPath = info.IconFile,
                SortOrder = group.Items.Count
            });
        }

        private void AddApplicationItem(string exePath, LauncherGroup group)
        {
            group.Items.Add(new LauncherItem
            {
                ItemType = ItemType.Application,
                Name = Path.GetFileNameWithoutExtension(exePath),
                TargetPath = exePath,
                WorkingDirectory = Path.GetDirectoryName(exePath) ?? string.Empty,
                IconPath = exePath,
                SortOrder = group.Items.Count
            });
        }

        private void AddFolderItem(string folderPath, LauncherGroup group)
        {
            group.Items.Add(new LauncherItem
            {
                ItemType = ItemType.Folder,
                Name = Path.GetFileName(folderPath),
                TargetPath = folderPath,
                SortOrder = group.Items.Count
            });
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                HideWindow();
            }
            else if (e.Key == Key.Tab && Keyboard.Modifiers == ModifierKeys.Control)
            {
                int currentIndex = TabControl.SelectedIndex;
                int nextIndex = (currentIndex + 1) % _appData.Groups.Count;
                TabControl.SelectedIndex = nextIndex;
            }
            else if (!_isSearchMode && e.Key != Key.Tab && !Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                _isSearchMode = true;
                SearchBox.Visibility = Visibility.Visible;
                SearchBox.Focus();
                if (e.Key != Key.Back && e.Key != Key.Delete)
                {
                    _searchText = new string(new[] { KeyInterop.KeyFromVirtualKey(KeyInterop.VirtualKeyFromKey(e.Key)) });
                    SearchBox.Text = _searchText;
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = SearchBox.Text;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void AddTabBtn_Click(object sender, RoutedEventArgs e)
        {
            var newGroup = new LauncherGroup
            {
                GroupName = "新分组",
                GroupType = GroupType.Normal,
                SortOrder = _appData.Groups.Count,
                Color = GetRandomColor()
            };
            _appData.Groups.Add(newGroup);
            TabControl.SelectedItem = newGroup;
            DataStorage.SaveData(_appData);
        }

        private void DeleteTabBtn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var group = button?.Tag as LauncherGroup;
            if (group != null)
            {
                _appData.Groups.Remove(group);
                DataStorage.SaveData(_appData);
            }
        }

        private string GetRandomColor()
        {
            var colors = new[] { "#0078D4", "#FF6B6B", "#4ECDC4", "#45B7D1", "#96CEB4", "#FFEAA7", "#DDA0DD", "#98D8C8" };
            return colors[new Random().Next(colors.Length)];
        }

        private void ItemBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var item = border?.Tag as LauncherItem;
            if (item != null)
            {
                LauncherHelper.LaunchItem(item);
                if (_appData.Settings.HideOnLaunch)
                {
                    HideWindow();
                }
            }
        }

        private void ItemBorder_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            _contextMenuTarget = border?.Tag as LauncherItem;
            
            var contextMenu = FindResource("ItemContextMenu") as ContextMenu;
            if (contextMenu != null && _contextMenuTarget != null)
            {
                contextMenu.PlacementTarget = border;
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
        }

        private void MenuItem_Open(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTarget != null)
            {
                LauncherHelper.LaunchItem(_contextMenuTarget);
                if (_appData.Settings.HideOnLaunch)
                {
                    HideWindow();
                }
            }
        }

        private void MenuItem_RunAsAdmin(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTarget != null)
            {
                bool original = _contextMenuTarget.RunAsAdmin;
                _contextMenuTarget.RunAsAdmin = true;
                LauncherHelper.LaunchItem(_contextMenuTarget);
                _contextMenuTarget.RunAsAdmin = original;
                if (_appData.Settings.HideOnLaunch)
                {
                    HideWindow();
                }
            }
        }

        private void MenuItem_OpenLocation(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTarget != null)
            {
                LauncherHelper.OpenFileLocation(_contextMenuTarget);
            }
        }

        private void MenuItem_Copy(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTarget != null)
            {
                Clipboard.SetText(_contextMenuTarget.TargetPath);
            }
        }

        private void MenuItem_Delete(object sender, RoutedEventArgs e)
        {
            if (_contextMenuTarget != null)
            {
                var currentGroup = TabControl.SelectedItem as LauncherGroup;
                if (currentGroup != null)
                {
                    currentGroup.Items.Remove(_contextMenuTarget);
                    DataStorage.SaveData(_appData);
                }
            }
        }

        private void LayoutBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (_appData.Settings.Layout)
            {
                case LayoutMode.Vertical:
                    _appData.Settings.Layout = LayoutMode.Horizontal;
                    break;
                case LayoutMode.Horizontal:
                    _appData.Settings.Layout = LayoutMode.SinglePage;
                    break;
                case LayoutMode.SinglePage:
                    _appData.Settings.Layout = LayoutMode.Vertical;
                    break;
            }
            UpdateLayout();
            DataStorage.SaveData(_appData);
        }

        private void IconSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            _appData.Settings.IconSize = _appData.Settings.IconSize == 48 ? 32 : 48;
            DataStorage.SaveData(_appData);
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_appData.Settings);
            settingsWindow.ShowDialog();
            UpdateTransparency();
            UpdateLayout();
            HotkeyHelper.RegisterHotkey(this, _appData.Settings.HotKey);
            DataStorage.SaveData(_appData);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            HideWindow();
        }

        private void GroupNameTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsReadOnly = false;
                textBox.SelectAll();
                textBox.Focus();
                e.Handled = true;
            }
        }

        private void GroupNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsReadOnly = true;
                DataStorage.SaveData(_appData);
            }
        }

        private void GroupNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && e.Key == Key.Enter)
            {
                textBox.IsReadOnly = true;
                DataStorage.SaveData(_appData);
                e.Handled = true;
            }
            else if (textBox != null && e.Key == Key.Escape)
            {
                textBox.IsReadOnly = true;
                e.Handled = true;
            }
        }
    }
}
