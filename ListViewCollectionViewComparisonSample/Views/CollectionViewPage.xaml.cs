namespace ListViewComparisonSample;

/// <summary>
/// Page demonstrating a grouped CollectionView with sticky header emulation,
/// swipe actions, manual load-more, and drag-and-drop item reordering.
/// </summary>
public partial class CollectionViewPage : ContentPage
{
    /// <summary>
    /// Initializes the page and subscribes to the Loaded event.
    /// </summary>
    public CollectionViewPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Strongly-typed access to the bound ViewModel.
    /// </summary>
    private BookInfoRepository Vm => BindingContext as BookInfoRepository;

    /// <summary>
    /// Handles page Loaded to initialize the pinned header text.
    /// </summary>
    private void OnLoaded(object sender, EventArgs e)
    {
        UpdateCurrentGroupHeader();
    }

    /// <summary>
    /// Updates the sticky header label with the name of the group currently at the top.
    /// </summary>
    private void OnCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        UpdateCurrentGroupHeader(e.FirstVisibleItemIndex);
    }

    /// <summary>
    /// Computes and sets the current group name based on the first visible item index.
    /// </summary>
    /// <param name="firstVisibleIndex">The flat index of the first visible item.</param>
    private void UpdateCurrentGroupHeader(int firstVisibleIndex = 0)
    {
        if (Vm == null || Vm.BookGroups == null || Vm.BookGroups.Count == 0)
            return;

        var index = firstVisibleIndex < 0 ? 0 : firstVisibleIndex;

        // Map to group by flattening groups until we cover firstVisibleIndex
        int cursor = 0;
        foreach (var group in Vm.BookGroups)
        {
            int groupCount = group.Count;
            if (index < cursor + groupCount)
            {
                Vm.CurrentGroupName = group.Name;
                return;
            }
            cursor += groupCount;
        }

        // Fallback
        Vm.CurrentGroupName = Vm.BookGroups[0].Name;
    }

    /// <summary>
    /// Begins a drag operation by placing the bound item in the drag data bag.
    /// </summary>
    private void OnDragStarting(object sender, DragStartingEventArgs e)
    {
        if (sender is Element element && element.BindingContext is BookInfo item)
        {
            e.Data.Properties["item"] = item;
        }
    }

    /// <summary>
    /// Handles item drop to reorder items in the underlying list and refresh the grouped view.
    /// </summary>
    private void OnDrop(object sender, DropEventArgs e)
    {
        if (Vm == null) return;
        if (!e.Data.Properties.TryGetValue("item", out var payload) || payload is not BookInfo source)
            return;

        if (sender is Element element && element.BindingContext is BookInfo target && !ReferenceEquals(source, target))
        {
            var list = Vm.BookInfo;
            var sourceIndex = list.IndexOf(source);
            var targetIndex = list.IndexOf(target);
            if (sourceIndex >= 0 && targetIndex >= 0)
            {
                // Normalize target index when removing earlier item affects index
                if (sourceIndex < targetIndex)
                    targetIndex--;

                list.RemoveAt(sourceIndex);
                list.Insert(targetIndex, source);
                // Update stable order indices and rebuild grouping to reflect new order in the UI
                Vm.ReindexOrders();
                Vm.RefreshGroups();
            }
        }
    }
}
