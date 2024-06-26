// This project is used for testing

using ConsoleClient;
using DatabaseClient.Contexts;
using DatabaseClient.Options;
using DatabaseClient.Repositories;
using Domain;

Logging.Init();

var factory = new DbContextFactory(Startup.Cred, new ServerOptions());

var clientsRepository = new ClientsRepository(factory);
var tagsRepository = new TagsRepository(factory);
var booksRepository = new BooksRepository(factory);
var reviewsRepository = new ReviewsRepository(factory, Startup.Mapper);
var ordersRepository = new OrdersRepository(factory, Startup.Mapper);
var principalsManager = new PrincipalRepository(factory);

await Startup.InitDatabaseAsync();

Logging.Close();