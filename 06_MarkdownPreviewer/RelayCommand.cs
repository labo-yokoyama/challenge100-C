using System;
using System.Windows.Input;

namespace MarkdownPreviewer
{
    /// <summary>
    /// ViewModelのアクションをViewにバインドするためのICommandの実装です。
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        /// <summary>
        /// 新しいコマンドを作成します。
        /// </summary>
        /// <param name="execute">コマンドの実行ロジック。</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// 新しいコマンドを作成します。
        /// </summary>
        /// <param name="execute">コマンドの実行ロジック。</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを判断するロジック。</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// コマンドが現在実行可能かどうかを決定するメソッド。
        /// </summary>
        /// <param name="parameter">コマンドで使用されるデータ。コマンドがデータを必要としない場合は、このオブジェクトをnullに設定できます。</param>
        /// <returns>このコマンドが実行できる場合はtrue、それ以外の場合はfalse。</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        /// <summary>
        /// CanExecuteの状態が変更されたときに発生します。
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドが呼び出されたときに実行されるメソッド。
        /// </summary>
        /// <param name="parameter">コマンドで使用されるデータ。コマンドがデータを必要としない場合は、このオブジェクトをnullに設定できます。</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
