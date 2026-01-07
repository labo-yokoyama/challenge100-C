using System;
using System.Windows.Input;

namespace Timer
{
    /// <summary>
    /// デリゲートを使用してコマンド処理を実装するためのクラスです。
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// コマンド実行時に呼び出されるアクションです。
        /// </summary>
        private readonly Action<object> executeAction;

        /// <summary>
        /// コマンドを実行できるかどうかを判定するデリゲートです。
        /// </summary>
        private readonly Predicate<object> canExecutePredicate;

        /// <summary>
        /// コマンドの実行可否が変化したときに発生するイベントです。
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="execute">コマンド実行時に呼び出されるアクション。</param>
        /// <param name="canExecute">コマンドを実行できるかどうかを判定するデリゲート。</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            executeAction = execute;
            canExecutePredicate = canExecute;
        }

        /// <summary>
        /// コマンドが実行可能かどうかを取得します。
        /// </summary>
        /// <param name="parameter">コマンドパラメーター。</param>
        /// <returns>実行可能な場合は true、それ以外の場合は false。</returns>
        public bool CanExecute(object parameter)
        {
            if (canExecutePredicate == null)
            {
                return true;
            }

            return canExecutePredicate(parameter);
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="parameter">コマンドパラメーター。</param>
        public void Execute(object parameter)
        {
            executeAction(parameter);
        }

        /// <summary>
        /// コマンドの実行可否が変化したことを通知します。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}


