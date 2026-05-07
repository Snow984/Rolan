using System;
using System.Windows;
using System.Windows.Threading;

namespace Rolan
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // 全局异常处理
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            
            try
            {
                // 确保窗口可见
                MainWindow = new MainWindow();
                MainWindow.Show();
                MainWindow.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"应用程序启动失败:\n\n" +
                    $"错误信息: {ex.Message}\n\n" +
                    $"堆栈跟踪: {ex.StackTrace}",
                    "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"发生未处理的异常:\n\n" +
                    $"错误信息: {ex.Message}\n\n" +
                    $"堆栈跟踪: {ex.StackTrace}",
                    "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"发生未处理的异常:\n\n" +
                $"错误信息: {e.Exception.Message}\n\n" +
                $"堆栈跟踪: {e.Exception.StackTrace}",
                "错误",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
