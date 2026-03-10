using System;
using System.Windows.Input;

namespace TicTacToe
{
    /// <summary>
    /// デリゲートを用いて ICommand を実装する汎用コマンドクラスです。
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// コマンド実行時に呼び出される処理です。
        /// </summary>
        private readonly Action<object> execute;

        /// <summary>
        /// コマンド実行可否を判定する処理です。null の場合は常に実行可能とみなします。
        /// </summary>
        private readonly Func<object, bool> canExecute;

        /// <summary>
        /// 実行処理のみを指定して RelayCommand の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="executeAction">コマンド実行時に呼び出される処理。</param>
        public RelayCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
        }

        /// <summary>
        /// 実行処理と実行可否判定処理を指定して RelayCommand の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="executeAction">コマンド実行時に呼び出される処理。</param>
        /// <param name="canExecuteFunc">コマンドが実行可能かどうかを判定する処理。</param>
        public RelayCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }

            execute = executeAction;
            canExecute = canExecuteFunc;
        }

        /// <summary>
        /// コマンドが現在実行可能かどうかを判定します。
        /// </summary>
        /// <param name="parameter">コマンドパラメーター。</param>
        /// <returns>実行可能な場合は true、それ以外は false。</returns>
        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        /// <summary>
        /// コマンドに関連付けられた処理を実行します。
        /// </summary>
        /// <param name="parameter">コマンドパラメーター。</param>
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        /// <summary>
        /// コマンドの実行可否が変化したことを通知するイベントです。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

