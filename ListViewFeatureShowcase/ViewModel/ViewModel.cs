using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ListViewFeatureShowcase;

/// <summary>
/// ViewModel that exposes a paged, groupable collection of <see cref="BookInfo"/> items,
/// with favorite toggle, delete, and incremental loading support for UI binding.
/// </summary>
public partial class BookInfoRepository : INotifyPropertyChanged
{
	#region Constants

	/// <summary>
	/// Number of items loaded per page during initial loading.
	/// </summary>
	private const int PageSize = 50;

	#endregion

	#region Fields

	/// <summary>
	/// Current insertion index used to page through the backing dataset.
	/// </summary>
	private int currentIndex = 0;

	/// <summary>
	/// Total number of items available from the data source.
	/// </summary>
	private readonly int totalItems = 100;

	private ObservableCollection<BookGroup> bookGroups = new();
	private string currentGroupName = string.Empty;
	private bool isLoading;

	#endregion

	#region Collections

	/// <summary>
	/// Flat collection bound to the list UI.
	/// </summary>
	public ObservableCollection<BookInfo> BookInfo { get; } = new();

	/// <summary>
	/// Grouped projection of <see cref="BookInfo"/> (e.g., A–Z headers).
	/// </summary>
	public ObservableCollection<BookGroup> BookGroups
	{
		get => bookGroups;
		private set { bookGroups = value; OnPropertyChanged(nameof(BookGroups)); }
	}

	#endregion

	#region Properties

	/// <summary>
	/// Name of the group currently at the top of the list (for sticky header emulation).
	/// </summary>
	public string CurrentGroupName
	{
		get => currentGroupName;
		set { if (currentGroupName == value) return; currentGroupName = value; OnPropertyChanged(nameof(CurrentGroupName)); }
	}

