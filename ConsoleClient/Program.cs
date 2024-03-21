// This project is used for testing

using ConsoleClient;
using DatabaseClient.Contexts;
using DatabaseClient.Managers;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
using Domain;
using Serilog;

Logging.Init();

DatabaseContext.LogIn("bsbd_owner", "very_secret_Password_forOwner");

var clientsRepository = new ClientsRepository();
var tagsRepository = new TagsRepository();
var booksRepository = new BooksRepository();
var reviewsRepository = new ReviewsRepository();
var ordersRepository = new OrdersRepository();
var usersManager = new UsersManager();
var reportsProvider = new ReportsProvider();

DatabaseContext.LogOff();
Logging.Close();