# Sniper Elite 4 AsrTextExtractor
誰も作らないので作成　　

## 使い方:　　
### AsrTextExtractor - 主にファイルの中に HTXT が一つの場合(Text の中身が対象です)
* AsrTextExtractor.exe -u Menu.asr_en [output.csv]  
 => `commandId,Text` で出力  
翻訳などにご使用ください

* AsrTextExtractor.exe -c Menu.asr_en Menu.asr_jp [output.csv]  
 => Switch や ps4 から翻訳をひっぱてきた場合に override で使用できる対応表を作ります。
 
* AsrTextExtractor.exe -o Menu.asr_en output.csv output.asr_en  
 => `commandId,Text,翻訳` の CSV から commandId と Text が一致したものを「翻訳」の内容に書き換えます
 
* AsrTextExtractor.exe -fo Menu.asr_en output.csv output.asr_en  
 => `commandId,Text,翻訳` の CSV から commandId が一致したものを「翻訳」の内容に書き換えます

```
Usage:
 Comparison: AsrTextExtractor.exe -c <asr|en file> <asr|en file> [<csv text file>]
 Unpack    : AsrTextExtractor.exe -u <asr|en file> [<csv text file>]
 Override  : AsrTextExtractor.exe -o <asr|en file> <csv text file> [<new asr|en file>]

options:
  -c        Create Comparison table option
  -u        Unpack option
  -o        Override option
  -fo       Override Force option

arguments:
  <asr file>      asr file path
  <csv text file> csv text file path
  <new asr file>  new asr file path
 ```
### AsrVoiceTextExtractor - 主にファイルの中に DLLN が複数の場合(Text の中身以外が対象です)
* AsrVoiceTextExtractor.exe -u MP.pc_en [output.csv]  
 => `commandId,Text,Text` で出力  
翻訳などにご使用ください

* AsrVoiceTextExtractor.exe -o MP.pc_en output.csv output.pc_en  
 => `commandId,Text,翻訳` の CSV から commandId が一致したものを「翻訳」の内容に書き換えます

```
Usage:
 Unpack    : AsrVoiceTextExtractor.exe -u <asr|en file> [<csv text file>]
 Override  : AsrVoiceTextExtractor.exe -o <asr|en file> <csv text file> [<new asr|en file>]

options:
  -u        Unpack option
  -o       Override Force option

arguments:
  <asr file>      asr file path
  <csv text file> csv text file path
  <new asr file>  new asr file path
 ```
