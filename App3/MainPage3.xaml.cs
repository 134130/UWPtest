using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Syndication;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace App3
{
    public sealed partial class MainPage3 : Page
    {
        static Payload payload;
        static HttpClient httpClient;
        static readonly String host = "https://shiis.uhs.ac.kr/";

        public MainPage3()
        {
            this.InitializeComponent();
            currentTime.Text = DateTime.Now.ToString();

            TimeSpan period = TimeSpan.FromSeconds(0.2);
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                // TODO: Work
                DateTime now = DateTime.Now;
                //////////////////
                await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    () =>
                    {
                        currentTime.Text = now.ToString();
                    });
            }, period);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            payload = (Payload)e.Parameter;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IAsyncAction asyncAction = ThreadPool.RunAsync((workItem) =>
            {
                _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    statusText.Text = "Hi";
                }));
            });
        }

        private async void Login_Button(object sender, RoutedEventArgs e)
        {
            httpClient = new HttpClient();
            var url = host + "hsudoc/sys.Login.doj";
            var parameters = new Dictionary<string, string> {
                { "strCommand", "LOGIN"}, { "strTarget", "MAIN" },
                { "strLoginId", payload.ans1}, { "strLoginPw", payload.ans2} };
            var encodedContent = new FormUrlEncodedContent(parameters);
            var headers = httpClient.DefaultRequestHeaders;
            try
            {
                headers.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36");
            } catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }

            //Uri reqUri = new Uri(host + "hsudoc/sys.Login.doj");
            HttpResponseMessage httpRes;


            httpRes = await httpClient.PostAsync(url, encodedContent);
            //var httpResBody = httpRes.Content.ReadAsStringAsync();
            //Debug.WriteLine(httpResBody.Result);
            statusText.Text = "Login Success";
            statusText.Text = httpRes.Headers.ToString();

        }

        private async void Regist_Button(object sender, RoutedEventArgs e)
        {
            var url = host + "hsudoc/sug.SugSugangMng.do";
            List<FormUrlEncodedContent> encodedContents = new List<FormUrlEncodedContent>();
            foreach (List<string> ans in payload.ans5)
            {
                var parameter = new Dictionary<string, string> {
                { "strCommand", "Save1" }, { "strSugangYy", payload.ans3 },
                { "strSugangShtm", payload.ans4 }, { "strDanFg", "1" },
                { "strSbjtCd",  ans[0]}, { "strDecs", String.Format("{00}", ans[1]) } };
                encodedContents.Add(new FormUrlEncodedContent(parameter));
            }

            HttpResponseMessage httpRes = new HttpResponseMessage();
            foreach(var encodedContent in encodedContents)
            {
                statusText.Text = "Suggest";
                try { 
                    httpRes = await httpClient.PostAsync(url, encodedContent);
                    var res = await httpRes.Content.ReadAsStreamAsync();
                    StreamReader reader = new StreamReader(res, Encoding.GetEncoding(51949));
                    statusText.Text = reader.ReadToEnd();
                } catch (Exception ex)
                {
                    statusText.Text = ex.ToString();
                }
            }
            
        }
    }
}
