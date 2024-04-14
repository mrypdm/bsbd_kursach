using System;

namespace DatabaseClient.Options;

[Serializable]
public class ServerOptions
{
    public string ServerName { get; init; } = "SHAPOVAL-M-NB\\SQLEXPRESS";

    public string DatabaseName { get; init; } = "bsbd_kursach";

    public bool Encryption { get; init; } = true;
}