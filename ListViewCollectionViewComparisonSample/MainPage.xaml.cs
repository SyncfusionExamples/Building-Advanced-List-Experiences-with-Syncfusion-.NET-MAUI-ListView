namespace ListViewCollectionViewComparisonSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void collectionViewPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CollectionViewPage());
        }

        private void listViewPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ListViewPage());
        }
    }
}
