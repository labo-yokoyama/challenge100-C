using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MarkdownPreviewer
{
    /// <summary>
    /// ViewModelの基底クラスで、プロパティ変更通知を実装します。
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティの変更時に発生します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// PropertyChangedイベントを発生させます。
        /// </summary>
        /// <param name="propertyName">変更されたプロパティの名前。</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
