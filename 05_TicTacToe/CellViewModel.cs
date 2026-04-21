namespace TicTacToe
{
    /// <summary>
    /// 盤面の1マスを表すビュー・モデルです。
    /// 表示文字、ハイライト、操作可否などを公開します。
    /// </summary>
    public class CellViewModel : ViewModelBase
    {
        /// <summary>
        /// マスのインデックス（0..8）を保持するフィールドです。
        /// </summary>
        private int index;

        /// <summary>
        /// マスに表示する記号（\"\"/\"○\"/\"×\"）を保持するフィールドです。
        /// </summary>
        private string markText;

        /// <summary>
        /// 勝利ラインのハイライト状態を保持するフィールドです。
        /// </summary>
        private bool isHighlighted;

        /// <summary>
        /// マスがクリック可能かどうかを保持するフィールドです。
        /// </summary>
        private bool isCellEnabled;

        /// <summary>
        /// CellViewModel クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="cellIndex">マスのインデックス（0..8）。</param>
        public CellViewModel(int cellIndex)
        {
            index = cellIndex;
            markText = string.Empty;
            isHighlighted = false;
            isCellEnabled = true;
        }

        /// <summary>
        /// マスのインデックス（0..8）を取得します。
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// マスに表示する記号（\"\"/\"○\"/\"×\"）を取得または設定します。
        /// </summary>
        public string MarkText
        {
            get { return markText; }
            set
            {
                if (markText == value)
                {
                    return;
                }

                markText = value;
                OnPropertyChanged("MarkText");
            }
        }

        /// <summary>
        /// 勝利ラインのハイライト状態を取得または設定します。
        /// </summary>
        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set
            {
                if (isHighlighted == value)
                {
                    return;
                }

                isHighlighted = value;
                OnPropertyChanged("IsHighlighted");
            }
        }

        /// <summary>
        /// マスがクリック可能かどうかを取得または設定します。
        /// </summary>
        public bool IsCellEnabled
        {
            get { return isCellEnabled; }
            set
            {
                if (isCellEnabled == value)
                {
                    return;
                }

                isCellEnabled = value;
                OnPropertyChanged("IsCellEnabled");
            }
        }
    }
}

