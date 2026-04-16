using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Livet;
using Livet.Commands;
using Livet.Messaging;

namespace JsonNotepad
{
    /// <summary>
    /// メインウィンドウのビジネスロジックを管理する ViewModel です。
    /// </summary>
    public class MainViewModel : ViewModel
    {
        private readonly JsonNotepadModel _model;
        private ObservableCollection<NoteItem> allNotes;
        private ObservableCollection<NoteItem> filteredNotes;
        private NoteItem selectedNote;
        private string searchText;
        private string editTitle;
        private string editContent;
        private string notesDirectory;

        /// <summary>
        /// 新規作成コマンド
        /// </summary>
        public ViewModelCommand NewCommand { get; private set; }

        /// <summary>
        /// 保存コマンド
        /// </summary>
        public ViewModelCommand SaveCommand { get; private set; }

        /// <summary>
        /// 削除コマンド
        /// </summary>
        public ViewModelCommand DeleteCommand { get; private set; }

        /// <summary>
        /// コピーコマンド
        /// </summary>
        public ViewModelCommand CopyCommand { get; private set; }

        /// <summary>
        /// 初期化コマンド
        /// </summary>
        public ViewModelCommand InitializeCommand { get; private set; }

        /// <summary>
        /// MainViewModel の新しいインスタンスを初期化します。
        /// </summary>
        public MainViewModel()
        {
            _model = new JsonNotepadModel();
            allNotes = new ObservableCollection<NoteItem>();
            filteredNotes = new ObservableCollection<NoteItem>();
            notesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notes");

            NewCommand = new ViewModelCommand(CreateNewNote);
            SaveCommand = new ViewModelCommand(SaveNote);
            DeleteCommand = new ViewModelCommand(DeleteNote, CanDelete);
            CopyCommand = new ViewModelCommand(CopyNote);
            InitializeCommand = new ViewModelCommand(Initialize);
        }

        /// <summary>
        /// アプリケーションの初期化処理を行います。
        /// </summary>
        public void Initialize()
        {
            _model.InitializeDirectory(notesDirectory);
            LoadAllNotes();
        }

        /// <summary>
        /// 表示用のメモ一覧
        /// </summary>
        public ObservableCollection<NoteItem> FilteredNotes
        {
            get { return filteredNotes; }
        }

        /// <summary>
        /// 選択されているメモ
        /// </summary>
        public NoteItem SelectedNote
        {
            get { return selectedNote; }
            set
            {
                if (selectedNote == value) return;
                selectedNote = value;
                RaisePropertyChanged("SelectedNote");

                // 選択されたメモの内容をエディタに反映（またはクリア）
                LoadSelectedNoteToEditor();
                
                // 選択が変更されたら削除ボタンの活性状態を更新
                if (DeleteCommand != null) DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 検索文字列
        /// </summary>
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                RaisePropertyChanged("SearchText");
                ApplyFilter();
            }
        }

        /// <summary>
        /// 編集中のタイトル
        /// </summary>
        public string EditTitle
        {
            get { return editTitle; }
            set
            {
                editTitle = value;
                RaisePropertyChanged("EditTitle");
            }
        }

        /// <summary>
        /// 編集中の内容
        /// </summary>
        public string EditContent
        {
            get { return editContent; }
            set
            {
                editContent = value;
                RaisePropertyChanged("EditContent");
            }
        }

        /// <summary>
        /// 保存されているすべてのメモを読み込みます。
        /// </summary>
        private void LoadAllNotes()
        {
            allNotes.Clear();
            string[] files = _model.GetNoteFiles(notesDirectory);
            foreach (string file in files)
            {
                NoteItem note = _model.LoadFile(file);
                if (note != null)
                {
                    allNotes.Add(note);
                }
            }
            ApplyFilter();
        }

