# 簡易カーニング-YMM4プラグイン
## 概要
テキストの描画位置や拡大率などを文字ごとに調整する映像エフェクトプラグインです。  
また、文字ごとに映像エフェクトをかける機能を提供します。  
## 導入方法
### [最新バージョンをダウンロード](https://github.com/tetra-te/SimpleKerningEffect/releases/latest)
ダウンロードしたファイルをダブルクリックして、表示される画面にしたがってインストールしてください。
> [!NOTE]
> **ファイルをダブルクリックしてもインストールできない場合**  
> YMM4を起動して`ヘルプ(H)`>`YMM4用拡張子の関連付け`>`登録する`から拡張子の関連付けを行ってください。  
> 再度ファイルをダブルクリックすることでインストール画面が表示されます。

## 使用方法
3つの映像エフェクト **簡易カーニング**, **文字間隔調整（横書）**, **文字間隔調整（縦書）** が追加されます。  
各エフェクトは映像エフェクトの`テキスト`カテゴリに追加されます。  
> [!IMPORTANT]
> プラグインの動作には文字ごとに分割が必要です。  
> テキストアイテムの文字ごとに分割を必ず有効にしてください。
### 簡易カーニングエフェクト
指定した文字の描画位置や拡大率を変更したり、映像エフェクトを付与したりできます。  
#### 文字位置
カーニング対象の文字を先頭から何文字目かの数字で指定します。  
`,`(カンマ)を用いた複数指定、`-`(ハイフン)を用いた範囲指定が可能です。  
例えば1,2,5,6,7文字目を対象にする場合は次のような指定が可能です。  
* `1,2,5,6,7`
* `1,2,5-7`
* `1-2,5-7`  

半角スペースは無視されます。間違った指定をするとエフェクトが適用されません。  
#### 描画
**文字位置**で指定した文字を対象に、描画位置や回転角などのシンプルなエフェクトを適用します。  
#### 簡易カーニング内のエフェクト
**文字位置**で指定した文字を対象に、縁取りや3D回転などのYMM4の映像エフェクトを適用します。  
### 文字間隔調整（横書,縦書）エフェクト
文字の一部分を対象に文字間隔を調整できるエフェクトです。テキストの設定によって横書と縦書を使い分けてください。  
#### 開始・終了
文字間隔を変更する範囲を指定します。  
#### 文字間隔
開始から終了までの範囲の文字間隔を調整します。
#### 全体を調整
有効にすると範囲内の文字と範囲外の文字が重ならないように範囲外の文字の描画位置を調整します。  
デフォルトで有効です。
## アンインストール方法
1. YMM4を起動して、`ヘルプ(H)`>`その他`>`プラグインフォルダを開く`をクリックする。  
2. YMM4を終了する。
3. `SimpleKerningEffect`という名前のフォルダを削除する。  
## 謝辞
文字ごとに映像エフェクトを付与する機能は[sinβ](https://x.com/sinBetaKun)(sinBetaKun)氏による実装です。  
これによりプラグインの自由度が大幅に向上しました。  
この場をお借りして、感謝申し上げます。
## ライセンス
[CC0 1.0 Universal](./LICENSE)