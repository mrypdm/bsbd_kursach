// This project is used for testing

using DatabaseClient.Contexts;
using DatabaseClient.Managers;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;

DatabaseContext.LogIn("bsbd_main_admin", "123456");

var clientsRepository = new ClientsRepository();
var tagsRepository = new TagsRepository();
var booksRepository = new BooksRepository();
var reviewsRepository = new ReviewsRepository();
var ordersRepository = new OrdersRepository();
var usersManager = new UsersManager();
var reportsProvider = new ReportsProvider();

DatabaseContext.LogOff();