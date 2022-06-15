using System;
using System.Net.Http;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;

namespace Kccs.KccsApiService.PaasConnect
{
    /// <summary>
    /// PaaS Connect を用いた気象予報データ配信機能リクエストのサンプルプログラム。
    /// 
    /// Google API クライアントライブラリを用いて JWT 認証トークン(アクセストークン)の生成および、
    /// PaaS Connect へのリクエスト送信を行なう。
    /// 
    /// <code>
    /// 動作確認環境:
    ///     - Microsoft .NET Core SDK 3.1.419 (x64) (https://dotnet.microsoft.com/ja-jp/download/dotnet/3.1)
    ///     - Google.Apis.Auth 1.57.0 (https://www.nuget.org/packages/Google.Apis.Auth/1.57.0)
    /// 
    /// 実行方法:
    ///     本ファイルがあるディレクトリで下記コマンドを実行する。
    ///     (1) (初回のみ) dotnet add package Google.Apis.Auth --version 1.57.0
    ///     (2) dotnet run
    /// </code>
    /// </summary>
    class PaasConnectExample
    {
        /// <summary>
        /// PaaS Connect のエンドポイント URL
        /// </summary>
        const string PAAS_CONNECT_URL = "https://asia-northeast1-kccs-apiservice-prod-210201.cloudfunctions.net/kccsapiPaasConnect";
        /// <summary>
        /// サービスアカウントのEメールアドレス(サービスアカウントの JSON キーファイルの client_email)
        /// </summary>
        const string SERVICE_ACCOUNT_EMAIL = "xxxxxxxxxx@xxxxxxxxxxxxxxxxxxxxxxxx";
        /// <summary>
        /// サービスアカウントの秘密鍵(サービスアカウントの JSON キーファイルの private_key)
        /// </summary>
        const string PRIVATE_KEY = "-----BEGIN PRIVATE KEY-----\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\nxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx・・・・\n-----END PRIVATE KEY-----\n";
        /// <summary>
        /// KCCS API サービスのアクセスキーID
        /// </summary>
        const string KCCS_API_ACCESS_KEY_ID = "xxxxxxxxxxxx";
        /// <summary>
        /// KCCS API サービスのシークレットアクセスキー
        /// </summary>
        const string KCCS_API_SECRET_ACCESS_KEY = "xxxxxxxxxxxxxxxxx";

        static async Task Main()
        {
            // サービスアカウント認証オブジェクトを生成し、アクセストークンを取得する
            ServiceAccountCredential credential = CreateCredentialAndRequestAccessToken();
            // 取得したアクセストークンを表示(デバッグ用)
            Console.WriteLine("Token: " + credential.Token.AccessToken);
            // PaaS Connect 経由で GET リクエストを送信(非同期実行されるのでawaitで待機)
            await SendPaasRequest(credential, "api/v1/weather-forecasts/?latitude=35.642507&longitude=139.741836&3h_weathers=1&6h_probability_of_precipitations=1&1d_weathers=1&3h_winds=1&3h_temperatures=1&1d_temperatures=1&1d_winds=1");
        }

        /// <summary>
        /// PaaS Connect への GET リクエストを送信する。
        /// </summary>
        /// <param name="credential">サービスアカウント認証オブジェクト</param>
        /// <param name="apiPath">リクエストする API のパス</param>
        /// <returns>実行完了を待機するための Task オブジェクト</returns>
        private static async Task SendPaasRequest(ServiceAccountCredential credential, string apiPath)
        {
            var factory = new HttpClientFactory();
            var createHttpClientArgs = new CreateHttpClientArgs();

            // credential は Google.Apis.Http.IConfigurableHttpClientInitializer を実装しており、
            // 認証ヘッダを HTTP リクエストに自動付与してくれる
            // See: https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth#serviceaccountcredential
            createHttpClientArgs.Initializers.Add(credential);

            // client は Google.Apis.Http.ConfigurableHttpClient のインスタンス(System.Net.Http.HttpClient のサブクラス)
            using (var client = factory.CreateHttpClient(createHttpClientArgs))
            {
                // PaaS Connect リクエスト用の URL を生成
                var url = $"{PAAS_CONNECT_URL}/{apiPath}";
                Console.WriteLine("URL: " + url);

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                // リクエストヘッダに KCCS API の認証情報を付加
                request.Headers.Add("X-KCCS-API-USER", KCCS_API_ACCESS_KEY_ID);
                request.Headers.Add("X-KCCS-API-TOKEN", KCCS_API_SECRET_ACCESS_KEY);

                var response = await client.SendAsync(request);

                // レスポンスを出力
                Console.WriteLine("================================================================================");
                Console.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                Console.WriteLine("Body: " + await response.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        /// サービスアカウント認証オブジェクトを生成し、アクセストークンを取得する。
        /// </summary>
        /// <returns>生成されたサービスアカウント認証オブジェクト</returns>
        private static ServiceAccountCredential CreateCredentialAndRequestAccessToken()
        {
            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(SERVICE_ACCOUNT_EMAIL){
                    Scopes = new[]{ PAAS_CONNECT_URL }
                }.FromPrivateKey(PRIVATE_KEY)
            );
            if (!credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Result)
            {
                throw new Exception("Failed to request access token.");
            }
            return credential;
        }
    }
}
