using System.Text.RegularExpressions;
namespace OOPWPFProject.Models.Storage;


public class PlaceDto
{
    public PlaceType Type { get; set; }

    public string Name {  get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;


    // Historical
    public DateOnly? YearBuilt { get; set; }
    public int? Significance { get; set; }

    // Natural
    public DateOnly? YearFormed { get; set; }
    public bool? ProtectedStatus {  get; set; }

    public static PlaceDto FromPlace(Place p) => p switch
    {
        HistoricalPlace historical => new PlaceDto
        {
            Type = PlaceType.Historical,
            Name = historical.Name,
            Country = historical.Country,
            YearBuilt = historical.YearBuilt,
            Significance = historical.Significance
        },
        NaturalPlace natural => new PlaceDto
        {
            Type = PlaceType.Natural,
            Name = natural.Name,
            Country = natural.Country,
            YearFormed = natural.YearFormed,
            ProtectedStatus = natural.ProtectedStatus
        },
        Place place => new PlaceDto
        {
            Type = PlaceType.Normal,
            Name = place.Name,
            Country = place.Country
        },
        _ => throw new InvalidOperationException(
                 $"Невідомий тип Place для серіалізації: {p.GetType().Name}")
    };

    /// <summary>Зворотне перетворення: DTO → доменний об'єкт.</summary>
    public Place ToPlace() => Type switch
    {
        PlaceType.Historical => new HistoricalPlace(
            Name, Country, Description,
            YearBuilt ?? null,
            Significance ?? null),

        PlaceType.Natural => new NaturalPlace(
            Name, Country, Description,
            YearFormed ?? null,
            ProtectedStatus ?? null),

        PlaceType.Normal => new Place(
            Name, Country, Description),

        _ => throw new InvalidOperationException(
                 $"Невідоме значення Type: {Type}")
    };


}
