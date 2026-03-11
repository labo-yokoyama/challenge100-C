using System.Windows;
using System.Windows.Controls;

namespace MarkdownPreviewer
{
    /// <summary>
    /// WebBrowserコントロールにHTMLコンテンツをバインドするためのヘルパークラスです。
    /// </summary>
    public static class WebBrowserHelper
    {
        /// <summary>
        /// HTMLコンテンツをWebBrowserにバインドするための添付プロパティです。
        /// </summary>
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(WebBrowserHelper), new FrameworkPropertyMetadata(OnHtmlChanged));

        /// <summary>
        /// Html添付プロパティのゲッターです。
        /// </summary>
        /// <param name="d">対象のDependencyObject。</param>
        /// <returns>HTMLコンテンツ。</returns>
        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(DependencyObject d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        /// <summary>
        /// Html添付プロパティのセッターです。
        /// </summary>
        /// <param name="d">対象のDependencyObject。</param>
        /// <param name="value">設定するHTMLコンテンツ。</param>
        public static void SetHtml(DependencyObject d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        /// <summary>
        /// Htmlプロパティが変更されたときに呼び出されます。
        /// </summary>
        /// <param name="d">対象のDependencyObject (WebBrowser)。</param>
        /// <param name="e">イベント引数。</param>
        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = d as WebBrowser;
            if (browser != null)
            {
                string html = e.NewValue as string;
                if (!string.IsNullOrEmpty(html))
                {
                    browser.NavigateToString(html);
                }
                else
                {
                    // 空のコンテンツを表示する場合
                    browser.NavigateToString("about:blank");
                }
            }
        }
    }
}
