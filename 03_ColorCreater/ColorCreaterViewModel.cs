using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorCreater
{
    /// <summary>
    /// カラークリエーターの動作と状態を管理する ViewModel を表します。
    /// </summary>
    public class ColorCreaterViewModel : ViewModelBase
    {
        /// <summary>
        /// 赤色の値を保持します（0-255）。
        /// </summary>
        private int redValue;

        /// <summary>
        /// 緑色の値を保持します（0-255）。
        /// </summary>
        private int greenValue;

        /// <summary>
        /// 青色の値を保持します（0-255）。
        /// </summary>
        private int blueValue;

        /// <summary>
        /// 現在の色を表すブラシを保持します。
        /// </summary>
        private SolidColorBrush currentColor;

        /// <summary>
        /// 色情報の16進数文字列を保持します。
        /// </summary>
        private string colorHexString;

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public ColorCreaterViewModel()
        {
            redValue = 0;
            greenValue = 0;
            blueValue = 0;
            colorHexString = "#000000";
            currentColor = new SolidColorBrush(Color.FromRgb((byte)redValue, (byte)greenValue, (byte)blueValue));

            CopyColorCommand = new RelayCommand(
                delegate(object o) { CopyToClipboard(); },
                delegate(object o) { return true; });
        }

        /// <summary>
        /// 赤色の値を取得または設定します（0-255）。
        /// </summary>
        public int RedValue
        {
            get { return redValue; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > 255)
                {
                    value = 255;
                }

                if (redValue != value)
                {
                    redValue = value;
                    OnPropertyChanged("RedValue");
                    OnPropertyChanged("RedHex");
                    UpdateColor();
                }
            }
        }

        /// <summary>
        /// 緑色の値を取得または設定します（0-255）。
        /// </summary>
        public int GreenValue
        {
            get { return greenValue; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > 255)
                {
                    value = 255;
                }

                if (greenValue != value)
                {
                    greenValue = value;
                    OnPropertyChanged("GreenValue");
                    OnPropertyChanged("GreenHex");
                    UpdateColor();
                }
            }
        }

        /// <summary>
        /// 青色の値を取得または設定します（0-255）。
        /// </summary>
        public int BlueValue
        {
            get { return blueValue; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > 255)
                {
                    value = 255;
                }

                if (blueValue != value)
                {
                    blueValue = value;
                    OnPropertyChanged("BlueValue");
                    OnPropertyChanged("BlueHex");
                    UpdateColor();
                }
            }
        }

        /// <summary>
        /// 赤色の16進数表現を取得します（例: "FF"）。
        /// </summary>
        public string RedHex
        {
            get
            {
                return redValue.ToString("X2");
            }
        }

        /// <summary>
        /// 緑色の16進数表現を取得します（例: "FF"）。
        /// </summary>
        public string GreenHex
        {
            get
            {
                return greenValue.ToString("X2");
            }
        }

        /// <summary>
        /// 青色の16進数表現を取得します（例: "FF"）。
        /// </summary>
        public string BlueHex
        {
            get
            {
                return blueValue.ToString("X2");
            }
        }

        /// <summary>
        /// 現在の色を表すブラシを取得します。
        /// </summary>
        public SolidColorBrush CurrentColor
        {
            get { return currentColor; }
        }

        /// <summary>
        /// 色情報の16進数文字列を取得します（例: "#FF0000"）。
        /// </summary>
        public string ColorHexString
        {
            get { return colorHexString; }
        }

        /// <summary>
        /// 色情報文字列をクリップボードにコピーするコマンドを取得します。
        /// </summary>
        public ICommand CopyColorCommand { get; private set; }

        /// <summary>
        /// RGB値から色と色情報文字列を更新します。
        /// </summary>
        private void UpdateColor()
        {
            Color color = Color.FromRgb((byte)redValue, (byte)greenValue, (byte)blueValue);
            currentColor = new SolidColorBrush(color);
            OnPropertyChanged("CurrentColor");

            colorHexString = string.Format("#{0}{1}{2}", RedHex, GreenHex, BlueHex);
            OnPropertyChanged("ColorHexString");
        }

        /// <summary>
        /// 色情報文字列をクリップボードにコピーします。
        /// </summary>
        private void CopyToClipboard()
        {
            Clipboard.SetText(colorHexString);
        }
    }
}
