using System.Text.RegularExpressions;
using SumX.Domain.Exceptions;

namespace SumX.Domain.ValueObjects;

public sealed class EmailAddress : IEquatable<EmailAddress>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Email address cannot be empty.");
        }

        var normalized = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
        {
            throw new DomainException("Email address is invalid.");
        }

        Value = normalized;
    }

    public static EmailAddress Create(string value) => new(value);

    public static implicit operator string(EmailAddress email) => email.Value;

    public bool Equals(EmailAddress? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) =>
        obj is EmailAddress other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
