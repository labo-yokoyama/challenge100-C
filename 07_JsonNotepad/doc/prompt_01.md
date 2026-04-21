# JSON形式の個別ファイル管理メモ帳アプリケーション作成プロンプト (改訂版)

## 概要
メモごとに個別のJSONファイルとして保存・管理し、タイトルによる検索機能を持つWPFアプリケーションを作成します。

## プロジェクト構成
- プロジェクト名: JsonNotepad
- フレームワーク: .NET Framework 4.5.2
- 言語: C# 5.0以前
- UIフレームワーク: WPF, MVVM
- プロジェクトフォルダ: 07_JsonNotepad
- プロジェクトファイル: JsonNotepad.csproj
- 既定名前空間: JsonNotepad

## UI要件
- メインウィンドウを左右に分割する（比率は 1 : 2）。
- ウィンドウの `ContentRendered` イベント発生時に、ViewModel の初期化処理（Initialize）をコマンド経由で実行する仕組み（添付プロパティ等）を実装すること。
- 左側（管理パネル）
    - 上部に「検索」用のTextBoxを配置。
    - 中央にメモのタイトル一覧を表示するListBoxを配置。検索文字に応じてリアルタイムに絞り込まれること。
    - 下部に「新規作成」「削除」ボタンを配置。
- 右側（編集パネル）
    - メモの「タイトル」を入力するTextBox（これがファイル名になります）。
    - メモの「内容」を入力するTextBox（複数行、自動垂直スクロールバーあり）。
    - 右上に「保存」ボタンを配置。
- MainWindow.xaml.cs は InitializeComponent() のみで、コードビハインドは使用しない。

## ロジック要件
- NuGetパッケージ: `Newtonsoft.Json` を使用する。
- 保存の仕組み:
    - 実行ファイルと同じフォルダに `notes` フォルダを作成し、その中に `[タイトル].json` という形式で個別に保存する。
    - ファイル入出力時は UTF-8 を明示的に指定する。
- 初期化処理:
    - `notes` フォルダが存在しなければ自動作成し、既存の全JSONファイルを読み込む。
- バリデーション機能:
    - 保存実行時に以下のチェックを行い、不備がある場合は保存を中断してメッセージを表示する。
    - メッセージボックスのタイトルは「JsonNotepad」とし、適切なアイコン（Warning, Error等）を表示すること。
    - **未入力チェック**: タイトルが空（または空白のみ）の場合、「タイトルを入力してください」と表示。
    - **不適切文字チェック**: タイトルにファイル名として使用できない文字（`\`, `/`, `:`, `*`, `?`, `"`, `<`, `>`, `|`）が含まれている場合、「”[その文字]”はファイル名に使えません」と表示。
- 検索機能:
    - 検索窓に入力した文字列がタイトルに含まれるメモのみをListBoxに表示する。
- データ構造:
    - `NoteItem`クラス: タイトル、内容、最終更新日時を保持。

## コーディング規約
- **C# 5.0 以前の制限**: 以下の機能は絶対に使用しない（CS5.0以前の文法を遵守）。
    - null 条件演算子 (`?.`, `?[]`)
    - 文字列補完 (`$""`)
    - `nameof` 演算子
    - 式形式のメンバー (`=>`)
    - 自動プロパティの初期化子
- **フィールド命名**: 先頭にアンダースコア（_）を付けない。
- **ドキュメントコメント**: すべてのクラス、プロパティ、メソッドに `<summary>` を付ける。
    - プロパティの summary は「メモのタイトル」のように名詞句で記述する（「～を取得または設定します」は不要）。
    - メソッドには引数や戻り値がある場合のみ `<param>` や `<returns>` を記述する。
- **RelayCommand 実装**: `RaiseCanExecuteChanged` メソッド内では `CommandManager.InvalidateRequerySuggested()` を呼び出すこと（直接イベントに代入しない）。
- **クリーンアップ**: 未使用の `using` ディレクティブは削除すること。

## ファイル構成（出力対象）
- すべてのファイルは `07_JsonNotepad` フォルダ内に配置すること。
- `07_JsonNotepad/JsonNotepad.csproj`
- `07_JsonNotepad/App.xaml`
- `07_JsonNotepad/App.xaml.cs`
- `07_JsonNotepad/MainWindow.xaml`
- `07_JsonNotepad/MainWindow.xaml.cs`
- `07_JsonNotepad/MainViewModel.cs`
- `07_JsonNotepad/NoteItem.cs`
- `07_JsonNotepad/ViewModelBase.cs`
- `07_JsonNotepad/RelayCommand.cs`
- `07_JsonNotepad/WindowBehaviors.cs`
