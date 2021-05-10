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

namespace AuthForTeslaDroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var tokensForm = FindViewById<Android.Widget.LinearLayout>(Resource.Id.tokensForm);
            tokensForm.Visibility = ViewStates.Gone;

            var login = FindViewById<AppCompatButton>(Resource.Id.login);
            login.Click += Login_Click;
        }

        private async void Login_Click(object sender, EventArgs e)
        {
            var username = FindViewById<AppCompatEditText>(Resource.Id.username);
            var password = FindViewById<AppCompatEditText>(Resource.Id.password);
            var mfaCode = FindViewById<AppCompatEditText>(Resource.Id.mfaCode);
            var accessToken = FindViewById<AppCompatTextView>(Resource.Id.accessToken);
            var refreshToken = FindViewById<AppCompatTextView>(Resource.Id.refreshToken);

            // When it's time to authenticate:
            var authHelper = new TeslaAuthHelper("AuthForTeslaDroid/1.0");
            var tokens = await authHelper.AuthenticateAsync(username.Text, password.Text, mfaCode.Text);

            accessToken.Text = tokens.AccessToken;
            refreshToken.Text = tokens.RefreshToken;

            var tokensForm = FindViewById<Android.Widget.LinearLayout>(Resource.Id.tokensForm);
            tokensForm.Visibility = ViewStates.Visible;

            var loginForm = FindViewById<Android.Widget.LinearLayout>(Resource.Id.loginForm);
            loginForm.Visibility = ViewStates.Gone;

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
	}
}
