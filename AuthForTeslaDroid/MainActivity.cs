using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using TeslaAuth;
using Android.Webkit;
using System.Threading.Tasks;
using Android.Graphics;
using System.Web;

namespace AuthForTeslaDroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        WebView web_view;
        HelloWebViewClient client = new HelloWebViewClient();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var tokensForm = FindViewById<Android.Widget.LinearLayout>(Resource.Id.tokensForm);
            tokensForm.Visibility = ViewStates.Gone;

            var webviewForm = FindViewById<Android.Widget.LinearLayout>(Resource.Id.webviewForm);
            webviewForm.Visibility = ViewStates.Gone;

            var login = FindViewById<AppCompatButton>(Resource.Id.login);
            login.Click += Login_Click;

            web_view = FindViewById<WebView>(Resource.Id.webview);
            web_view.Settings.JavaScriptEnabled = true;
            web_view.SetWebViewClient(client);
            web_view.LoadUrl("about:blank");
        }


        private async void Login_Click(object sender, EventArgs e)
        {
            var accessToken = FindViewById<AppCompatTextView>(Resource.Id.accessToken);
            var refreshToken = FindViewById<AppCompatTextView>(Resource.Id.refreshToken);

            // When it's time to authenticate:
            var authHelper = new TeslaAuthHelper("AuthForTeslaDroid/1.0");

            var tokens = await authHelper.AuthenticateAsync(async (codeUrl, cancellationToken) =>
            {
                var webviewForm = FindViewById<Android.Widget.LinearLayout>(Resource.Id.webviewForm);
                webviewForm.Visibility = ViewStates.Visible;

                Console.WriteLine(codeUrl);
                web_view.ClearCache(true);
                web_view.ClearHistory();
                CookieManager.Instance.RemoveAllCookies(null);
                CookieManager.Instance.Flush();
                web_view.LoadUrl(codeUrl);
                var code = await WaitForCode();

                webviewForm.Visibility = ViewStates.Gone;
                return code;
            });

            accessToken.Text = tokens.AccessToken;
            refreshToken.Text = tokens.RefreshToken;

            var tokensForm = FindViewById<Android.Widget.LinearLayout>(Resource.Id.tokensForm);
            tokensForm.Visibility = ViewStates.Visible;

            var loginForm = FindViewById<Android.Widget.LinearLayout>(Resource.Id.loginForm);
            loginForm.Visibility = ViewStates.Gone;

        }

        private Task<string> WaitForCode()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    if (client.authCode != null)
                    {
                        var code = client.authCode;
                        client.authCode = null;
                        return code;
                    }
                }
            });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
            
        public class HelloWebViewClient : WebViewClient
        {
            public string authCode = null;

            // For API level 24 and later
            public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
            {
                view.LoadUrl(request.Url.ToString());

                Console.WriteLine(request.Url);
                try
                {
                    authCode = HttpUtility.ParseQueryString(request.Url.Query).Get("code");
                }
                catch (Exception)
                {
                }

                return false;
            }
        }
    }
}
