using System;

namespace DatabaseClient.Models.Internal;

[Serializable]
public class DbOrder
{
    public int OrderId { get; set; }

    public int ClientId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public Gender Gender { get; set; }

    public bool IsClientDeleted { get; set; }
}