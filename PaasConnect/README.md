# PaasConnectExample

KCCS API サービスの PaaS Connect 機能を C# 言語を使って利用する場合のサンプルコードです。

## Dependency

以下の環境で動作確認しています。

- [Microsoft .NET Core SDK 3.1.419 (x64)](https://dotnet.microsoft.com/ja-jp/download/dotnet/3.1)
- [Google.Apis.Auth 1.57.0](https://www.nuget.org/packages/Google.Apis.Auth/1.57.0)

## Setup

本プログラムを動作させるためには KCCS API サービスのアカウントの払い出しとサービスアカウントの認証キーの発行が必要です。サポートに連絡し、アカウントの払い出しおよびサービスアカウントの認証キーの発行を依頼してください。

1. サンプルプログラム一式(PaasConnectExample.csproj および PaasConnectExample.cs)を同一ディレクトリに配置してください。
2. PaasConnectExample.cs 内の SERVICE_ACCOUNT_EMAIL, PRIVATE_KEY, KCCS_API_ACCESS_KEY_ID, KCCS_API_SECRET_ACCESS_KEY の値をアカウント情報に合わせて書き換えてください。
3. カレントディレクトリをサンプルプログラムを配置したディレクトリに変更し、次のコマンドを実行してください。

    ```shell
    dotnet add package Google.Apis.Auth --version 1.57.0
    ```

デフォルトでは、KCCS API サービスの「天気予報データ配信機能」をリクエストするようになっています。
リクエストする API を変更したい場合は、PaasConnectExample.cs の Main() メソッド内にある SendPaasRequest() 呼び出しの第2引数(下記コードの `"api/v1/..."` の部分)を変更してください。

```csharp
await SendPaasRequest(credential, "api/v1/weather-forecasts/?latitude=35.642507&longitude=139.741836&3h_weathers=1&6h_probability_of_precipitations=1&1d_weathers=1&3h_winds=1&3h_temperatures=1&1d_temperatures=1&1d_winds=1");
```

どのような値に変更すればいいのかについては、サービス仕様書の「気象データ配信API」の章の各機能の「リクエストURI」および「リクエスト」を参照してください。

## Usage

カレントディレクトリをサンプルプログラムを配置したディレクトリに変更し、次のコマンドを実行してください。

```shell
dotnet run
```

## Licence

このソフトウェアは、 [Apache 2.0ライセンス](https://www.apache.org/licenses/LICENSE-2.0)で配布されている製作物が含まれています。

## Authors

KYOCERA Communication System Co., Ltd.