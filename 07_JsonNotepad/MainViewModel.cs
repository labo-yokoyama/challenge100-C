using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;

namespace JsonNotepad
{
    /// <summary>
    /// メインウィンドウのビジネスロジックを管理する ViewModel です。
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
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
        public ICommand NewCommand { get; private set; }

        /// <summary>
        /// 保存コマンド
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// 削除コマンド
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// 初期化コマンド
        /// </summary>
        public ICommand InitializeCommand { get; private set; }

        /// <summary>
        /// MainViewModel の新しいインスタンスを初期化します。
        /// </summary>
        public MainViewModel()
        {
            allNotes = new ObservableCollection<NoteItem>();
            filteredNotes = new ObservableCollection<NoteItem>();
            notesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notes");

            NewCommand = new RelayCommand(CreateNewNote);
            SaveCommand = new RelayCommand(SaveNote);
            DeleteCommand = new RelayCommand(DeleteNote, CanDelete);
            InitializeCommand = new RelayCommand(Initialize);
        }

        /// <summary>
        /// アプリケーションの初期化処理を行います。
        /// </summary>
        public void Initialize()
        {
            InitializeDirectory();
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
                selectedNote = value;
                RaisePropertyChanged("SelectedNote");
                LoadSelectedNoteToEditor();
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
        /// 保存先ディレクトリの存在確認と作成を行います。
        /// </summary>
        private void InitializeDirectory()
        {
            if (!Directory.Exists(notesDirectory))
            {
                Directory.CreateDirectory(notesDirectory);
            }
        }

        /// <summary>
        /// 保存されているすべてのメモを読み込みます。
        /// </summary>
        private void LoadAllNotes()
        {
            allNotes.Clear();
            string[] files = Directory.GetFiles(notesDirectory, "*.json");
            foreach (string file in files)
            {
                try
                {
                    string json = File.ReadAllText(file, Encoding.UTF8);
                    NoteItem note = JsonConvert.DeserializeObject<NoteItem>(json);
                    if (note != null)
                    {
                        allNotes.Add(note);
                    }
                }
                catch (Exception)
                {
                    // ファイル読み込み失敗時はスキップ
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
                if (string.IsNullOrEmpty(searchText) || note.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
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
            SelectedNote = null;
            EditTitle = string.Empty;
            EditContent = string.Empty;
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

            string filePath = Path.Combine(notesDirectory, note.Title + ".json");
            
            // 以前のファイル名と異なる場合は古いファイルを削除（リネーム対応）
            if (selectedNote != null && selectedNote.Title != editTitle)
            {
                string oldPath = Path.Combine(notesDirectory, selectedNote.Title + ".json");
                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }
                allNotes.Remove(selectedNote);
            }
            else if (selectedNote != null)
            {
                allNotes.Remove(selectedNote);
            }

            try
            {
                string json = JsonConvert.SerializeObject(note);
                File.WriteAllText(filePath, json, Encoding.UTF8);
                
                allNotes.Add(note);
                ApplyFilter();
                SelectedNote = note;
                MessageBox.Show("保存しました。", "JsonNotepad", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存に失敗しました: " + ex.Message, "JsonNotepad", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("タイトルを入力してください", "JsonNotepad", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        MessageBox.Show(string.Format("\"{0}\"はファイル名に使えません", c), "JsonNotepad", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// メモを削除できるかどうかを判断します。
        /// </summary>
        /// <returns>削除可能な場合は true</returns>
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

            if (MessageBox.Show("このメモを削除しますか？", "JsonNotepad", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string filePath = Path.Combine(notesDirectory, selectedNote.Title + ".json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                allNotes.Remove(selectedNote);
                ApplyFilter();
                CreateNewNote();
            }
        }
    }
}