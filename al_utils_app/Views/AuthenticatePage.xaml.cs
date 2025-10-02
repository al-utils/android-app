using al_utils_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace al_utils_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthenticatePage : ContentPage
    {
        public AuthenticatePage()
        {
            InitializeComponent();
        }

        private async void webViewButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WebViewPage("https://anilist.co/api/v2/oauth/authorize?client_id=28257&response_type=token", "Login to Anilist"));
            receivedToken.IsVisible = true;
        }

        private void pasteButton_Clicked(object sender, EventArgs e)
        {
            if (Clipboard.HasText)
            {
                var text = Clipboard.GetTextAsync();
                textbox.Text = text.Result;
            }
        }

        private void clearButton_Clicked(object sender, EventArgs e)
        {
            textbox.Text = "";
        }

        private string BuildQuery()
        {
            return $@"
                query{{
    Viewer{{
        name
        id
        options{{
            titleLanguage
        }}
    }}
}}
";
        }

        private async void submitButton_Clicked(object sender, EventArgs e)
        {
            var token = textbox.Text;
            // check authorized
            Response r = await Request.RequestDataAsync(BuildQuery(), new Dictionary<string, object>(), token);
            if (r != null)
            {
                Authentication.SaveAccessToken(token);

                var username = r.Data.Viewer.Name;
                var userId = r.Data.Viewer.Id;
                Preferences.Set("currentUser", username);
                Preferences.Set("userId", userId);

                var rootPage = Navigation.NavigationStack.ToList()[0];
                Navigation.InsertPageBefore(new MainPage(username), rootPage);
                await Navigation.PopToRootAsync();
            }
            else
            {
                await DisplayAlert("Error", "Authentication failed. Please try again. ", "Ok");
            }

        }
    }
}