// This project is used for testing

using ConsoleClient;
using DatabaseClient.Contexts;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
using DatabaseClient.Users;
using Domain;

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