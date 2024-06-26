﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using DatabaseClient.Extensions;
using DatabaseClient.Providers;

namespace DatabaseClient.Models;

[Serializable]
[SuppressMessage("Naming", "CA1724: Type names should not match namespaces")]
public sealed class Principal : IPrincipalProvider, IDisposable
{
    private SecureString _password;

    public int Id { get; set; }

    public string Name { get; set; }

    public Role Role { get; set; }

    [NotMapped]
    public SecureString SecurePassword
    {
        get => _password;
        set
        {
            _password?.Dispose();
            _password = value;
        }
    }

    [NotMapped]
    public string Password => SecurePassword.Unsecure();

    public void Dispose()
    {
        _password?.Dispose();
    }

    public Principal GetPrincipal()
    {
        return this;
    }

    public override string ToString()
    {
        return $"{Role}/{Name}";
    }
}