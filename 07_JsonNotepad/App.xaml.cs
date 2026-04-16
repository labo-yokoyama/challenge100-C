using System;
using System.Windows;
using System.Windows.Threading;
using Livet;

namespace JsonNotepad
{
    /// <summary>
    /// アプリケーションの管理。
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// アプリケーション起動時の処理です。
        /// 未処理例外ハンドラを登録します。
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Livet の DispatcherHelper を初期化します（コマンドの実行制御に必要です）
            DispatcherHelper.UIDispatcher = Dispatcher.CurrentDispatcher;

            // UIスレッドでの未処理例外をキャッチ
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // UIスレッド以外（Taskや別スレッドなど）での未処理例外をキャッチ
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// UIスレッドで発生した未処理例外を処理します。
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ReportException(e.Exception, "UI Dispatcher");
            
            // true に設定すると、アプリケーションの強制終了を回避しようと試みます。
            e.Handled = true;
        }

        /// <summary>
        /// UIスレッド以外で発生した未処理例外を処理します。
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ReportException(e.ExceptionObject as Exception, "AppDomain");
        }

        /// <summary>
        /// 例外の内容をメッセージボックスで表示します。
        /// </summary>
        private void ReportException(Exception ex, string source)
        {
            string errorMsg = "予期せぬエラーが発生しました。\n\n";
            if (ex != null)
            {
                errorMsg += ex.ToString();
            }
            else
            {
                errorMsg += "詳細不明のエラーです。";
            }

            MessageBox.Show(errorMsg, "JsonNotepad 致命的なエラー (" + source + ")", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
