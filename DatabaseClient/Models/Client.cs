using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DatabaseClient.Models;

[Serializable]
[SuppressMessage("Naming", "CA1724:Type names should not match namespaces")]
public class Client : IEntity
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public Gender Gender { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}