using SampleProject.Domain.Exceptions;

namespace SampleProject.Domain.ValueObjects;

/// <summary>
/// 地址值物件 - 封裝地址資訊和驗證邏輯
/// </summary>
public record Address
{
    public string Street { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }

    // 私有建構函式（用於 Dapper）
    private Address()
    {
        Street = string.Empty;
    }

    public Address(string street, string? city = null, string? state = null,
                   string? postalCode = null, string? country = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentNullException(nameof(street));

        if (street.Length > ValidationRules.Address.MaxStreetLength)
            throw new DomainException($"Street cannot exceed {ValidationRules.Address.MaxStreetLength} characters");

        if (city != null && city.Length > ValidationRules.Address.MaxCityLength)
            throw new DomainException($"City cannot exceed {ValidationRules.Address.MaxCityLength} characters");

        if (state != null && state.Length > ValidationRules.Address.MaxStateLength)
            throw new DomainException($"State cannot exceed {ValidationRules.Address.MaxStateLength} characters");

        if (postalCode != null && postalCode.Length > ValidationRules.Address.MaxPostalCodeLength)
            throw new DomainException($"Postal code cannot exceed {ValidationRules.Address.MaxPostalCodeLength} characters");

        if (country != null && country.Length > ValidationRules.Address.MaxCountryLength)
            throw new DomainException($"Country cannot exceed {ValidationRules.Address.MaxCountryLength} characters");

        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Street);

    public override string ToString()
    {
        var parts = new[] { Street, City, State, PostalCode, Country }
            .Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(", ", parts);
    }
}
