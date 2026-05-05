
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Rolan.Helpers
{
    public enum EdgePosition
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }

    public static class EdgeSnapHelper
    {
        private const int SnapThreshold = 20;
        private const int AnimationDuration = 300;

        public static EdgePosition GetSnapPosition(Window window)
        {
            if (window == null) return EdgePosition.None;

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            double left = window.Left;
            double top = window.Top;
            double right = left + window.Width;
            double bottom = top + window.Height;

            if (left <= SnapThreshold) return EdgePosition.Left;
            if (right >= screenWidth - SnapThreshold) return EdgePosition.Right;
            if (top <= SnapThreshold) return EdgePosition.Top;
            if (bottom >= screenHeight - SnapThreshold) return EdgePosition.Bottom;

            return EdgePosition.None;
        }

        public static void SnapToEdge(Window window, EdgePosition edge)
        {
            if (window == null || edge == EdgePosition.None) return;

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            double targetX = window.Left;
            double targetY = window.Top;

            switch (edge)
            {
                case EdgePosition.Left:
                    targetX = -window.Width + 5;
                    break;
                case EdgePosition.Right:
                    targetX = screenWidth - 5;
                    break;
                case EdgePosition.Top:
                    targetY = -window.Height + 5;
                    break;
                case EdgePosition.Bottom:
                    targetY = screenHeight - 5;
                    break;
            }

            AnimateWindowPosition(window, targetX, targetY);
        }

        public static void ShowFromEdge(Window window, EdgePosition edge)
        {
            if (window == null || edge == EdgePosition.None) return;

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            double targetX = window.Left;
            double targetY = window.Top;

            switch (edge)
            {
                case EdgePosition.Left:
                    targetX = 0;
                    break;
                case EdgePosition.Right:
                    targetX = screenWidth - window.Width;
                    break;
                case EdgePosition.Top:
                    targetY = 0;
                    break;
                case EdgePosition.Bottom:
                    targetY = screenHeight - window.Height;
                    break;
            }

            AnimateWindowPosition(window, targetX, targetY);
        }

        public static bool IsMouseNearEdge(EdgePosition edge, double threshold = 10)
        {
            System.Windows.Point mousePos = System.Windows.Forms.Control.MousePosition;
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            switch (edge)
            {
                case EdgePosition.Left:
                    return mousePos.X <= threshold;
                case EdgePosition.Right:
                    return mousePos.X >= screenWidth - threshold;
                case EdgePosition.Top:
                    return mousePos.Y <= threshold;
                case EdgePosition.Bottom:
                    return mousePos.Y >= screenHeight - threshold;
                default:
                    return false;
            }
        }

        private static void AnimateWindowPosition(Window window, double targetX, double targetY)
        {
            DoubleAnimation animX = new DoubleAnimation(window.Left, targetX, TimeSpan.FromMilliseconds(AnimationDuration));
            DoubleAnimation animY = new DoubleAnimation(window.Top, targetY, TimeSpan.FromMilliseconds(AnimationDuration));

            animX.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
            animY.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };

            window.BeginAnimation(Window.LeftProperty, animX);
            window.BeginAnimation(Window.TopProperty, animY);
        }
    }
}
