// This project is used for testing

using ConsoleClient;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Options;
using DatabaseClient.Principals;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
using Domain;

Logging.Init();

using var cred = new Principal("bsbd_owner", "very_secret_Password_forOwner".AsSecure(), Role.Owner);

var factory = new DatabaseContextFactory(cred, new ServerOptions());

var clientsRepository = new ClientsRepository(factory);
var tagsRepository = new TagsRepository(factory);
var booksRepository = new BooksRepository(factory);
var reviewsRepository = new ReviewsRepository(factory);
var ordersRepository = new OrdersRepository(factory);
var principalsManager = new PrincipalsManager(factory);
var reportsProvider = new ReportsProvider(factory);

await Startup.ClearAllAsync(cred.Name, cred.Password);
await Startup.InitDatabaseAsync(cred.Name, cred.Password);

Logging.Close();