// This project is used for testing

using ConsoleClient;
using DatabaseClient.Contexts;
using DatabaseClient.Options;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
using Domain;

Logging.Init();

var factory = new DatabaseContextFactory(Startup.Cred, new ServerOptions());

var clientsRepository = new ClientsRepository(factory);
var tagsRepository = new TagsRepository(factory);
var booksRepository = new BooksRepository(factory);
var reviewsRepository = new ReviewsRepository(factory);
var ordersRepository = new OrdersRepository(factory);
var principalsManager = new PrincipalRepository(factory);
var reportsProvider = new ReportsProvider(factory);

await Startup.ClearAllAsync();
await Startup.InitDatabaseAsync();

Logging.Close();