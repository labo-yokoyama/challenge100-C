using System;

namespace JsonNotepad
{
    /// <summary>
    /// メモの情報を保持するクラスです。
    /// </summary>
    public class NoteItem
    {
        /// <summary>
        /// メモのタイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// メモの内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 最終更新日時
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}