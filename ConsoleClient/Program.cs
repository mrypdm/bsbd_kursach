// This project is used for testing

using System.Globalization;
using DatabaseClient.Contexts;
using DatabaseClient.Managers;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
using Serilog;
using Serilog.Events;

const string format = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";
const string filePath = "./logs/log.txt";

var loggerConfiguration = new LoggerConfiguration()
    .WriteTo.Async(p => p.Console(LogEventLevel.Information, format, CultureInfo.InvariantCulture))
    .WriteTo.Async(p => p.File(filePath, LogEventLevel.Debug, format, CultureInfo.InvariantCulture));
Log.Logger = loggerConfiguration.CreateLogger();

DatabaseContext.LogIn("bsbd_owner", "very_secret_Password_forOwner");

var clientsRepository = new ClientsRepository();
var tagsRepository = new TagsRepository();
var booksRepository = new BooksRepository();
var reviewsRepository = new ReviewsRepository();
var ordersRepository = new OrdersRepository();
var usersManager = new UsersManager();
var reportsProvider = new ReportsProvider();

DatabaseContext.LogOff();
Log.CloseAndFlush();
