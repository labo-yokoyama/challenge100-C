using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace TicTacToe
{
    /// <summary>
    /// 三目並べ画面全体のビュー・モデルです。
    /// 盤面状態、手番、コマンド、勝敗判定、CPU手（2秒後）を管理します。
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 盤面（9マス）を保持するフィールドです。
        /// 'O'（ユーザー）、'X'（CPU）、' '（空）を使用します。
        /// </summary>
        private char[] board;

        /// <summary>
        /// 盤面表示用のセルコレクションを保持するフィールドです。
        /// </summary>
        private ObservableCollection<CellViewModel> cells;

        /// <summary>
        /// 状態メッセージを保持するフィールドです。
        /// </summary>
        private string statusMessage;

        /// <summary>
        /// ゲームが決着済みかどうかを保持するフィールドです。
        /// </summary>
        private bool isGameOver;

        /// <summary>
        /// CPUの手番待ち（2秒待機中）かどうかを保持するフィールドです。
        /// </summary>
        private bool isWaitingForCpu;

        /// <summary>
        /// CPU手を2秒遅延させるためのタイマーです。
        /// </summary>
        private DispatcherTimer cpuTimer;

        /// <summary>
        /// 勝利ライン定義（8通り）です。各行は3つのインデックスを持ちます。
        /// </summary>
        private int[][] winLines;

        /// <summary>
        /// ランダム選択に使用する乱数生成器です。
        /// </summary>
        private Random random;

        /// <summary>
        /// MainViewModel クラスの新しいインスタンスを生成します。
        /// 盤面、セル、コマンド、タイマーを初期化し、ゲーム開始状態にします。
        /// </summary>
        public MainViewModel()
        {
            board = new char[9];
            cells = new ObservableCollection<CellViewModel>();
            winLines = CreateWinLines();
            random = new Random();

            int i;
            for (i = 0; i < 9; i++)
            {
                cells.Add(new CellViewModel(i));
            }

            StartGameCommand = new RelayCommand(delegate(object o) { StartGame(); });
            CellClickCommand = new RelayCommand(OnCellClick, CanCellClick);

            cpuTimer = new DispatcherTimer();
            cpuTimer.Interval = TimeSpan.FromSeconds(2);
            cpuTimer.Tick += OnCpuTimerTick;

            StartGame();
        }

        /// <summary>
        /// 盤面のセル一覧を取得します。
        /// </summary>
        public ObservableCollection<CellViewModel> Cells
        {
            get { return cells; }
        }

        /// <summary>
        /// 画面上部に表示する状態メッセージを取得または設定します。
        /// </summary>
        public string StatusMessage
        {
            get { return statusMessage; }
            private set
            {
                if (statusMessage == value)
                {
                    return;
                }

                statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }

        /// <summary>
        /// ゲームを開始（リセット）するコマンドを取得します。
        /// </summary>
        public ICommand StartGameCommand { get; private set; }

        /// <summary>
        /// マスをクリックしたときに実行されるコマンドを取得します。
        /// </summary>
        public ICommand CellClickCommand { get; private set; }

        /// <summary>
        /// ゲームを初期状態に戻します。
        /// 盤面の記号とハイライトを消し、ユーザー先手で開始します。
        /// </summary>
        /// <returns>戻り値はありません。</returns>
        private void StartGame()
        {
            if (cpuTimer.IsEnabled)
            {
                cpuTimer.Stop();
            }

            int i;
            for (i = 0; i < 9; i++)
            {
                board[i] = ' ';
                cells[i].MarkText = string.Empty;
                cells[i].IsHighlighted = false;
            }

            isGameOver = false;
            isWaitingForCpu = false;
            StatusMessage = "あなたの番です。任意の枠をクリックしてください";
            UpdateCellEnabledStates();
        }

        /// <summary>
        /// セルクリックが可能かどうかを判定します。
        /// </summary>
        /// <param name="parameter">セルのインデックス（0..8）。</param>
        /// <returns>クリック可能な場合は true、それ以外は false。</returns>
        private bool CanCellClick(object parameter)
        {
            if (isGameOver)
            {
                return false;
            }

            if (isWaitingForCpu)
            {
                return false;
            }

            int index;
            if (!TryGetIndex(parameter, out index))
            {
                return false;
            }

            return board[index] == ' ';
        }

        /// <summary>
        /// セルがクリックされたときの処理を行います。ユーザーの○を配置します。
        /// </summary>
        /// <param name="parameter">セルのインデックス（0..8）。</param>
        /// <returns>戻り値はありません。</returns>
        private void OnCellClick(object parameter)
        {
            int index;
            if (!TryGetIndex(parameter, out index))
            {
                return;
            }

            if (!CanCellClick(index))
            {
                return;
            }

            PlaceMark(index, 'O');
            if (EvaluateAfterMove('O'))
            {
                return;
            }

            isWaitingForCpu = true;
            StatusMessage = "コンピューターの番です。少しお待ちください";
            UpdateCellEnabledStates();

            cpuTimer.Stop();
            cpuTimer.Start();
        }

        /// <summary>
        /// CPU手番タイマーの Tick を処理します。CPUの×を配置します。
        /// </summary>
        /// <param name="sender">イベント送信元。</param>
        /// <param name="e">イベント引数。</param>
        /// <returns>戻り値はありません。</returns>
        private void OnCpuTimerTick(object sender, EventArgs e)
        {
            cpuTimer.Stop();

            if (isGameOver)
            {
                return;
            }

            int moveIndex = DecideCpuMoveIndex();
            if (moveIndex < 0)
            {
                isWaitingForCpu = false;
                UpdateCellEnabledStates();
                return;
            }

            PlaceMark(moveIndex, 'X');
            if (EvaluateAfterMove('X'))
            {
                return;
            }

            isWaitingForCpu = false;
            StatusMessage = "あなたの番です。任意の枠をクリックしてください";
            UpdateCellEnabledStates();
        }

        /// <summary>
        /// 指定したインデックスに記号を配置し、表示を更新します。
        /// </summary>
        /// <param name="index">配置するセルのインデックス（0..8）。</param>
        /// <param name="mark">'O' または 'X'。</param>
        /// <returns>戻り値はありません。</returns>
        private void PlaceMark(int index, char mark)
        {
            board[index] = mark;
            cells[index].MarkText = mark == 'O' ? "○" : "×";
        }

        /// <summary>
        /// 手番の直後に、勝敗または引き分けを判定して必要な処理を行います。
        /// </summary>
        /// <param name="lastMark">直前に置かれた記号（'O' または 'X'）。</param>
        /// <returns>決着した場合は true、それ以外は false。</returns>
        private bool EvaluateAfterMove(char lastMark)
        {
            int[] winLine;
            if (TryGetWinnerLine(lastMark, out winLine))
            {
                HighlightWinLine(winLine);
                isGameOver = true;
                isWaitingForCpu = false;
                UpdateCellEnabledStates();

                if (lastMark == 'O')
                {
                    StatusMessage = "決着しました。あなたの勝ち！";
                    MessageBox.Show("決着しました。あなたの勝ち！");
                }
                else
                {
                    StatusMessage = "決着しました。あなたの負け！";
                    MessageBox.Show("決着しました。あなたの負け！");
                }

                return true;
            }

            if (IsBoardFull())
            {
                isGameOver = true;
                isWaitingForCpu = false;
                UpdateCellEnabledStates();
                StatusMessage = "引き分けです。";
                MessageBox.Show("引き分けです。");
                return true;
            }

            return false;
        }

        /// <summary>
        /// 盤面がすべて埋まっているかどうかを判定します。
        /// </summary>
        /// <returns>すべて埋まっている場合は true、それ以外は false。</returns>
        private bool IsBoardFull()
        {
            int i;
            for (i = 0; i < 9; i++)
            {
                if (board[i] == ' ')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 勝利ラインをハイライト表示します。
        /// </summary>
        /// <param name="winLine">勝利した3マスのインデックス配列。</param>
        /// <returns>戻り値はありません。</returns>
        private void HighlightWinLine(int[] winLine)
        {
            if (winLine == null)
            {
                return;
            }

            int i;
            for (i = 0; i < winLine.Length; i++)
            {
                int idx = winLine[i];
                if (idx >= 0 && idx < 9)
                {
                    cells[idx].IsHighlighted = true;
                }
            }
        }

        /// <summary>
        /// 各セルの操作可否を、ゲーム状態に応じて更新します。
        /// </summary>
        /// <returns>戻り値はありません。</returns>
        private void UpdateCellEnabledStates()
        {
            int i;
            for (i = 0; i < 9; i++)
            {
                bool enabled = !isGameOver && !isWaitingForCpu && board[i] == ' ';
                cells[i].IsCellEnabled = enabled;
            }

            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// 指定した記号が勝利しているかどうかを判定し、勝利ラインを取得します。
        /// </summary>
        /// <param name="mark">'O' または 'X'。</param>
        /// <param name="winLine">勝利した場合の3マスインデックス配列。</param>
        /// <returns>勝利している場合は true、それ以外は false。</returns>
        private bool TryGetWinnerLine(char mark, out int[] winLine)
        {
            int i;
            for (i = 0; i < winLines.Length; i++)
            {
                int[] line = winLines[i];
                if (board[line[0]] == mark && board[line[1]] == mark && board[line[2]] == mark)
                {
                    winLine = line;
                    return true;
                }
            }

            winLine = null;
            return false;
        }

        /// <summary>
        /// CPUの手を決定します（簡単AI）。
        /// 勝てる手があれば優先し、次に負けを防ぐ手、最後にその他の手を選びます。
        /// </summary>
        /// <returns>選択したマスのインデックス。打てる場所がない場合は -1。</returns>
        private int DecideCpuMoveIndex()
        {
            int winMove = FindImmediateWinMove('X');
            if (winMove >= 0)
            {
                return winMove;
            }

            int blockMove = FindImmediateWinMove('O');
            if (blockMove >= 0)
            {
                return blockMove;
            }

            if (board[4] == ' ')
            {
                return 4;
            }

            int[] corners = new int[] { 0, 2, 6, 8 };
            int cornerIndex = ChooseFirstEmptyFromList(corners);
            if (cornerIndex >= 0)
            {
                return cornerIndex;
            }

            int[] edges = new int[] { 1, 3, 5, 7 };
            int edgeIndex = ChooseFirstEmptyFromList(edges);
            if (edgeIndex >= 0)
            {
                return edgeIndex;
            }

            return ChooseRandomEmpty();
        }

        /// <summary>
        /// 指定した記号が次の1手で勝てる位置があるかを探します。
        /// </summary>
        /// <param name="mark">'O' または 'X'。</param>
        /// <returns>勝てる位置のインデックス。なければ -1。</returns>
        private int FindImmediateWinMove(char mark)
        {
            int i;
            for (i = 0; i < winLines.Length; i++)
            {
                int[] line = winLines[i];
                int emptyIndex = -1;
                int markCount = 0;

                int j;
                for (j = 0; j < 3; j++)
                {
                    int idx = line[j];
                    if (board[idx] == mark)
                    {
                        markCount++;
                    }
                    else if (board[idx] == ' ')
                    {
                        emptyIndex = idx;
                    }
                }

                if (markCount == 2 && emptyIndex >= 0)
                {
                    return emptyIndex;
                }
            }

            return -1;
        }

        /// <summary>
        /// 指定リストの中から最初に空いているマスを選びます。
        /// </summary>
        /// <param name="candidates">候補インデックス配列。</param>
        /// <returns>空きマスのインデックス。なければ -1。</returns>
        private int ChooseFirstEmptyFromList(int[] candidates)
        {
            int i;
            for (i = 0; i < candidates.Length; i++)
            {
                int idx = candidates[i];
                if (idx >= 0 && idx < 9 && board[idx] == ' ')
                {
                    return idx;
                }
            }

            return -1;
        }

        /// <summary>
        /// 空いているマスからランダムに1つ選びます。
        /// </summary>
        /// <returns>空きマスのインデックス。なければ -1。</returns>
        private int ChooseRandomEmpty()
        {
            int[] empties = new int[9];
            int count = 0;

            int i;
            for (i = 0; i < 9; i++)
            {
                if (board[i] == ' ')
                {
                    empties[count] = i;
                    count++;
                }
            }

            if (count == 0)
            {
                return -1;
            }

            int pick = random.Next(0, count);
            return empties[pick];
        }

        /// <summary>
        /// 勝利ライン（8通り）を生成します。
        /// </summary>
        /// <returns>勝利ラインの配列。</returns>
        private int[][] CreateWinLines()
        {
            return new int[][]
            {
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 6, 7, 8 },
                new int[] { 0, 3, 6 },
                new int[] { 1, 4, 7 },
                new int[] { 2, 5, 8 },
                new int[] { 0, 4, 8 },
                new int[] { 2, 4, 6 }
            };
        }

        /// <summary>
        /// パラメーターからセルインデックスを取得します。
        /// </summary>
        /// <param name="parameter">コマンドパラメーター。</param>
        /// <param name="index">取得したインデックス。</param>
        /// <returns>取得できた場合は true、それ以外は false。</returns>
        private bool TryGetIndex(object parameter, out int index)
        {
            if (parameter is int)
            {
                index = (int)parameter;
                return true;
            }

            if (parameter is string)
            {
                int parsed;
                if (int.TryParse((string)parameter, out parsed))
                {
                    index = parsed;
                    return true;
                }
            }

            index = -1;
            return false;
        }
    }
}

