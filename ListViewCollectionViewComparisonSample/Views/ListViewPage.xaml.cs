using Syncfusion.Maui.DataSource;
namespace ListViewComparisonSample;

/// <summary>
/// Page demonstrating Syncfusion SfListView with grouping, swipe actions,
/// drag-and-drop and manual load-more (with lazy-loading spinner).
/// </summary>
public partial class ListViewPage : ContentPage
{
	/// <summary>
	/// Initializes the page and configures grouping on the data source.
	/// </summary>
	public ListViewPage()
	{
		InitializeComponent();

        // Group items by the first character of BookName (uppercase).
        listView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
        {
            PropertyName = "BookName",
            KeySelector = (object obj1) =>
            {
                var item = (obj1 as BookInfo);
                return item.BookName[0].ToString();
            }
        });
    }
}
