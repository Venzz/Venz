using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Venz.Telemetry
{
    public sealed class SalesforceTelemetryService: ITelemetryService
    {
        private Object Sync = new Object();
        private String AppTitle;
        private String AppVersion;
        private String Platform;
        private HttpClient Client;
        private Task InitializationTask;
        private JsonObject ApplicationIdentity;
        private SessionData Session;



        public SalesforceTelemetryService(String appTitle, String appVersion, String platform)
        {
            AppTitle = appTitle;
            AppVersion = appVersion;
            Platform = platform;
        }

        public void Start() { }

        public void Finish() { }

        public async void LogEvent(String title) => await LogEventAsync(title);

        public async void LogEvent(String title, String parameter, String value) => await LogEventAsync(title, parameter, value);

        public void LogException(String comment, Exception exception)
        {
            /*return Task.Run(async () =>
            {
                try
                {
                    var session = await InitializeAsync().ConfigureAwait(false);
                    if (session == null)
                        return;

                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{session.Instance}/services/apexrest/Venz/telemetry/exception", UriKind.Absolute));
                    httpRequest.Headers.Authorization = new HttpCredentialsHeaderValue("Bearer", session.AccessToken);

                    var jsonObject = new JsonObject();
                    jsonObject.Add("appIdentity", ApplicationIdentity);
                    jsonObject.Add("title", (name == null) ? JsonValue.CreateNullValue() : JsonValue.CreateStringValue(name));
                    jsonObject.Add("message", (message == null) ? JsonValue.CreateNullValue() : JsonValue.CreateStringValue(message));
                    jsonObject.Add("stacktrace", (stacktrace == null) ? JsonValue.CreateNullValue() : JsonValue.CreateStringValue(stacktrace));
                    httpRequest.Content = new HttpStringContent(jsonObject.Stringify(), UnicodeEncoding.Utf8, "application/json");
                    var response = await Client.SendRequestAsync(httpRequest).AsTask().ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            });*/
        }

        public IAsyncAction LogDailyEventAsync(String title)
        {
            if (title == null)
                return TaskExtensions.CompletedAction;

            return Task.Run(async () =>
            {
                try
                {
                    await EnsureInitializedAsync().ConfigureAwait(false);
                    if (Session == null)
                        return;

                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Session.Instance}/services/apexrest/Venz/telemetry/daily_event", UriKind.Absolute));
                    httpRequest.Headers.Authorization = new HttpCredentialsHeaderValue("Bearer", Session.AccessToken);

                    var jsonObject = new JsonObject();
                    jsonObject.Add("appIdentity", ApplicationIdentity);
                    jsonObject.Add("title", JsonValue.CreateStringValue(title));
                    httpRequest.Content = new HttpStringContent(jsonObject.Stringify(), UnicodeEncoding.Utf8, "application/json");
                    await Client.SendRequestAsync(httpRequest).AsTask().ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }).AsAsyncAction();
        }

        public IAsyncAction LogEventAsync(String title) => LogEventAsync(title, null, null);

        public IAsyncAction LogEventAsync(String title, String parameter, String parameterValue)
        {
            if (title == null)
                return TaskExtensions.CompletedAction;

            return Task.Run(async () =>
            {
                try
                {
                    await EnsureInitializedAsync().ConfigureAwait(false);
                    if (Session == null)
                        return;

                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Session.Instance}/services/apexrest/Venz/telemetry/event", UriKind.Absolute));
                    httpRequest.Headers.Authorization = new HttpCredentialsHeaderValue("Bearer", Session.AccessToken);

                    var jsonObject = new JsonObject();
                    jsonObject.Add("appIdentity", ApplicationIdentity);
                    jsonObject.Add("title", JsonValue.CreateStringValue(title));
                    if (parameter != null)
                        jsonObject.Add("param", JsonValue.CreateStringValue(parameter));
                    if (parameterValue != null)
                        jsonObject.Add("value", JsonValue.CreateStringValue(parameterValue));
                    httpRequest.Content = new HttpStringContent(jsonObject.Stringify(), UnicodeEncoding.Utf8, "application/json");
                    await Client.SendRequestAsync(httpRequest).AsTask().ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }).AsAsyncAction();
        }



        private Task EnsureInitializedAsync()
        {
            lock (Sync)
            {
                if (InitializationTask == null)
                    InitializationTask = InitializeAsync();
                return InitializationTask;
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                Client = new HttpClient();
                ApplicationIdentity = new JsonObject();
                ApplicationIdentity.Add("name", JsonValue.CreateStringValue(AppTitle));
                ApplicationIdentity.Add("version", JsonValue.CreateStringValue(AppVersion));
                ApplicationIdentity.Add("platform", JsonValue.CreateStringValue(Platform));

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri("https://login.salesforce.com/services/oauth2/token", UriKind.Absolute));
                var httpRequestParameters = new Dictionary<String, String>();
                httpRequestParameters.Add("grant_type", "password");
                httpRequestParameters.Add("client_id", "3MVG9Rd3qC6oMalVqvMHlTbHOcK_ZQLUD9AXE5sLbR_Z56f34p9riJR67v6yWXExAv6HO29_UtKJTNjyOJkZ.");
                httpRequestParameters.Add("client_secret", "3175602535861280176");
                httpRequestParameters.Add("username", "venexzz@gmail.com.telemetry");
                httpRequestParameters.Add("password", "monev666QxOnFuth5SwIZOYM7q74F24t");
                httpRequest.Content = new HttpFormUrlEncodedContent(httpRequestParameters);
                var response = await Client.SendRequestAsync(httpRequest).AsTask().ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var responseString = response.Content.ToString();
                    Session = new SessionData(responseString.Between("\"access_token\":\"", "\""), responseString.Between("\"instance_url\":\"", "\""));
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
        }

        private class SessionData
        {
            public String AccessToken { get; }
            public String Instance { get; }

            public SessionData(String accessToken, String instance)
            {
                AccessToken = accessToken;
                Instance = instance;
            }
        }
    }
}