	/// <summary>
	/// Indicates whether a load-more operation is in progress.
	/// </summary>
	public bool IsLoading
	{
		get => isLoading;
		private set { isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
	}

	/// <summary>
	/// Exposes whether more items can be loaded for binding (e.g., show/hide footer button).
	/// </summary>
	public bool HasMoreItems => CanLoadMore();

	#endregion

	#region Commands

	/// <summary>
	/// Command to toggle the favorite state of a book.
	/// </summary>
	public ICommand FavoriteCommand { get; }

	/// <summary>
	/// Command to delete a book from the collection.
	/// </summary>
	public ICommand DeleteCommand { get; }

	/// <summary>
	/// Command to load the next page of items; suitable for “Load more” UI.
	/// </summary>
	public ICommand LoadMoreItemsCommand { get; }

	#endregion

	#region INotifyPropertyChanged

	/// <summary>
	/// Occurs when a property value changes. 
	/// </summary>
	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged(string name) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance, wires commands, and seeds the initial page of data.
	/// </summary>
	public BookInfoRepository()
	{
		LoadMoreItemsCommand = new Command<object>(LoadMoreItems, CanLoadMoreItems);
		FavoriteCommand = new Command<BookInfo>(OnFavorite);
		DeleteCommand = new Command<BookInfo>(OnDelete);
		LoadInitialBooks();
	}

	#endregion

	#region Initialization

	/// <summary>
	/// Loads the initial page and builds grouping.
	/// </summary>
	private void LoadInitialBooks()
	{
		AddBooks(0, PageSize);
		currentIndex = PageSize;
		RebuildGroups();
		CurrentGroupName = BookGroups.FirstOrDefault()?.Name ?? string.Empty;
		OnPropertyChanged(nameof(HasMoreItems));
		(LoadMoreItemsCommand as Command<object>)?.ChangeCanExecute();
	}

	#endregion

	#region Paging/Loading

	/// <summary>
	/// Indicates whether more items can be loaded based on <see cref="totalItems"/>.
	/// </summary>
	private bool CanLoadMore() => BookInfo.Count < totalItems;

	/// <summary>
	/// Wrapper used by the load-more command to evaluate executability.
	/// </summary>
	private bool CanLoadMoreItems(object obj) => CanLoadMore() && !IsLoading;

	/// <summary>
	/// Loads the next page of items and updates the UI; optionally toggles SfListView lazy loader.
	/// </summary>
	/// <param name="obj">
	/// The sender (optionally a Syncfusion.Maui.ListView.SfListView) to show IsLazyLoading UI.
	/// </param>
	private async void LoadMoreItems(object obj)
	{
		if (!CanLoadMore() || IsLoading)
		{
			return;
		}
		else
		{
			if (obj is Syncfusion.Maui.ListView.SfListView listView)
			{
				listView.IsLazyLoading = true;
				await Task.Delay(2000);
				listView.IsLazyLoading = false;
			}
			else
			{
				IsLoading = true;
				await Task.Delay(2000);
				IsLoading = false;
			}

			AddBooks(currentIndex, PageSize);
			currentIndex += PageSize;
			RebuildGroups();
		}
	}

	#endregion

	#region Data population

	/// <summary>
	/// Adds a slice of items from the backing dataset into <see cref="BookInfo"/>.
	/// </summary>
	private void AddBooks(int start, int count)
	{
		var books = new[]
		{
			new BookInfo { BookName = "Object-Oriented Programming in C#", BookDescription = "OOP paradigm based on objects" },
			new BookInfo { BookName = "C# Code Contracts", BookDescription = "Convey code assumptions" },
			new BookInfo { BookName = "Machine Learning Using C#", BookDescription = "Learn ML approaches" },
			new BookInfo { BookName = "Neural Networks Using C#", BookDescription = "Exciting field of software dev" },
			new BookInfo { BookName = "Visual Studio Code", BookDescription = "Powerful code editor" },
			new BookInfo { BookName = "Android Programming", BookDescription = "Overview of Android lifecycle" },
			new BookInfo { BookName = "iOS Succinctly", BookDescription = "Step into iPhone development" },
			new BookInfo { BookName = "Visual Studio 2015", BookDescription = "New IDE version" },
			new BookInfo { BookName = "Xamarin.Forms", BookDescription = "Cross-platform UI toolkit" },
			new BookInfo { BookName = "Windows Store Apps", BookDescription = "Radical shift in Windows dev" },
			new BookInfo { BookName = ".NET MAUI Fundamentals", BookDescription = "Build cross-platform native apps" },
			new BookInfo { BookName = "ASP.NET Core Web APIs", BookDescription = "Design RESTful services" },
			new BookInfo { BookName = "Entity Framework Core", BookDescription = "Map objects to relational DBs" },
			new BookInfo { BookName = "LINQ in Practice", BookDescription = "Expressive querying for data" },
			new BookInfo { BookName = "Design Patterns in C#", BookDescription = "Classic patterns in modern C#" },
			new BookInfo { BookName = "Async and Await in C#", BookDescription = "Responsive async programming" },
			new BookInfo { BookName = "Unit Testing with xUnit", BookDescription = "Write fast, reliable tests" },
			new BookInfo { BookName = "Dependency Injection in .NET", BookDescription = "Decouple and manage lifetimes" },
			new BookInfo { BookName = "Blazor for Web UI", BookDescription = "Interactive UIs with C#" },
			new BookInfo { BookName = "Git Essentials for Developers", BookDescription = "Workflows for collaboration" },
			new BookInfo { BookName = "C# 12 New Features", BookDescription = "What’s new and how to use it" },
			new BookInfo { BookName = "Clean Architecture in .NET", BookDescription = "Maintainable, testable apps" },
			new BookInfo { BookName = "SOLID Principles Explained", BookDescription = "Practical design guidance" },
			new BookInfo { BookName = "gRPC with .NET", BookDescription = "High-performance RPC services" },
			new BookInfo { BookName = "CQRS and MediatR", BookDescription = "Command–query separation" },
			new BookInfo { BookName = "Minimal APIs in ASP.NET Core", BookDescription = "Lean HTTP endpoints" },
			new BookInfo { BookName = "SignalR Realtime Apps", BookDescription = "Push updates over websockets" },
			new BookInfo { BookName = "Dapper Essentials", BookDescription = "Micro-ORM for raw performance" },
			new BookInfo { BookName = "Refactoring in C#", BookDescription = "Improve design safely" },
			new BookInfo { BookName = "Source Generators", BookDescription = "Compile-time code generation" },
			new BookInfo { BookName = "MAUI Shell Navigation", BookDescription = "Tabs, flyouts, deep links" },
			new BookInfo { BookName = "MAUI Handlers", BookDescription = "Customize native controls" },
			new BookInfo { BookName = "MAUI Performance Tips", BookDescription = "Smoother UI and startup" },
			new BookInfo { BookName = "MAUI Dependency Injection", BookDescription = "Compose services for apps" },
			new BookInfo { BookName = "MAUI Styling and Theming", BookDescription = "Consistent, reusable styles" },
			new BookInfo { BookName = "MAUI Graphics", BookDescription = "Draw shapes and paths" },
			new BookInfo { BookName = "MAUI Animations", BookDescription = "Delightful motion effects" },
			new BookInfo { BookName = "MAUI Data Binding", BookDescription = "Connect UI and ViewModel" },
			new BookInfo { BookName = "MAUI Shell URI Routing", BookDescription = "Navigate with parameters" },
			new BookInfo { BookName = "MAUI Accessibility", BookDescription = "Inclusive UX practices" },
			new BookInfo { BookName = "REST API Design", BookDescription = "Resources, verbs, and errors" },
			new BookInfo { BookName = "OpenAPI & Swashbuckle", BookDescription = "Document your endpoints" },
			new BookInfo { BookName = "Authentication with JWT", BookDescription = "Secure API access" },
			new BookInfo { BookName = "Authorization Policies", BookDescription = "Fine-grained control" },
			new BookInfo { BookName = "Caching Strategies", BookDescription = "Faster and cheaper calls" },
			new BookInfo { BookName = "Rate Limiting in .NET", BookDescription = "Protect your services" },
			new BookInfo { BookName = "Azure App Service Basics", BookDescription = "Deploy and scale web apps" },
			new BookInfo { BookName = "Docker for .NET", BookDescription = "Containerize applications" },
			new BookInfo { BookName = "Kubernetes for Developers", BookDescription = "Orchestrate workloads" },
			new BookInfo { BookName = "Azure Functions", BookDescription = "Serverless event handlers" },
			new BookInfo { BookName = "PostgreSQL with .NET", BookDescription = "Npgsql and indexing tips" },
			new BookInfo { BookName = "SQL Server Tuning", BookDescription = "Queries, plans, and stats" },
			new BookInfo { BookName = "NoSQL with MongoDB", BookDescription = "Documents and schemas" },
			new BookInfo { BookName = "Redis Caching", BookDescription = "In-memory speedups" },
			new BookInfo { BookName = "EF Core Performance", BookDescription = "Tracking and batching" },
			new BookInfo { BookName = "Migrations and Seeding", BookDescription = "Version your database" },
			new BookInfo { BookName = "Transactions and Concurrency", BookDescription = "Consistency patterns" },
			new BookInfo { BookName = "LINQ Query Patterns", BookDescription = "Idiomatic operators" },
			new BookInfo { BookName = "Async Data Access", BookDescription = "Non-blocking I/O" },
			new BookInfo { BookName = "Connection Pooling", BookDescription = "Throughput under load" },
			new BookInfo { BookName = "Testing ASP.NET Core", BookDescription = "Unit, integration, E2E" },
			new BookInfo { BookName = "Test Doubles in C#", BookDescription = "Mocks, stubs, fakes" },
			new BookInfo { BookName = "FluentAssertions", BookDescription = "Readable assertions" },
			new BookInfo { BookName = "Playwright for .NET", BookDescription = "UI automation" },
			new BookInfo { BookName = "BenchmarkDotNet Basics", BookDescription = "Measure performance" },
			new BookInfo { BookName = "Profiling .NET Apps", BookDescription = "Find bottlenecks" },
			new BookInfo { BookName = "Logging with Serilog", BookDescription = "Structured logs" },
			new BookInfo { BookName = "Tracing with OpenTelemetry", BookDescription = "End-to-end visibility" },
			new BookInfo { BookName = "Feature Flags", BookDescription = "Ship safely and iterate" },
			new BookInfo { BookName = "Blue/Green Deployments", BookDescription = "Zero-downtime releases" },
			new BookInfo { BookName = "Security Fundamentals", BookDescription = "Principles and threats" },
			new BookInfo { BookName = "OWASP Top 10 for APIs", BookDescription = "Mitigate common risks" },
			new BookInfo { BookName = "Data Protection in .NET", BookDescription = "Encrypt and hash" },
			new BookInfo { BookName = "Secure Storage in MAUI", BookDescription = "Protect local secrets" },
			new BookInfo { BookName = "Key Vault Integration", BookDescription = "Centralize secrets" },
			new BookInfo { BookName = "Identity with OAuth2/OIDC", BookDescription = "Modern auth flows" },
			new BookInfo { BookName = "HTTPS and HSTS", BookDescription = "Transport security" },
			new BookInfo { BookName = "Content Security Policy", BookDescription = "Mitigate XSS" },
			new BookInfo { BookName = "Input Validation", BookDescription = "Trust nothing" },
			new BookInfo { BookName = "Secure Coding in C#", BookDescription = "Practical safeguards" },
			new BookInfo { BookName = "WPF to MAUI Migration", BookDescription = "Plan and port UI" },
			new BookInfo { BookName = "WinForms Interop", BookDescription = "Embed and reuse" },
			new BookInfo { BookName = "gRPC-Web with Blazor", BookDescription = "Browser-friendly RPC" },
			new BookInfo { BookName = "Background Services", BookDescription = "Hosted workers" },
			new BookInfo { BookName = "Quartz.NET Scheduling", BookDescription = "Timed jobs" },
			new BookInfo { BookName = "Polly Resilience", BookDescription = "Retry and circuit breakers" },
			new BookInfo { BookName = "Message Queues with RabbitMQ", BookDescription = "Async messaging" },
			new BookInfo { BookName = "Event Sourcing Basics", BookDescription = "Store events, not state" },
			new BookInfo { BookName = "Domain-Driven Design", BookDescription = "Ubiquitous language" },
			new BookInfo { BookName = "Microservices in .NET", BookDescription = "Bounded contexts" },
			new BookInfo { BookName = "Caching in MAUI", BookDescription = "List virtualization tips" },
			new BookInfo { BookName = "Image Optimization", BookDescription = "Faster lists and grids" },
			new BookInfo { BookName = "Localization in MAUI", BookDescription = "Global-ready apps" },
			new BookInfo { BookName = "Push Notifications", BookDescription = "Engage users" },
			new BookInfo { BookName = "In-App Purchases", BookDescription = "Monetize features" },
			new BookInfo { BookName = "SQLite in MAUI", BookDescription = "Local persistence" },
			new BookInfo { BookName = "Preferences and SecureStorage", BookDescription = "Store settings" },
			new BookInfo { BookName = "Offline Sync", BookDescription = "Resilient data" },
			new BookInfo { BookName = "Maps and Geolocation", BookDescription = "Location features" },
			new BookInfo { BookName = "Notifications and Toasts", BookDescription = "User feedback" }
		};

		for (int i = start; i < start + count && i < books.Length; i++)
		{
			var item = books[i];
			item.Order = BookInfo.Count; // sequential order as items are added
			BookInfo.Add(item);
		}
	}

	#endregion

	#region Grouping

	/// <summary>
	/// Rebuilds <see cref="BookGroups"/> by grouping <see cref="BookInfo"/> on first letter of <see cref="BookInfo.BookName"/>.
	/// </summary>
	private void RebuildGroups()
	{
		var groups = BookInfo
			.GroupBy(books => string.IsNullOrWhiteSpace(books.BookName) ? "#" : books.BookName.Substring(0, 1).ToUpperInvariant())
			.OrderBy(group => group.Min(books => books.Order)) // order groups by first appearance in the flat list
			.Select(group => new BookGroup(group.Key, group.OrderBy(books => books.Order))); // keep item order per group

		BookGroups = new ObservableCollection<BookGroup>(groups);

		if (string.IsNullOrEmpty(CurrentGroupName))
			CurrentGroupName = BookGroups.FirstOrDefault()?.Name ?? string.Empty;
	}

	/// <summary>
	/// Public method to rebuild groups, useful for UI interactions like drag/drop reorder.
	/// </summary>
	public void RefreshGroups() => RebuildGroups();

	/// <summary>
	/// Reindex the Order property to match the current sequence in BookInfo.
	/// Call after reordering items.
	/// </summary>
	public void ReindexOrders()
	{
		for (int i = 0; i < BookInfo.Count; i++)
		{
			if (BookInfo[i].Order != i)
				BookInfo[i].Order = i;
		}
	}

	#endregion

	#region Item actions

	/// <summary>
	/// Toggles the favorite state of the specified <paramref name="book"/>.
	/// </summary>
	private async void OnFavorite(BookInfo book)
	{
		if (book == null) return;
		book.IsFavorite = !book.IsFavorite;

        await Shell.Current.DisplayAlertAsync(
                "Favorite",
                book.IsFavorite
                    ? "Added to Favorites"
                    : "Removed from Favorites",
                "OK");

    }

    /// <summary>
    /// Removes the specified <paramref name="book"/> from <see cref="BookInfo"/> and refreshes groups.
    /// </summary>
    private void OnDelete(BookInfo book)
	{
		if (book == null) return;
		if (BookInfo.Remove(book))
			RebuildGroups();
	}

	#endregion
}