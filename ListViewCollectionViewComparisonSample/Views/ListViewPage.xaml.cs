using Syncfusion.Maui.DataSource;
namespace ListViewCollectionViewComparisonSample;

public partial class ListViewPage : ContentPage
{
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