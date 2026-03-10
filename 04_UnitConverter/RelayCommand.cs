using System;
using System.Windows.Input;

namespace UnitConverter
{
    /// <summary>
    /// ICommandを実装するMVVM用コマンドクラス。
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>実行アクション</summary>
        private readonly Action<object> executeAction;
        /// <summary>実行可否判定</summary>
        private readonly Predicate<object> canExecutePredicate;
        /// <summary>
        /// RelayCommandの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="execute">実行アクション</param>
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        /// <summary>
        /// RelayCommandの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="execute">実行アクション</param>
        /// <param name="canExecute">実行可否判定</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            executeAction = execute;
            canExecutePredicate = canExecute;
        }
        /// <summary>
        /// コマンドが実行可能かどうかを判定します。
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (canExecutePredicate == null) return true;
            return canExecutePredicate(parameter);
        }
        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        public void Execute(object parameter)
        {
            executeAction(parameter);
        }
        /// <summary>
        /// コマンドの実行可否が変化したときに発生します。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
