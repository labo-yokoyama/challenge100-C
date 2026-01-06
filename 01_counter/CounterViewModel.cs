using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace counter
{
    /// <summary>
    /// カウンター画面用のビュー・モデルクラスです。
    /// カウント値と増減用コマンドを公開し、ビューとデータバインディングされます。
    /// </summary>
    public class CounterViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// カウンターの現在値を保持するフィールドです。
        /// </summary>
        private int _count;

        /// <summary>
        /// 画面に表示されるカウンターの値を取得・設定します。
        /// 値が変更された場合、プロパティ変更通知を発行します。
        /// </summary>
        public int Count
        {
            get { return _count; }
            set
            {
                if (_count == value)
                {
                    return;
                }

                _count = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// カウンターを1増加させるコマンドを取得します。
        /// </summary>
        public ICommand IncrementCommand { get; private set; }

        /// <summary>
        /// カウンターを1減少させるコマンドを取得します。
        /// </summary>
        public ICommand DecrementCommand { get; private set; }

        /// <summary>
        /// CounterViewModel クラスの新しいインスタンスを生成します。
        /// 増加・減少コマンドの実装を初期化します。
        /// </summary>
        public CounterViewModel()
        {
            IncrementCommand = new RelayCommand(delegate(object o) { Count++; });
            DecrementCommand = new RelayCommand(delegate(object o) { Count--; });
        }

        /// <summary>
        /// プロパティ変更時に発行されるイベントです。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 指定されたプロパティ名の変更を通知します。
        /// 呼び出し元メンバー名を自動的に取得して通知します。
        /// </summary>
        /// <param name="propertyName">変更されたプロパティ名。</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// デリゲートを用いて ICommand を実装する汎用コマンドクラスです。
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// コマンド実行時に呼び出される処理です。
        /// </summary>
        private readonly Action<object> _execute;

        /// <summary>
        /// コマンド実行可否を判定する処理です。null の場合は常に実行可能とみなします。
        /// </summary>
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// 実行処理のみを指定して RelayCommand の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンド実行時に呼び出される処理。</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// 実行処理と実行可否判定処理を指定して RelayCommand の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンド実行時に呼び出される処理。</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを判定する処理。</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドが現在実行可能かどうかを判定します。
        /// </summary>
        /// <param name="parameter">コマンドパラメーター。</param>
        /// <returns>実行可能な場合は true、それ以外は false。</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// コマンドに関連付けられた処理を実行します。
        /// </summary>
        /// <param name="parameter">コマンドパラメーター。</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// コマンドの実行可否が変化したことを通知するイベントです。
        /// CommandManager.RequerySuggested と連動します。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

