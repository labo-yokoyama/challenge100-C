using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace UnitConverter
{
    /// <summary>
    /// MainWindow用のViewModel。
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>カテゴリ一覧</summary>
        public ObservableCollection<string> Categories { get; private set; }
        /// <summary>選択中カテゴリ</summary>
        public string SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                if (selectedCategory != value)
                {
                    selectedCategory = value;
                    OnPropertyChanged("SelectedCategory");
                    UpdateUnits();
                    LeftValue = string.Empty;
                    RightValue = string.Empty;
                    WarningMessage = string.Empty;
                }
            }
        }
        /// <summary>左側の単位一覧</summary>
        public ObservableCollection<string> LeftUnits { get; private set; }
        /// <summary>右側の単位一覧</summary>
        public ObservableCollection<string> RightUnits { get; private set; }
        /// <summary>左側の選択単位</summary>
        public string SelectedLeftUnit
        {
            get { return selectedLeftUnit; }
            set { if (selectedLeftUnit != value) { selectedLeftUnit = value; OnPropertyChanged("SelectedLeftUnit"); } }
        }
        /// <summary>右側の選択単位</summary>
        public string SelectedRightUnit
        {
            get { return selectedRightUnit; }
            set { if (selectedRightUnit != value) { selectedRightUnit = value; OnPropertyChanged("SelectedRightUnit"); } }
        }
        /// <summary>左側の値</summary>
        public string LeftValue
        {
            get { return leftValue; }
            set
            {
                if (leftValue != value)
                {
                    leftValue = value;
                    OnPropertyChanged("LeftValue");
                    WarningMessage = string.Empty;
                }
            }
        }
        /// <summary>右側の値</summary>
        public string RightValue
        {
            get { return rightValue; }
            set
            {
                if (rightValue != value)
                {
                    rightValue = value;
                    OnPropertyChanged("RightValue");
                    WarningMessage = string.Empty;
                }
            }
        }
        /// <summary>警告メッセージ</summary>
        public string WarningMessage
        {
            get { return warningMessage; }
            set { if (warningMessage != value) { warningMessage = value; OnPropertyChanged("WarningMessage"); } }
        }
        /// <summary>右変換コマンド</summary>
        public ICommand ConvertRightCommand { get; private set; }
        /// <summary>左変換コマンド</summary>
        public ICommand ConvertLeftCommand { get; private set; }

        string selectedCategory;
        string selectedLeftUnit;
        string selectedRightUnit;
        string leftValue;
        string rightValue;
        string warningMessage;
        UnitConverterModel model;

        /// <summary>
        /// MainWindowViewModelの新しいインスタンスを初期化します。
        /// </summary>
        public MainWindowViewModel()
        {
            Categories = new ObservableCollection<string>(new string[] { "長さ", "重さ", "温度" });
            LeftUnits = new ObservableCollection<string>();
            RightUnits = new ObservableCollection<string>();
            model = new UnitConverterModel();
            ConvertRightCommand = new RelayCommand(delegate(object o) { ConvertLeftToRight(); });
            ConvertLeftCommand = new RelayCommand(delegate(object o) { ConvertRightToLeft(); });
            SelectedCategory = Categories[0];
        }
        /// <summary>
        /// 単位リストをカテゴリに応じて更新します。
        /// </summary>
        private void UpdateUnits()
        {
            LeftUnits.Clear();
            RightUnits.Clear();
            string[] units = model.GetUnits(SelectedCategory);
            foreach (string u in units)
            {
                LeftUnits.Add(u);
                RightUnits.Add(u);
            }
            SelectedLeftUnit = units.Length > 0 ? units[0] : null;
            SelectedRightUnit = units.Length > 1 ? units[1] : null;
        }
        /// <summary>
        /// 左→右の変換を実行します。
        /// </summary>
        private void ConvertLeftToRight()
        {
            double v;
            if (!double.TryParse(LeftValue, out v))
            {
                WarningMessage = "数字を入力してください";
                return;
            }
            if (v <= 0)
            {
                WarningMessage = "0より大きい数字を入力してください";
                return;
            }
            double result = model.Convert(SelectedCategory, SelectedLeftUnit, SelectedRightUnit, v);
            RightValue = result.ToString();
        }
        /// <summary>
        /// 右→左の変換を実行します。
        /// </summary>
        private void ConvertRightToLeft()
        {
            double v;
            if (!double.TryParse(RightValue, out v))
            {
                WarningMessage = "数字を入力してください";
                return;
            }
            if (v <= 0)
            {
                WarningMessage = "0より大きい数字を入力してください";
                return;
            }
            double result = model.Convert(SelectedCategory, SelectedRightUnit, SelectedLeftUnit, v);
            LeftValue = result.ToString();
        }
    }
}
