namespace ListViewCollectionViewComparisonSample
{
    /// <summary>
    /// Landing page to navigate to the CollectionView and SfListView samples.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initializes the main page UI components.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Navigates to the CollectionView sample page.
        /// </summary>
        private void collectionViewPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CollectionViewPage());
        }

        /// <summary>
        /// Navigates to the Syncfusion ListView sample page.
        /// </summary>
        private void listViewPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ListViewPage());
        }
    }
}
