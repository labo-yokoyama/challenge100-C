using Markdig;
using System.Windows;
using System.Windows.Input;

namespace MarkdownPreviewer
{
    /// <summary>
    /// メインウィンドウのViewModelで、アプリケーションの主要なロジックを管理します。
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private string markdownText;
        private string htmlText;

        /// <summary>
        /// MainViewModelの新しいインスタンスを初期化します。
        /// </summary>
        public MainViewModel()
        {
            CopyCommand = new RelayCommand(CopyMarkdownToClipboard, CanCopyMarkdown);
            MarkdownText = "# Hello, Markdown!\n\nこんにちは、マークダウン！";
        }

        /// <summary>
        /// ユーザーが入力したMarkdownテキストを取得または設定します。
        /// </summary>
        public string MarkdownText
        {
            get { return markdownText; }
            set
            {
                if (markdownText != value)
                {
                    markdownText = value;
                    OnPropertyChanged("MarkdownText");
                    ConvertMarkdownToHtml();
                }
            }
        }

        /// <summary>
        /// Markdownから変換されたHTMLテキストを取得または設定します。
        /// </summary>
        public string HtmlText
        {
            get { return htmlText; }
            private set
            {
                if (htmlText != value)
                {
                    htmlText = value;
                    OnPropertyChanged("HtmlText");
                }
            }
        }

        /// <summary>
        /// 入力されたMarkdownをクリップボードにコピーするコマンドを取得します。
        /// </summary>
        public ICommand CopyCommand { get; private set; }

        /// <summary>
        /// MarkdownテキストをHTMLに変換し、HtmlTextプロパティを更新します。
        /// </summary>
        private void ConvertMarkdownToHtml()
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            string htmlBody = Markdown.ToHtml(MarkdownText, pipeline);
            HtmlText = "<html><head><meta charset='UTF-8'></head><body>" + htmlBody + "</body></html>";
        }

        /// <summary>
        /// Markdownテキストをクリップボードにコピーします。
        /// </summary>
        /// <param name="parameter">コマンドパラメータ（未使用）。</param>
        private void CopyMarkdownToClipboard(object parameter)
        {
            Clipboard.SetText(MarkdownText);
        }

        /// <summary>
        /// コピーコマンドが実行可能かどうかを判断します。
        /// </summary>
        /// <param name="parameter">コマンドパラメータ（未使用）。</param>
        /// <returns>Markdownテキストが空でない場合はtrue、それ以外の場合はfalse。</returns>
        private bool CanCopyMarkdown(object parameter)
        {
            return !string.IsNullOrEmpty(MarkdownText);
        }
    }
}
