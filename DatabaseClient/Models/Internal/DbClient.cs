using System;

namespace DatabaseClient.Models.Internal;

[Serializable]
public class DbClient : IDbEntity<Client>
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public Gender Gender { get; set; }

    public bool IsDeleted { get; set; }

    public Client ToEntity()
    {
        return new Client
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            Phone = Phone,
            Gender = Gender,
            IsDeleted = IsDeleted
        };
    }
}