using System;
using System.Linq;

namespace DatabaseClient.Extensions;

public static class StringExtensions
{
    private const string ForbiddenCharacters = "[]'\";";

    public static void ValidateForSqlInjection(this string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Intersect(ForbiddenCharacters).Any())
        {
            throw new ArgumentException($"Invalid SQL. String cannot contains characters: {ForbiddenCharacters}");
        }
    }
}