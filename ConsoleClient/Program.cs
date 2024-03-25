// This project is used for testing

using ConsoleClient;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Options;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
using Domain;

Logging.Init();

using var cred = new DbPrincipal();
cred.Name = "bsbd_owner";
cred.SecurePassword = "very_secret_Password_forOwner".AsSecure();

var factory = new DatabaseContextFactory(cred, new ServerOptions());

var clientsRepository = new ClientsRepository(factory);
var tagsRepository = new TagsRepository(factory);
var booksRepository = new BooksRepository(factory);
var reviewsRepository = new ReviewsRepository(factory);
var ordersRepository = new OrdersRepository(factory);
var principalsManager = new PrincipalRepository(factory);
var reportsProvider = new ReportsProvider(factory);

await Startup.ClearAllAsync(cred.Name, cred.Password);
await Startup.InitDatabaseAsync(cred.Name, cred.Password);

Logging.Close();