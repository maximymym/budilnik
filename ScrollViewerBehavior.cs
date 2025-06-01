// ScrollViewerBehavior.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace budilnik
{
    public static class ScrollViewerBehavior
    {
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached(
                "VerticalOffset",
                typeof(double),
                typeof(ScrollViewerBehavior),
                new PropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(DependencyObject d, double value) =>
            d.SetValue(VerticalOffsetProperty, value);

        public static double GetVerticalOffset(DependencyObject d) =>
            (double)d.GetValue(VerticalOffsetProperty);

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer sv)
                sv.ScrollToVerticalOffset((double)e.NewValue);
        }

        public static void AnimateTo(this ScrollViewer sv, double to)
        {
            var anim = new DoubleAnimation(to, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            sv.BeginAnimation(VerticalOffsetProperty, anim);
        }
    }
}
