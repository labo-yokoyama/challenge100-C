## WPF カウンターアプリ（MVVM版・C#5文法限定）一発生成用プロンプト

以下のプロンプトを別のチャットに貼り付ければ、この WPF カウンターアプリ（現在の状態：MVVM構成・スタイル含む）を一発で生成できる想定です。

```text
C# の WPF アプリケーションを作成したいです。以下の要件をすべて満たすプロジェクト構成とコードを、一度で完全に出力してください。

【重要な制約】
- C# 5.0 以前の文法だけを使用してください。
  - ファイルスコープ名前空間（`namespace X;`）は使わず、必ずブロック形式の `namespace X { ... }` を使ってください。
  - null 許容参照型（`string?` など）は使わないでください。
  - 式形式メンバー（式形式プロパティ、式形式メソッド、式形式イベントアクセサなど）は使わないでください。
  - 自動プロパティは C#5 で使える範囲（通常の `get; set;`、`get; private set;` など）のみを使用してください。

【環境・プロジェクト構成】
- .NET 8（`net8.0-windows`）向けの WPF アプリケーション。
- プロジェクトフォルダ名は `01_counter` とします（フォルダ名に合わせた調整は不要です）。
- プロジェクトファイル名は `01_counter.csproj`。
- `RootNamespace` は **`counter`** としてください。
- 必要なファイル一式をすべてコードで出してください：
  - `01_counter/01_counter.csproj`
  - `01_counter/App.xaml`
  - `01_counter/App.xaml.cs`
  - `01_counter/MainWindow.xaml`
  - `01_counter/MainWindow.xaml.cs`
  - `01_counter/CounterViewModel.cs`

【プロジェクトファイル（csproj）の要件】
- SDK は `Microsoft.NET.Sdk.WindowsDesktop` でも `Microsoft.NET.Sdk` でも構いませんが、WPF が動作する設定にしてください。
- `<TargetFramework>net8.0-windows</TargetFramework>` を指定してください。
- `<UseWPF>true</UseWPF>` を指定してください。
- `<Nullable>disable</Nullable>` または Nullable 無効相当の設定にしてください（C#5相当として扱いたいため）。
- `<RootNamespace>counter</RootNamespace>` を指定してください。

【App.xaml / App.xaml.cs の要件】
- 名前空間は `counter`。
- `App.xaml` の `x:Class` は `counter.App`。
- `StartupUri` は `MainWindow.xaml`。
- `App.xaml.cs` は `public partial class App : Application` とし、特別な処理は不要です。
- `App` クラスには日本語の `<summary>` コメントを付けてください（アプリケーション全体を表す旨）。

【MainWindow.xaml / MainWindow.xaml.cs の要件（View 部分）】
- 名前空間は `counter`。
- `MainWindow.xaml` の `x:Class` は `counter.MainWindow`。
- ウィンドウは白背景で、タイトルは `01 Counter (WPF)`、サイズはおおよそ **幅 400 / 高さ 450**、画面中央に表示してください。
- 画面の中心に、次のようなレイアウトを配置してください：
  - ルートは `Grid`。
  - 中央に縦方向の `StackPanel` を配置。
  - 上段に現在の数値を表示する `TextBlock`（`Text="{Binding Count}"`、フォントサイズは 48、中央寄せ、下にマージン 20）。
  - 下段に `+` ボタンと `-` ボタンを横並びに配置するが、次のように **間にボタン1個分の空白**を入れてください：
    - `StackPanel` ではなく `Grid` を使い、3列の `ColumnDefinition` を定義する。
    - 左列（Auto）に `+` ボタン、中央列に幅 `60` の空列、右列（Auto）に `-` ボタンを配置する。
- `Window.DataContext` に `CounterViewModel` をインスタンス化して設定してください（XAML 内で `<local:CounterViewModel />` を使う）。
- XAML 上ではコードビハインドの `Click` イベントは使わず、**コマンドバインディング（`Command`）だけ**を使用してください。

【ボタンのスタイル要件】
- 丸いボタンスタイルを `Window.Resources` 内に定義してください。
- 共通のプラスボタン用スタイル `RoundButtonStyle`（`x:Key="RoundButtonStyle"`）:
  - `TargetType="Button"`。
  - `Width="60"`, `Height="60"`。
  - `FontSize="32"`（+ の文字が大きく見えるサイズ）。
  - `Background="#FF4CAF50"`（緑系）、`Foreground="White"`。
  - `BorderBrush="#FF388E3C"`, `BorderThickness="1"`。
  - `ControlTemplate` をカスタムし、`Border` の `CornerRadius="30"` を指定して**正円のボタン**にしてください。
  - `IsMouseOver` 時は背景色を少し明るい緑（例: `#FF66BB6A`）、`IsPressed` 時は少し濃い緑（例: `#FF2E7D32`）に変えるトリガーを定義してください。
  - 無効時（`IsEnabled="False"`）は `Opacity="0.5"` にするトリガーを定義してください。
- マイナスボタン用スタイル `RoundMinusButtonStyle`（`x:Key="RoundMinusButtonStyle"`）:
  - `BasedOn` ではなく、**プラスボタンとは別に完全なスタイルとして定義**してください（ホバー時に緑にならないようにするため）。
  - `TargetType="Button"`。
  - `Width="60"`, `Height="60"`。
  - `FontSize="32"`。
  - `Background="#FFF44336"`（赤系）、`Foreground="White"`。
  - `BorderBrush="#FFD32F2F"`, `BorderThickness="1"`。
  - `ControlTemplate` はプラスボタンと同様に `CornerRadius="30"` の円形ボタンとしつつ、トリガーで赤系の色変化を行ってください。
  - `IsMouseOver` 時は少し明るい赤（例: `#FFE57373`）、`IsPressed` 時は濃い赤（例: `#FFD32F2F`）に変更するトリガーを定義してください。
  - 無効時は `Opacity="0.5"` にしてください。
