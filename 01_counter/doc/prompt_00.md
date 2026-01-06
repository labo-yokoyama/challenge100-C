## WPF カウンターアプリ一発生成用プロンプト

以下のプロンプトを別のチャットに貼り付ければ、この WPF カウンターアプリの実装一式を一発で生成できる想定です。

```text
C# の WPF アプリケーションを作成したいです。以下の要件をすべて満たすプロジェクト構成とコードを、一度で完全に出力してください。

【環境・プロジェクト構成】
- .NET 8（`net8.0-windows`）向けの WPF アプリケーション。
- プロジェクトフォルダ名は `01_counter` とします（フォルダ名に合わせた調整は不要です）。
- プロジェクトファイル名は `01_counter.csproj`。
- `RootNamespace` は **`counter`** としてください（数字から始まらない有効な名前空間）。
- 必要なファイル一式をすべてコードで出してください：
  - `01_counter/01_counter.csproj`
  - `01_counter/App.xaml`
  - `01_counter/App.xaml.cs`
  - `01_counter/MainWindow.xaml`
  - `01_counter/MainWindow.xaml.cs`

【プロジェクトファイル（csproj）の要件】
- SDK は `Microsoft.NET.Sdk.WindowsDesktop` でも `Microsoft.NET.Sdk` でも構いませんが、WPF が動作する設定にしてください。
- `<TargetFramework>net8.0-windows</TargetFramework>` を指定してください。
- `<UseWPF>true</UseWPF>` を指定してください。
- `<Nullable>enable</Nullable>` を指定してください。
- `<RootNamespace>counter</RootNamespace>` を指定してください。

【App.xaml / App.xaml.cs の要件】
- 名前空間は `counter`。
- `App.xaml` の `x:Class` は `counter.App`。
- `StartupUri` は `MainWindow.xaml`。
- `App.xaml.cs` は `public partial class App : Application` とし、特別な処理は不要です。

【MainWindow.xaml / MainWindow.xaml.cs の要件】
- 名前空間は `counter`。
- `MainWindow.xaml` の `x:Class` は `counter.MainWindow`。
- ウィンドウは白背景で、タイトルは `01 Counter (WPF)`、サイズはおおよそ 800x450、画面中央に表示してください。
- 画面中央に、次のようなレイアウトを配置してください：
  - 縦方向の `StackPanel` を中央に配置。
  - 上に現在の数値を表示する `TextBlock`（初期表示は `"0"`、フォントサイズは大きめ）。
  - 下に `+` ボタンと `-` ボタンを横並び (`StackPanel Orientation="Horizontal"`) で配置。
- `TextBlock` の `x:Name` は `CounterTextBlock`。
- ボタンの `x:Name` は `PlusButton` と `MinusButton`。
- ボタンにはそれぞれ `Click="PlusButton_Click"`、`Click="MinusButton_Click"` を指定してください。

【カウンターの動作仕様（MainWindow.xaml.cs）】
- クラス `MainWindow` の名前空間は `counter`。
- `MainWindow` は `Window` を継承した `partial` クラス。
- フィールドとして `private int _count = 0;` を持たせ、初期値は 0。
- コンストラクタで `InitializeComponent();` の後に現在値を表示するメソッド（例: `UpdateCounterText();`）を呼び出してください。
- `PlusButton_Click` では `_count` を 1 増やし、`MinusButton_Click` では `_count` を 1 減らしてください。
- `UpdateCounterText` メソッドで `CounterTextBlock.Text = _count.ToString();` のように現在の数値を表示してください。

【出力形式の指示】
- 各ファイルを区別して出力してください（例：「01_counter/01_counter.csproj:」「01_counter/App.xaml:」のように見出し or コメントを付けてから、そのファイルの中身をコードブロックで示す）。
- 余計な説明文は最小限にし、必要なコードをすべて再現できるようにしてください。
```


