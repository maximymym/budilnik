// WheelPicker.xaml.cs
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace budilnik
{
    public partial class WheelPicker : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource),
                                        typeof(IEnumerable),
                                        typeof(WheelPicker),
                                        new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex),
                                        typeof(int),
                                        typeof(WheelPicker),
                                        new PropertyMetadata(0, OnSelectedIndexChanged));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public WheelPicker()
        {
            InitializeComponent();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (WheelPicker)d;
            ctl.itemsPanel.Children.Clear();
            if (e.NewValue is IEnumerable seq)
            {
                foreach (var item in seq)
                {
                    var tb = new TextBlock
                    {
                        Text = item.ToString(),
                        FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                        FontSize = 72,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Height = 80
                    };
                    ctl.itemsPanel.Children.Add(tb);
                }
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (WheelPicker)d;
            double offset = ctl.SelectedIndex * 80;
            var anim = new DoubleAnimation(offset, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            ctl.scroller.BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, anim);
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (itemsPanel.Children.Count == 0) return;
            int max = itemsPanel.Children.Count - 1;
            int delta = e.Delta > 0 ? -1 : +1;
            SelectedIndex = (SelectedIndex + delta + max + 1) % (max + 1);
        }
    }
}
