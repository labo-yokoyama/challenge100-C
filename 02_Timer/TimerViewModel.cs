using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Timer
{
    /// <summary>
    /// カウントダウンタイマーの動作と状態を管理する ViewModel を表します。
    /// </summary>
    public class TimerViewModel : ViewModelBase
    {
        /// <summary>
        /// スタートボタンに表示するテキストです。
        /// </summary>
        private const string StartText = "スタート";

        /// <summary>
        /// ストップボタンに表示するテキストです。
        /// </summary>
        private const string StopText = "ストップ";

        /// <summary>
        /// 合計秒数を保持します。
        /// </summary>
        private int totalSeconds;

        /// <summary>
        /// カウントダウン中かどうかを示します。
        /// </summary>
        private bool isCountingDown;

        /// <summary>
        /// 設定ボタンが有効かどうかを示します。
        /// </summary>
        private bool areAdjustButtonsEnabled;

        /// <summary>
        /// スタート／ストップボタンに表示する文字列です。
        /// </summary>
        private string startStopButtonText;

        /// <summary>
        /// カウントダウン用のタイマーです。
        /// </summary>
        private readonly DispatcherTimer dispatcherTimer;

        /// <summary>
        /// 分の十の位を増加させるコマンドです。
        /// </summary>
        public ICommand IncreaseMinuteTensCommand { get; private set; }

        /// <summary>
        /// 分の十の位を減少させるコマンドです。
        /// </summary>
        public ICommand DecreaseMinuteTensCommand { get; private set; }

        /// <summary>
        /// 分の一の位を増加させるコマンドです。
        /// </summary>
        public ICommand IncreaseMinuteOnesCommand { get; private set; }

        /// <summary>
        /// 分の一の位を減少させるコマンドです。
        /// </summary>
        public ICommand DecreaseMinuteOnesCommand { get; private set; }

        /// <summary>
        /// 秒の十の位を増加させるコマンドです。
        /// </summary>
        public ICommand IncreaseSecondTensCommand { get; private set; }

        /// <summary>
        /// 秒の十の位を減少させるコマンドです。
        /// </summary>
        public ICommand DecreaseSecondTensCommand { get; private set; }

        /// <summary>
        /// 秒の一の位を増加させるコマンドです。
        /// </summary>
        public ICommand IncreaseSecondOnesCommand { get; private set; }

        /// <summary>
        /// 秒の一の位を減少させるコマンドです。
        /// </summary>
        public ICommand DecreaseSecondOnesCommand { get; private set; }

        /// <summary>
        /// カウントダウンを開始または停止するコマンドです。
        /// </summary>
        public ICommand StartStopCommand { get; private set; }

        /// <summary>
        /// タイマーをリセットするコマンドです。
        /// </summary>
        public ICommand ResetCommand { get; private set; }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public TimerViewModel()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += OnTimerTick;

            totalSeconds = 0;
            isCountingDown = false;
            areAdjustButtonsEnabled = true;
            startStopButtonText = StartText;

            InitializeCommands();
            RaiseAllDigitPropertiesChanged();
        }

        /// <summary>
        /// 分の十の位を取得します。
        /// </summary>
        public int MinuteTens
        {
            get
            {
                int minutes = totalSeconds / 60;
                return minutes / 10;
            }
        }

        /// <summary>
        /// 分の一の位を取得します。
        /// </summary>
        public int MinuteOnes
        {
            get
            {
                int minutes = totalSeconds / 60;
                return minutes % 10;
            }
        }

        /// <summary>
        /// 秒の十の位を取得します。
        /// </summary>
        public int SecondTens
        {
            get
            {
                int seconds = totalSeconds % 60;
                return seconds / 10;
            }
        }

        /// <summary>
        /// 秒の一の位を取得します。
        /// </summary>
        public int SecondOnes
        {
            get
            {
                int seconds = totalSeconds % 60;
                return seconds % 10;
            }
        }

        /// <summary>
        /// カウントダウン中かどうかを取得または設定します。
        /// </summary>
        public bool IsCountingDown
        {
            get { return isCountingDown; }
            private set
            {
                if (isCountingDown != value)
                {
                    isCountingDown = value;
                    OnPropertyChanged("IsCountingDown");

                    AreAdjustButtonsEnabled = !isCountingDown;
                    RaiseAllCommandCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// 設定ボタンが有効かどうかを取得または設定します。
        /// </summary>
        public bool AreAdjustButtonsEnabled
        {
            get { return areAdjustButtonsEnabled; }
            private set
            {
                if (areAdjustButtonsEnabled != value)
                {
                    areAdjustButtonsEnabled = value;
                    OnPropertyChanged("AreAdjustButtonsEnabled");
                }
            }
        }

        /// <summary>
        /// スタート／ストップボタンに表示する文字列を取得または設定します。
        /// </summary>
        public string StartStopButtonText
        {
            get { return startStopButtonText; }
            private set
            {
                if (startStopButtonText != value)
                {
                    startStopButtonText = value;
                    OnPropertyChanged("StartStopButtonText");
                }
            }
        }

        /// <summary>
        /// コマンドを初期化します。
        /// </summary>
        private void InitializeCommands()
        {
            IncreaseMinuteTensCommand = new RelayCommand(
                delegate(object o) { ChangeMinuteTens(1); },
                delegate(object o) { return AreAdjustButtonsEnabled; });

            DecreaseMinuteTensCommand = new RelayCommand(
                delegate(object o) { ChangeMinuteTens(-1); },
                delegate(object o) { return AreAdjustButtonsEnabled; });

            IncreaseMinuteOnesCommand = new RelayCommand(
                delegate(object o) { ChangeMinuteOnes(1); },
                delegate(object o) { return AreAdjustButtonsEnabled; });

            DecreaseMinuteOnesCommand = new RelayCommand(
                delegate(object o) { ChangeMinuteOnes(-1); },
                delegate(object o) { return AreAdjustButtonsEnabled; });

            IncreaseSecondTensCommand = new RelayCommand(
                delegate(object o) { ChangeSecondTens(1); },
                delegate(object o) { return AreAdjustButtonsEnabled; });

            DecreaseSecondTensCommand = new RelayCommand(
                delegate(object o) { ChangeSecondTens(-1); },
                delegate(object o) { return AreAdjustButtonsEnabled; });

            IncreaseSecondOnesCommand = new RelayCommand(
                delegate(object o) { ChangeSecondOnes(1); },
                delegate(object o) { return AreAdjustButtonsEnabled; });

            DecreaseSecondOnesCommand = new RelayCommand(
                delegate(object o) { ChangeSecondOnes(-1); },
                delegate(object o) { return AreAdjustButtonsEnabled; });

            StartStopCommand = new RelayCommand(
                delegate(object o) { ToggleStartStop(); },
                delegate(object o) { return totalSeconds > 0 || IsCountingDown; });

            ResetCommand = new RelayCommand(
                delegate(object o) { Reset(); },
                delegate(object o) { return AreAdjustButtonsEnabled; });
        }

        /// <summary>
        /// すべての桁プロパティの変更通知を行います。
        /// </summary>
        private void RaiseAllDigitPropertiesChanged()
        {
            OnPropertyChanged("MinuteTens");
            OnPropertyChanged("MinuteOnes");
            OnPropertyChanged("SecondTens");
            OnPropertyChanged("SecondOnes");
        }

        /// <summary>
        /// すべてのコマンドの CanExecuteChanged を発生させます。
        /// </summary>
        private void RaiseAllCommandCanExecuteChanged()
        {
            RaiseCanExecuteChanged(IncreaseMinuteTensCommand);
            RaiseCanExecuteChanged(DecreaseMinuteTensCommand);
            RaiseCanExecuteChanged(IncreaseMinuteOnesCommand);
            RaiseCanExecuteChanged(DecreaseMinuteOnesCommand);
            RaiseCanExecuteChanged(IncreaseSecondTensCommand);
            RaiseCanExecuteChanged(DecreaseSecondTensCommand);
            RaiseCanExecuteChanged(IncreaseSecondOnesCommand);
            RaiseCanExecuteChanged(DecreaseSecondOnesCommand);
            RaiseCanExecuteChanged(StartStopCommand);
            RaiseCanExecuteChanged(ResetCommand);
        }

        /// <summary>
        /// 指定されたコマンドの CanExecuteChanged を発生させます。
        /// </summary>
        /// <param name="command">対象のコマンド。</param>
        private void RaiseCanExecuteChanged(ICommand command)
        {
            RelayCommand relay = command as RelayCommand;
            if (relay != null)
            {
                relay.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 分の十の位を変更します。
        /// </summary>
        /// <param name="delta">増減値。</param>
        private void ChangeMinuteTens(int delta)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            int tens = minutes / 10;
            int ones = minutes % 10;

            tens += delta;
            if (tens < 0)
            {
                tens = 0;
            }
            if (tens > 5)
            {
                tens = 5;
            }

            minutes = tens * 10 + ones;
            if (minutes > 59)
            {
                minutes = 59;
            }

            totalSeconds = minutes * 60 + seconds;
            RaiseAllDigitPropertiesChanged();
            RaiseAllCommandCanExecuteChanged();
        }

        /// <summary>
        /// 分の一の位を変更します。
        /// </summary>
        /// <param name="delta">増減値。</param>
        private void ChangeMinuteOnes(int delta)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            int tens = minutes / 10;
            int ones = minutes % 10;

            ones += delta;
            if (ones < 0)
            {
                ones = 0;
            }
            if (ones > 9)
            {
                ones = 9;
            }

            minutes = tens * 10 + ones;
            if (minutes > 59)
            {
                minutes = 59;
            }

            totalSeconds = minutes * 60 + seconds;
            RaiseAllDigitPropertiesChanged();
            RaiseAllCommandCanExecuteChanged();
        }

        /// <summary>
        /// 秒の十の位を変更します。
        /// </summary>
        /// <param name="delta">増減値。</param>
        private void ChangeSecondTens(int delta)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            int tens = seconds / 10;
            int ones = seconds % 10;

            tens += delta;
            if (tens < 0)
            {
                tens = 0;
            }
            if (tens > 5)
            {
                tens = 5;
            }

            seconds = tens * 10 + ones;
            if (seconds > 59)
            {
                seconds = 59;
            }

            totalSeconds = minutes * 60 + seconds;
            RaiseAllDigitPropertiesChanged();
            RaiseAllCommandCanExecuteChanged();
        }

        /// <summary>
        /// 秒の一の位を変更します。
        /// </summary>
        /// <param name="delta">増減値。</param>
        private void ChangeSecondOnes(int delta)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            int tens = seconds / 10;
            int ones = seconds % 10;

            ones += delta;
            if (ones < 0)
            {
                ones = 0;
            }
            if (ones > 9)
            {
                ones = 9;
            }

            seconds = tens * 10 + ones;
            if (seconds > 59)
            {
                seconds = 59;
            }

            totalSeconds = minutes * 60 + seconds;
            RaiseAllDigitPropertiesChanged();
            RaiseAllCommandCanExecuteChanged();
        }

        /// <summary>
        /// スタート／ストップボタン押下時の処理を行います。
        /// </summary>
        private void ToggleStartStop()
        {
            if (IsCountingDown)
            {
                dispatcherTimer.Stop();
                IsCountingDown = false;
                StartStopButtonText = StartText;
            }
            else
            {
                if (totalSeconds <= 0)
                {
                    return;
                }

                dispatcherTimer.Start();
                IsCountingDown = true;
                StartStopButtonText = StopText;
            }
        }

        /// <summary>
        /// タイマーをリセットします。
        /// </summary>
        private void Reset()
        {
            if (IsCountingDown)
            {
                return;
            }

            totalSeconds = 0;
            RaiseAllDigitPropertiesChanged();
            RaiseAllCommandCanExecuteChanged();
        }

        /// <summary>
        /// タイマーの Tick イベント時の処理を行います。
        /// </summary>
        /// <param name="sender">イベント送信元。</param>
        /// <param name="e">イベント引数。</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (totalSeconds > 0)
            {
                totalSeconds--;
                RaiseAllDigitPropertiesChanged();
                RaiseAllCommandCanExecuteChanged();

                if (totalSeconds == 0)
                {
                    dispatcherTimer.Stop();
                    IsCountingDown = false;
                    StartStopButtonText = StartText;
                    MessageBox.Show("時間です！");
                }
            }
            else
            {
                dispatcherTimer.Stop();
                IsCountingDown = false;
                StartStopButtonText = StartText;
            }
        }
    }
}


