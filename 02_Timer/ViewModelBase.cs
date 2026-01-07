using System;
using System.ComponentModel;

namespace Timer
{
    /// <summary>
    /// ViewModel の基底クラスを表します。プロパティ変更通知の機能を提供します。
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ変更時に発生するイベントです。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 指定されたプロパティ名の変更を通知します。
        /// </summary>
        /// <param name="propertyName">変更されたプロパティ名。</param>
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}


