using System;
using System.Diagnostics.CodeAnalysis;

namespace DatabaseClient.Models;

[Serializable]
[SuppressMessage("Naming", "CA1724:Type names should not match namespaces")]
public class Client
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public Gender Gender { get; set; }

    public bool IsDeleted { get; set; }

    public override string ToString()
    {
        return IsDeleted ? "DELETED CLIENT" : $"{FirstName} {LastName} / {Phone}";
    }
}