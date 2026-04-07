using System;
using System.Windows;
using System.Windows.Input;

namespace JsonNotepad
{
    /// <summary>
    /// ウィンドウのイベントをコマンドにバインドするための添付プロパティを提供します。
    /// </summary>
    public static class WindowBehaviors
    {
        /// <summary>
        /// ContentRendered イベント時に実行するコマンド
        /// </summary>
        public static readonly DependencyProperty ContentRenderedCommandProperty =
            DependencyProperty.RegisterAttached(
                "ContentRenderedCommand",
                typeof(ICommand),
                typeof(WindowBehaviors),
                new PropertyMetadata(null, OnContentRenderedCommandChanged));

        /// <summary>
        /// ContentRenderedCommand を設定します。
        /// </summary>
        /// <param name="element">対象のウィンドウ</param>
        /// <param name="value">実行するコマンド</param>
        public static void SetContentRenderedCommand(Window element, ICommand value)
        {
            element.SetValue(ContentRenderedCommandProperty, value);
        }

        /// <summary>
        /// ContentRenderedCommand を取得します。
        /// </summary>
        /// <param name="element">対象のウィンドウ</param>
        /// <returns>実行するコマンド</returns>
        public static ICommand GetContentRenderedCommand(Window element)
        {
            return (ICommand)element.GetValue(ContentRenderedCommandProperty);
        }

        /// <summary>
        /// プロパティ変更時のイベントハンドラ
        /// </summary>
        private static void OnContentRenderedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window != null)
            {
                window.ContentRendered += OnWindowContentRendered;
            }
        }

        /// <summary>
        /// ウィンドウの ContentRendered イベントハンドラ
        /// </summary>
        private static void OnWindowContentRendered(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            ICommand command = GetContentRenderedCommand(window);

            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }

            // 一度だけ実行されるようにイベントを解除します
            window.ContentRendered -= OnWindowContentRendered;
        }
    }
}