        /// <summary>
        /// 検索文字列に基づいて一覧をフィルタリングします。
        /// </summary>
        private void ApplyFilter()
        {
            filteredNotes.Clear();
            foreach (NoteItem note in allNotes)
            {
                // note.Title が null の場合に IndexOf で落ちるのを防ぐ
                if (string.IsNullOrEmpty(searchText) || (note.Title != null && note.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    filteredNotes.Add(note);
                }
            }
        }

        /// <summary>
        /// 編集エリアをクリアして新規作成状態にします。
        /// </summary>
        private void CreateNewNote()
        {
            // 編集内容を先にクリアしてから選択を解除する（順序が重要です）
            EditTitle = string.Empty;
            EditContent = string.Empty;
            SelectedNote = null;
        }

        /// <summary>
        /// 選択されたメモの内容を編集エリアに読み込みます。
        /// </summary>
        private void LoadSelectedNoteToEditor()
        {
            if (selectedNote != null)
            {
                EditTitle = selectedNote.Title;
                EditContent = selectedNote.Content;
            }
            else
            {
                EditTitle = string.Empty;
                EditContent = string.Empty;
            }
        }

        /// <summary>
        /// 編集中の内容をクリップボードにコピーします。
        /// </summary>
        private void CopyNote()
        {
            if (!string.IsNullOrEmpty(editContent))
            {
                Clipboard.SetText(editContent);
            }
        }

        /// <summary>
        /// 現在編集中のメモをファイルに保存します。
        /// </summary>
        private void SaveNote()
        {
            if (!ValidateTitle())
            {
                return;
            }

            NoteItem note = new NoteItem();
            note.Title = editTitle;
            note.Content = editContent;
            note.LastModified = DateTime.Now;

            // 以前のファイル名と異なる場合は古いファイルを削除（リネーム対応）し、リストから一旦削除
            if (selectedNote != null && selectedNote.Title != editTitle)
            {
                _model.DeleteFile(notesDirectory, selectedNote.Title);
                allNotes.Remove(selectedNote);
            }
            else if (selectedNote != null)
            {
                allNotes.Remove(selectedNote);
            }

            try
            {
                _model.SaveFile(notesDirectory, note);
                
                allNotes.Add(note);
                ApplyFilter();
                SelectedNote = note;
                
                Messenger.Raise(new InformationMessage("保存しました。", "JsonNotepad", MessageBoxImage.Information, "InfoMessage"));
            }
            catch (Exception ex)
            {
                Messenger.Raise(new InformationMessage("保存に失敗しました: " + ex.Message, "JsonNotepad", MessageBoxImage.Error, "InfoMessage"));
            }
        }

        /// <summary>
        /// 入力されたタイトルの妥当性を検証します。
        /// </summary>
        /// <returns>妥当な場合は true、それ以外は false</returns>
        private bool ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(editTitle))
            {
                Messenger.Raise(new InformationMessage("タイトルを入力してください", "JsonNotepad", MessageBoxImage.Warning, "InfoMessage"));
                return false;
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in editTitle)
            {
                char[] currentInvalidChars = invalidChars;
                for (int i = 0; i < currentInvalidChars.Length; i++)
                {
                    if (c == currentInvalidChars[i])
                    {
                        Messenger.Raise(new InformationMessage(string.Format("\"{0}\"はファイル名に使えません", c), "JsonNotepad", MessageBoxImage.Warning, "InfoMessage"));
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// メモを削除できるかどうかを判断します。
        /// </summary>
        private bool CanDelete()
        {
            return selectedNote != null;
        }

        /// <summary>
        /// 現在選択されているメモをファイルとリストから削除します。
        /// </summary>
        private void DeleteNote()
        {
            if (selectedNote == null) return;

            var message = new ConfirmationMessage("このメモを削除しますか？", "削除の確認", MessageBoxImage.Question, MessageBoxButton.YesNo, "ConfirmMessage");
            Messenger.GetResponse(message);

            if (message.Response == true)
            {
                _model.DeleteFile(notesDirectory, selectedNote.Title);

                allNotes.Remove(selectedNote);
                ApplyFilter();
                CreateNewNote();
            }
        }
    }
}