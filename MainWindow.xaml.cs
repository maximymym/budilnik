using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace budilnik
{
    public partial class MainWindow : Window
    {
        private DateTime? alarmTime;
        private string songPath;
        private readonly DispatcherTimer timer;
        private readonly MediaPlayer player;

        private int hours = 0;
        private int minutes = 0;
        private int seconds = 0;

        public MainWindow()
        {
            InitializeComponent();
            HourPicker.ItemsSource = Enumerable.Range(0, 24);
            MinutePicker.ItemsSource = Enumerable.Range(0, 60);
            SecondPicker.ItemsSource = Enumerable.Range(0, 60);

            HourPicker.SelectedIndex = DateTime.Now.Hour;
            MinutePicker.SelectedIndex = DateTime.Now.Minute;
            SecondPicker.SelectedIndex = DateTime.Now.Second;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
            BeginAnimation(Window.OpacityProperty, fade);
        }

        #region Song Selection
        private void SelectSongButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.wav|All Files|*.*",
                Title = "Выберите песню"
            };
            if (dlg.ShowDialog() == true)
            {
                songPath = dlg.FileName;
                SongTextBlock.Text = "Выбрана: " + System.IO.Path.GetFileName(songPath);
            }
        }
        #endregion

        #region Time Increment/Decrement & MouseWheel
        private void ChangeTime(ref int field, int max, int delta, System.Windows.Controls.TextBlock display)
        {
            field = (field + delta + (max + 1)) % (max + 1);
            display.Text = field.ToString("D2");
        }

        private void HourIncrement_Click(object sender, RoutedEventArgs e) => ChangeTime(ref hours, 23, +1, HourTextBlock);
        private void HourDecrement_Click(object sender, RoutedEventArgs e) => ChangeTime(ref hours, 23, -1, HourTextBlock);
        private void MinuteIncrement_Click(object sender, RoutedEventArgs e) => ChangeTime(ref minutes, 59, +1, MinuteTextBlock);
        private void MinuteDecrement_Click(object sender, RoutedEventArgs e) => ChangeTime(ref minutes, 59, -1, MinuteTextBlock);
        private void SecondIncrement_Click(object sender, RoutedEventArgs e) => ChangeTime(ref seconds, 59, +1, SecondTextBlock);
        private void SecondDecrement_Click(object sender, RoutedEventArgs e) => ChangeTime(ref seconds, 59, -1, SecondTextBlock);

        private void Hour_MouseWheel(object sender, MouseWheelEventArgs e) => ChangeTime(ref hours, 23, e.Delta > 0 ? +1 : -1, HourTextBlock);
        private void Minute_MouseWheel(object sender, MouseWheelEventArgs e) => ChangeTime(ref minutes, 59, e.Delta > 0 ? +1 : -1, MinuteTextBlock);
        private void Second_MouseWheel(object sender, MouseWheelEventArgs e) => ChangeTime(ref seconds, 59, e.Delta > 0 ? +1 : -1, SecondTextBlock);

        private void UpdateTimeDisplay()
        {
            HourTextBlock.Text = hours.ToString("D2");
            MinuteTextBlock.Text = minutes.ToString("D2");
            SecondTextBlock.Text = seconds.ToString("D2");
        }
        #endregion

        private void SetAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            alarmTime = new DateTime(now.Year, now.Month, now.Day,
                                     HourPicker.SelectedIndex,
                                     MinutePicker.SelectedIndex,
                                     SecondPicker.SelectedIndex);
            if (alarmTime <= now)
                alarmTime = alarmTime.Value.AddDays(1);

            var aw = new AlarmWindow(alarmTime.Value, songPath);
            aw.Show();
            aw.StartCountdownAndPlay();
            Close();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!alarmTime.HasValue) return;
            var now = DateTime.Now;
            var remaining = alarmTime.Value - now;
            CountdownTextBlock.Text = "До срабатывания: " + remaining.ToString(@"hh\:mm\:ss");
            if (remaining <= TimeSpan.Zero)
            {
                timer.Stop();
                // Close settings window
                this.Close();
                // Show alarm window
                var alarmWindow = new AlarmWindow(alarmTime.Value, songPath);
                alarmWindow.Show();
                alarmWindow.StartCountdownAndPlay();
            }
        }

        private void PlaySong()
        {
            if (string.IsNullOrEmpty(songPath))
            {
                MessageBox.Show("Песня не выбрана!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                player.Open(new Uri(songPath));
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось воспроизвести песню:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}