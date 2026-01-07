## WPF タイマーアプリ一発生成用プロンプト

このテキストを別のチャットに貼り付ければ、最初の依頼（本チャット先頭）とその後の調整をすべて反映した状態で一発生成できます。01_counter の書式には合わせず、必要事項だけを簡潔にまとめています。

```text
C# の WPF アプリケーションを作成したいです。以下をすべて満たすコードを、一度で完全に出力してください。

【環境・構成】
- .NET 9 (`net9.0-windows`) の WPF アプリ。
- プロジェクトフォルダ: 02_Timer
- プロジェクトファイル: Timer.csproj
- 既定名前空間: Timer
- 出力対象: Timer.csproj / App.xaml / App.xaml.cs / MainWindow.xaml / MainWindow.xaml.cs / ViewModelBase.cs / RelayCommand.cs / TimerViewModel.cs

【共通ルール】
- MVVM。MainWindow.xaml.cs は InitializeComponent() のみ（DataContext は XAML 側で設定）。
- 使用文法は C#5.0 以前のみ（nameof, ?. など C#6+ 機能は禁止）。
- フィールド先頭にアンダースコアを付けない。
- すべてのクラス・プロパティ・メソッドに <summary> コメント。

【UI 要件】
- 初期表示 00分00秒。4 桁（分十・分一・秒十・秒一）の上下ボタン（△/▽）と数字表示。カウントダウン中は上下ボタンとリセットを無効化。
- スタート／ストップのトグルボタン、リセットボタンを下部に配置。スタート時に「ストップ」に切替。
- 時間が 0 になったら MessageBox.Show("時間です！")。
- レイアウト: ウィンドウ 250x320 前後、中央表示、リサイズ不可。
  - 行0: 6 列グリッド（分十・分一・分ラベル・秒十・秒一・秒ラベル）。各桁は行0:△、行1:数字、行2:▽。ラベル「分」は列2行1右下寄せ、「秒」は列5行1右下寄せ。
  - 行1: 下部ボタン StackPanel（横並び中央）。スタート/ストップは幅100高さ40背景 LightGreen、リセットは幅80高さ40背景 LightCoral（IsEnabled は調整ボタンと同じ条件）。
- DataContext は XAML の Window.DataContext で TimerViewModel を設定（コードビハインドで設定しない）。

【ロジック要件（TimerViewModel）】
- フィールド: StartText="スタート", StopText="ストップ", totalSeconds, isCountingDown, areAdjustButtonsEnabled, startStopButtonText, dispatcherTimer（readonly）。
- プロパティ: MinuteTens/MinuteOnes/SecondTens/SecondOnes は totalSeconds から算出。IsCountingDown/AreAdjustButtonsEnabled/StartStopButtonText は private set。
- コマンド: 上下 8 個 + StartStop + Reset。RelayCommand を使用。上下とリセットは AreAdjustButtonsEnabled が true のときのみ。StartStop は totalSeconds>0 または IsCountingDown が true のとき実行可。
- 初期化: dispatcherTimer Interval=1 秒、Tick を登録。totalSeconds=0、areAdjustButtonsEnabled=true、startStopButtonText=StartText。
- カウントダウン: Start で dispatcherTimer.Start、IsCountingDown=true、文言 StopText。Stop で停止し StartText。Tick ごとに totalSeconds--、0 で停止して StartText に戻し MessageBox。
- 桁操作: 桁を 0〜59 に収めて totalSeconds を再計算し、プロパティ更新＋コマンド更新。
- リセット: カウントダウン中は何もしない。停止中は totalSeconds=0 にし、プロパティ更新＋コマンド更新。

【MVVM 基盤】
- ViewModelBase: INotifyPropertyChanged、OnPropertyChanged(string)（CallerMemberName 不使用）。
- RelayCommand: ICommand 実装、executeAction/canExecutePredicate を保持、RaiseCanExecuteChanged を提供。

【csproj 要点】
- Sdk: Microsoft.NET.Sdk
- TargetFramework: net9.0-windows
- UseWPF: true
- Nullable: enable
- ImplicitUsings: enable
- RootNamespace: Timer

【出力形式】
- 各ファイルを見出し付きで分けてコードブロックに出力してください。
- 余計な説明は最小限。貼り付けだけでプロジェクトを再現できる形にしてください。
```

