// This project is used for testing

using System.Net;
using ConsoleClient;
using DatabaseClient.Contexts;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
using DatabaseClient.Users;
using Domain;

Logging.Init();

var username = "bsbd_owner";
var password = "very_secret_Password_forOwner";

var factory = new DatabaseContextFactory(new NetworkCredential(username, password));

var clientsRepository = new ClientsRepository(factory);
var tagsRepository = new TagsRepository(factory);
var booksRepository = new BooksRepository(factory);
var reviewsRepository = new ReviewsRepository(factory);
var ordersRepository = new OrdersRepository(factory);
var usersManager = new UsersManager(factory);
var reportsProvider = new ReportsProvider(factory);

Logging.Close();