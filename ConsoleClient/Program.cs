// This project is used for testing

using System.Net;
using DatabaseClient.Contexts;
using DatabaseClient.Options;
using DatabaseClient.Principals;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
using Domain;

Logging.Init();

var cred = new NetworkCredential("bsbd_owner", "very_secret_Password_forOwner");

var factory = new DatabaseContextFactory(new CredentialProvider(cred), new ServerOptions());

var clientsRepository = new ClientsRepository(factory);
var tagsRepository = new TagsRepository(factory);
var booksRepository = new BooksRepository(factory);
var reviewsRepository = new ReviewsRepository(factory);
var ordersRepository = new OrdersRepository(factory);
var principalsManager = new PrincipalsManager(factory);
var reportsProvider = new ReportsProvider(factory);

Logging.Close();