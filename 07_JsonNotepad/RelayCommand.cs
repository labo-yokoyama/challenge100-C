using System;
using System.Windows.Input;

namespace JsonNotepad
{
    /// <summary>
    /// デリゲートを受け取る ICommand 実装クラスです。
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        /// <summary>
        /// コマンドの初期化
        /// </summary>
        /// <param name="execute">実行ロジック</param>
        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        /// <summary>
        /// コマンドの初期化
        /// </summary>
        /// <param name="execute">実行ロジック</param>
        /// <param name="canExecute">実行可能状態を返すデリゲート</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// コマンドを実行できるかどうかが変更されたときに発生します。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドを実行できるかどうかを判断します。
        /// </summary>
        /// <param name="parameter">パラメータ（使用しません）</param>
        /// <returns>実行可能な場合は true</returns>
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="parameter">パラメータ（使用しません）</param>
        public void Execute(object parameter)
        {
            execute();
        }

        /// <summary>
        /// CanExecuteChanged イベントを手動で発生させます。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}