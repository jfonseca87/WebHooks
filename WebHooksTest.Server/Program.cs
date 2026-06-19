var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<WebHookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/webhook/subscribe", (WebHookService srv, Subscription subs) =>
{
    srv.Subscribe(subs);
    return Results.Ok();
});


app.Run();

public record Subscription(string Topic, string Callback);
public record Message (string Topic, string Action, object Data);
public record Book(string Title, string Author, int Year, int Pages);

public class WebHookService
{
    private readonly List<Subscription> _subscriptions = new();
    private readonly HttpClient _httpClient;

    public WebHookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void Subscribe(Subscription subscription) =>
        _subscriptions.Add(subscription);

    public async Task Notify(Message message)
    {
        var subscriptions = _subscriptions.Where(s => s.Topic == message.Topic);

        var tasks = new List<Task>();
        foreach (var subscription in subscriptions)
        {
            tasks.Add(_httpClient.PostAsJsonAsync(subscription.Callback, message));
        }

        await Task.WhenAll(tasks);
    }
}

public class BookService
{
    private readonly List<Book> _books;

    public BookService()
    {
        _books = SeedBooks();
    }

    public IEnumerable<Book> GetBooks() => _books;

    public void AddBook(Book book) => _books.Add(book);
    
    public void UpdateBook(Book book) 
    {
        var index = _books.FindIndex(b => b.Title == book.Title);
        if (index >= 0)
        {
            _books[index] = book;
        }
    }

    public void DeleteBook(Book book) => _books.Remove(book);

    private List<Book> SeedBooks()
    {
        return new List<Book>
        {
            new Book("The Hobbit", "J.R.R. Tolkien", 1937, 295),
            new Book("The Lord of the Rings", "J.R.R. Tolkien", 1954, 1178),
            new Book("The Silmarillion", "J.R.R. Tolkien", 1977, 1178),
            new Book("The Fellowship of the Ring", "J.R.R. Tolkien", 1954, 1178),
            new Book("The Two Towers", "J.R.R. Tolkien", 1954, 1178),
            new Book("The Return of the King", "J.R.R. Tolkien", 1954, 1178),
            new Book("The Silmarillion", "J.R.R. Tolkien", 1977, 1178),
            new Book("The Silmarillion", "J.R.R. Tolkien", 1977, 1178)
        };
    }
}
