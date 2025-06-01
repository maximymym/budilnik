using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;

namespace budilnik
{
    public partial class AlarmWindow : Window
    {
        private readonly DateTime alarmTime;
        private readonly string songPath;
        private readonly DispatcherTimer countdownTimer;
        private readonly MediaPlayer player;

        public AlarmWindow(DateTime alarmTime, string songPath)
        {
            InitializeComponent();
            this.alarmTime = alarmTime;
            this.songPath = songPath;

            countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            countdownTimer.Tick += CountdownTimer_Tick;
            player = new MediaPlayer();
        }

        public void StartCountdownAndPlay()
        {
            AlarmTimeLabel.Text = alarmTime.ToString("h:mm tt");
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            var remaining = alarmTime - DateTime.Now;
            if (remaining <= TimeSpan.Zero)
            {
                CountdownLabel.Text = "00:00:00";
                countdownTimer.Stop();
                PlaySong();
            }
            else
            {
                CountdownLabel.Text = remaining.ToString(@"hh\:mm\:ss");
            }
        }

        private void PlaySong()
        {
            if (!string.IsNullOrEmpty(songPath))
            {
                player.Open(new Uri(songPath));
                player.Play();
            }
            else
            {
                MessageBox.Show("Песня не выбрана!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SnoozeButton_Click(object sender, RoutedEventArgs e)
        {
            // Добавляем 10 минут к alarmTime и перезапускаем таймер
            var newTime = DateTime.Now.AddMinutes(10);
            countdownTimer.Stop();
            Show();
            new AlarmWindow(newTime, songPath).StartCountdownAndPlay();
            Close();
        }

        private void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            Close();
        }
    }
}