- `+` ボタンは `Style="{StaticResource RoundButtonStyle}"` を、`-` ボタンは `Style="{StaticResource RoundMinusButtonStyle}"` を適用してください。

【MainWindow.xaml のボタン配置】
- 下段のレイアウトは以下のようにしてください（概念的な指定です）：
  - `Grid HorizontalAlignment="Center"` の中に 3 列の `ColumnDefinition`。
  - `Column 0` に `+` ボタン（`Content="+"`、`Command="{Binding IncrementCommand}"`）。
  - `Column 1` に `Width="60"` の空列（実際には要素を置かず、列幅だけを空白として確保）。
  - `Column 2` に `-` ボタン（`Content="-"`、`Command="{Binding DecrementCommand}"`）。
  - 各ボタンには `Margin="5"` を付けてください。

【MainWindow.xaml.cs の要件（コードビハインド）】
- 名前空間は `counter`。
- `MainWindow` は `Window` を継承した `public partial class`。
- コンストラクタは `public MainWindow()` のみで、`InitializeComponent();` を呼び出すだけにしてください。
- **コードビハインドにカウンターのロジックやボタンのクリックイベント処理は一切書かないでください。**
- クラスとコンストラクタには、日本語の `<summary>` コメントを付けてください。

【CounterViewModel.cs の要件（ViewModel 部分）】
- 名前空間は `counter`（ブロック形式の `namespace counter { ... }`）。
- クラス `CounterViewModel` は `INotifyPropertyChanged` を実装します。
- C#5文法で実装してください（ラムダや匿名メソッドは C#5 で可能な範囲で使用可、式形式プロパティは禁止）。
- メンバー：
  - フィールド `private int _count;`：カウンターの現在値。
  - プロパティ `public int Count`：
    - getter で `_count` を返し、setter で値が変わったときだけ `_count` を更新して `OnPropertyChanged()` を呼び出します。
  - `public ICommand IncrementCommand { get; private set; }`：
    - カウンターを 1 増やすコマンド。
  - `public ICommand DecrementCommand { get; private set; }`：
    - カウンターを 1 減らすコマンド。
  - コンストラクタ `public CounterViewModel()`：
    - `IncrementCommand = new RelayCommand(delegate(object o) { Count++; });`
    - `DecrementCommand = new RelayCommand(delegate(object o) { Count--; });`
  - `public event PropertyChangedEventHandler PropertyChanged;`
  - `private void OnPropertyChanged([CallerMemberName] string propertyName = null)`：
    - C#5互換の書き方で `CallerMemberName` 属性を使い、`PropertyChanged` イベントを発行します。
    - `handler` をローカル変数に退避して null チェック後に `handler(this, new PropertyChangedEventArgs(propertyName));` を呼び出してください。
- `CounterViewModel` クラス、フィールド `_count`、プロパティ `Count`、コマンドプロパティ、コンストラクタ、`PropertyChanged` イベント、`OnPropertyChanged` メソッドには、すべて日本語の `<summary>` コメントを付けてください。

【RelayCommand の要件】
- `CounterViewModel.cs` と同じファイル内で、`CounterViewModel` と同じ名前空間 `counter` の中に `public class RelayCommand : ICommand` を定義してください。
- C#5文法のみを使って実装してください。
- フィールド：
  - `private readonly Action<object> _execute;`
  - `private readonly Func<object, bool> _canExecute;`（null の場合は常に実行可能とみなす）。
- コンストラクタ：
  - `public RelayCommand(Action<object> execute)`：`this(execute, null)` を呼び出す。
  - `public RelayCommand(Action<object> execute, Func<object, bool> canExecute)`：
    - `execute` が null の場合は `new ArgumentNullException("execute")` を投げる。
    - フィールド `_execute` と `_canExecute` を初期化する。
- メソッド：
  - `public bool CanExecute(object parameter)`：
    - `_canExecute` が null の場合は `true` を返し、それ以外の場合は `_canExecute(parameter)` を返す。
  - `public void Execute(object parameter)`：
    - `_execute(parameter)` を呼び出す。
- イベント：
  - `public event EventHandler CanExecuteChanged`：
    - `add { CommandManager.RequerySuggested += value; }`
    - `remove { CommandManager.RequerySuggested -= value; }`
- `RelayCommand` クラス、フィールド `_execute`・`_canExecute`、両方のコンストラクタ、`CanExecute`、`Execute`、`CanExecuteChanged` イベントには、日本語の `<summary>` コメントを付けてください。

【コメント・ドキュメンテーション】
- すべての `<summary>` コメントは **日本語** で記述してください。
- クラス・プロパティ・メソッド・イベントに対して、今回説明した内容を参考に、意味がわかる短い日本語の説明を書いてください。

【出力形式の指示】
- 各ファイルを区別して出力してください（例：「01_counter/01_counter.csproj:」「01_counter/App.xaml:」のように見出しやコメントを付けてから、そのファイルの中身をコードブロックで示す）。
- 余計な説明文は最小限にし、必要なコードをすべて再現できるようにしてください。
```



