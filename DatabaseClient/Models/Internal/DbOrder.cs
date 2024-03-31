using System;

namespace DatabaseClient.Models.Internal;

[Serializable]
public class DbOrder : IDbEntity<Order>
{
    public int OrderId { get; set; }

    public int ClientId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public Gender Gender { get; set; }

    public bool IsClientDeleted { get; set; }

    public Order ToEntity()
    {
        return new Order
        {
            Id = OrderId,
            ClientId = ClientId,
            CreatedAt = CreatedAt,
            Client = new Client
            {
                Id = ClientId,
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone,
                Gender = Gender,
                IsDeleted = IsClientDeleted
            }
        };
    }
}