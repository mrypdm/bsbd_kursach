using DatabaseClient.Models;

namespace DatabaseClient.Reports;

public class RevenueClient(Client client, int totalSum)
{
    public Client Client { get; } = client;

    public int TotalSum { get; } = totalSum;
}