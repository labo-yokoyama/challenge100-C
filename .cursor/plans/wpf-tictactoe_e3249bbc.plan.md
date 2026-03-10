---
name: wpf-tictactoe
overview: 05_TicTacToe（.NET 9 WPF）で、MVVM・C#5限定・2秒後CPU手・勝敗判定/引き分け/勝利ライン強調・開始ボタンによるリセットを満たす三目並べアプリを実装します。
todos:
  - id: create-project
    content: "`05_TicTacToe` のWPFプロジェクト作成（net9.0-windows / UseWPF / RootNamespace / LangVersion=5 / Nullable=disable）"
    status: completed
  - id: mvvm-core
    content: "`ViewModelBase`・`RelayCommand`・`CellViewModel`・`MainViewModel` をC#5互換で実装（XMLコメント完備、フィールド先頭'_'禁止）"
    status: completed
  - id: xaml-ui
    content: "`MainWindow.xaml` を250x320/中央/リサイズ不可で作成し、状態文言・3×3盤面（枠線）・開始ボタンをバインドで構築"
    status: completed
  - id: game-logic
    content: 勝敗/引き分け判定、勝利ラインハイライト、2秒後CPU手（DispatcherTimer）、決着時MessageBox表示を実装
    status: completed
  - id: lint-build-check
    content: ビルドできることを確認し、必要なら最小限の修正（lints/エラー）
    status: completed
isProject: false
---

## 変更/作成するもの（プロジェクト）

- 新規WPFプロジェクト: `05_TicTacToe/TicTacToe.csproj`
  - `TargetFramework`: `net9.0-windows`
  - `UseWPF`: `true`
  - `RootNamespace`: `TicTacToe`
  - `**LangVersion`: `5`（C#5.0固定）**
  - `Nullable`: `disable`（nullable参照型など新機能を避ける）

## UI（`MainWindow.xaml`）

- ウィンドウ
  - `Width="250" Height="320"` 前後
  - `WindowStartupLocation="CenterScreen"`
  - `ResizeMode="NoResize"`
- 上部に状態文言（要件の文言を初期表示）
  - 例: `TextBlock Text="{Binding StatusMessage}"`
- 3×3盤面
  - `ItemsControl` + `UniformGrid Columns="3" Rows="3"`
  - 各マスは `Button`
    - `Command`: `CellClickCommand`
    - `CommandParameter`: マスIndex（0..8）
    - `Content`: `Cells[i].MarkText`（"○"/"×"/空）
    - ハイライト: `Cells[i].IsHighlighted` に応じて `Background` を変更（例: 通常 White、勝利ライン Yellow）
- 「ゲーム開始」ボタン
  - `Command`: `StartGameCommand`

## MVVM（コードビハインド最小）

- `MainWindow.xaml.cs`
  - `InitializeComponent()` のみ（既存課題と同様）
- `MainViewModel`
  - `ObservableCollection<CellViewModel> Cells`（9個固定）
  - `string StatusMessage`（例: あなたの番/CPU思考中/決着済みなど）
  - `ICommand StartGameCommand`
  - `ICommand CellClickCommand`
  - 2秒待機のための `DispatcherTimer` をViewModel内で管理（クリック後に開始→TickでCPU手を打つ）
  - 入力制御: CPU待ち/決着済みの間はマスクリックを無効化（`CanExecute` か、Cell側 `IsEnabled` バインドで制御）
- `CellViewModel`
  - `int Index`
  - `string MarkText`（""/"○"/"×"）
  - `bool IsHighlighted`
  - `bool IsCellEnabled`
- `ViewModelBase`
  - C#5互換の `INotifyPropertyChanged` 実装（`OnPropertyChanged("Property")` 形式。`CallerMemberName`は使わない）
- `RelayCommand`
  - C#5互換（`Action<object>` / `Func<object,bool>`）

## ゲームロジック

- 盤面の内部表現
  - `char[] board`（'O','X',' ' など）を `MainViewModel` 内で保持し、`Cells`へ反映
- 勝敗判定
  - 8通りの勝利ライン（3行×3列+2斜め）を配列で定義
  - いずれかが同一記号で埋まったら勝利
  - 勝利時は該当3マスの `IsHighlighted=true`
- 引き分け判定
  - 全マス埋まり & 勝者なし
- メッセージボックス
  - 決着時に `MessageBox.Show(...)`
    - ユーザー勝ち: 「決着しました。あなたの勝ち！」
    - CPU勝ち: 「決着しました。あなたの負け！」
    - 引き分け: 「引き分けです。」
- 手順（要件順）
  - ユーザーが空マスクリック → ○配置 → 決着判定
  - 未決着なら **2秒後** にCPUが×配置 → 決着判定

## CPU簡単AI（×）

- 優先順位
  - **勝てる手があればそれを打つ**（×で3つ揃うラインの空き1を探す）
  - なければ **負けを防ぐ**（○が次で勝つラインの空き1を埋める）
  - なければ **残り空きから選択**（中央→角→辺、またはランダム）
- 実装はC#5互換（LINQは使ってもよいが、極力ループで明示）

## すべてのクラス/プロパティ/メソッドにXMLコメント

- `<summary>`必須
- メソッドは `<param>` / `<returns>` も必須

## 追加で整えるもの（任意だが推奨）

- 生成物（`bin/` `obj/` `.vs/`）をコミットしないための `.gitignore` を整備（既に未追跡の `.gitignore` があるため内容を確認して不足があれば追記）

