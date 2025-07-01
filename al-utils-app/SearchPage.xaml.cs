using al_utils_app.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace al_utils_app
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchPage : ContentPage
	{
        //TODO: search by id
        private int currPage = 0;
        public string BuildQuery(int page)
        {
            return $@"query ($search: String) {{
  Page(page: {page}, perPage: 50) {{
  	media(search: $search, format_in: [TV, TV_SHORT, MOVIE, SPECIAL, OVA, OVA], sort: POPULARITY_DESC) {{
    	id,
      title {{
        romaji
        english
        native
      }},
        description,
      coverImage {{
        extraLarge
        color
      }}
  	}}
	}}
}}
";
        }
        private Dictionary<string, object> BuildVariables(string search)
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables.Add("search", search);
            return variables;
        }
        private async Task<List<SearchResult>> GetData(string search)
        {
            Response data = await Request.RequestDataAsync(BuildQuery(currPage), BuildVariables(search));
            var dict = data.Data.Page.Results;
            return dict;
        }

		public SearchPage ()
		{
			InitializeComponent ();
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string prev = searchbar.Text;

            if (prev == "" || prev == null)
                Results.Clear();

            await Task.Delay(500);

            string now = searchbar.Text;
            if (prev == now && now.Length >= 2) // stopped typing after delay
			    SearchForMedia(now);
        }


        private void searchbar_SearchButtonPressed(object sender, EventArgs e)
        {
			var search = searchbar.Text;
			if (search.Length == 1) // manually search if one char
			    SearchForMedia(search);
        }

        private ObservableCollection<SearchResult> Results { get; set; } = new ObservableCollection<SearchResult>();

		private async void SearchForMedia(string search)
		{
            List<SearchResult> results = await GetData(search);
            Results = new ObservableCollection<SearchResult>(results);

            //Console.WriteLine(Results.Count);
            //foreach (var d in results)
            //{
            //    Console.WriteLine(d.Id);
            //}
            resultList.ItemsSource = Results;
		}

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var id = (int)((TappedEventArgs)e).Parameter;
            await Navigation.PushAsync(new MediaPage(id));
        }
    }
}