# WebHooks

A .NET 8 solution with two projects demonstrating a simple **webhook publish-subscribe pattern**. The **Server** API manages book data and provides a webhook subscription system; the **Client** API is a standalone weather forecast service.

## Projects

### WebHooksTest.Server

The server exposes two capabilities:

1. **Book management** — `BookService` maintains an in-memory list of Tolkien books with CRUD operations (`GetBooks`, `AddBook`, `UpdateBook`, `DeleteBook`).
2. **Webhook subscriptions** — `WebHookService` accepts subscriber registrations via `POST /api/webhook/subscribe` with a `Subscription` (topic + callback URL). When a message is published for that topic, the service fires HTTP POST callbacks to all registered subscribers concurrently using `HttpClient`.

### WebHooksTest.Client

A minimal weather forecast API (similar to the default ASP.NET Core template) that returns 5-day forecast data with random temperatures and summaries. This project serves as an example target for webhook callbacks.

## Technologies / Libraries

- **.NET 8** (ASP.NET Core Minimal API)
- **Swashbuckle.AspNetCore** 6.4.0 — Swagger UI
- **Microsoft.AspNetCore.OpenApi** 8.0.4

## Key Features

- **Pub-sub model** — subscribers register by topic; notifications fan out to all subscribers.
- **In-memory book catalog** — seeded with J.R.R. Tolkien titles.
- **Concurrent webhook delivery** — uses `Task.WhenAll` to fire all callbacks in parallel.
- **Two independent services** — server and client run as separate processes.

## How to Run

1. Start the **Server**:
   ```bash
   dotnet run --project WebHooksTest.Server
   ```
2. Start the **Client** (in a separate terminal):
   ```bash
   dotnet run --project WebHooksTest.Client
   ```
3. Subscribe to a topic by sending a POST to the server's `/api/webhook/subscribe`:
   ```json
   { "topic": "books", "callback": "http://localhost:5053/weatherforecast" }
   ```
4. When the server notifies subscribers of a "books" event, the client's endpoint receives the callback.
