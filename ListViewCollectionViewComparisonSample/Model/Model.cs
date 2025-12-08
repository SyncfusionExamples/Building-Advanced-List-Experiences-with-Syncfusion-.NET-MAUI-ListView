using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ListViewCollectionViewComparisonSample;

/// <summary>
/// Represents a bindable book item with name, description, and favorite flag.
/// Implements INotifyPropertyChanged for UI data binding.
/// </summary>
public class BookInfo : INotifyPropertyChanged
{
    private string bookName;
    private string bookDesc;
    private bool isFavorite = false;

    /// <summary>
    /// Gets or sets whether the book is marked as favorite.
    /// Raises <see cref="PropertyChanged"/> when changed.
    /// </summary>
    public bool IsFavorite
    {
        get => isFavorite;
        set
        {
            if (isFavorite == value) return;
            isFavorite = value;
            OnPropertyChanged(nameof(IsFavorite));
        }
    }

    /// <summary>
    /// Gets or sets the display title of the book.
    /// Raises <see cref="PropertyChanged"/> when changed.
    /// </summary>
    public string BookName
    {
        get => bookName;
        set
        {
            if (bookName == value) return;
            bookName = value;
            OnPropertyChanged(nameof(BookName));
        }
    }

    /// <summary>
    /// Gets or sets the short description of the book.
    /// Raises <see cref="PropertyChanged"/> when changed.
    /// </summary>
    public string BookDescription
    {
        get => bookDesc;
        set
        {
            if (bookDesc == value) return;
            bookDesc = value;
            OnPropertyChanged(nameof(BookDescription));
        }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="name">The name of the property that changed.</param>
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

/// <summary>
/// Represents a named group of <see cref="BookInfo"/> items for grouped UI views.
/// </summary>
public class BookGroup : ObservableCollection<BookInfo>
{
    /// <summary>
    /// Gets the group name (for example, the first letter of book titles).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BookGroup"/> class.
    /// </summary>
    /// <param name="name">The name of the group.</param>
    /// <param name="items">The items to include in the group.</param>
    public BookGroup(string name, IEnumerable<BookInfo> items) : base(items) => Name = name;
}