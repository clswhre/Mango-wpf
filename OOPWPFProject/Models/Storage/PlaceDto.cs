using Microsoft.Data.Sqlite;

namespace OOPWPFProject.Models.Storage;

public class PlaceDto
{
    public PlaceType Type { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? Rating { get; set; } = null;
    public DateOnly? Date { get; set; } = null;
    public string? Review { get; set; } = string.Empty;
    public string? Notes { get; set; } = string.Empty;
    public string? ReviewsJson { get; set; } = string.Empty;

    // Historical
    public DateOnly? YearBuilt { get; set; }
    public int? Significance { get; set; }

    // Natural
    public DateOnly? YearFormed { get; set; }
    public bool? ProtectedStatus { get; set; }

    public static PlaceDto FromPlace(Place p)
    {
        var dto = new PlaceDto
        {
            Name = p.Name,
            Country = p.Country,
            Description = p.Description,
            Rating = p.Rating.HasValue ? (int)p.Rating.Value : null,
            Date = p.Date,
            Review = p.Review,
            Notes = p.Notes,
            ReviewsJson = p.Reviews != null ? System.Text.Json.JsonSerializer.Serialize(p.Reviews) : null
        };

        switch (p)
        {
            case HistoricalPlace h:
                dto.Type = PlaceType.Historical;
                dto.YearBuilt = h.YearBuilt;
                dto.Significance = h.Significance;
                break;
            case NaturalPlace n:
                dto.Type = PlaceType.Natural;
                dto.YearFormed = n.YearFormed;
                dto.ProtectedStatus = n.ProtectedStatus;
                break;
            default:
                dto.Type = PlaceType.Normal;
                break;
        }
        return dto;
    }

    public Place ToPlace()
    {
        Place place = Type switch
        {
            PlaceType.Historical => new HistoricalPlace { YearBuilt = YearBuilt, Significance = Significance ?? 0 },
            PlaceType.Natural => new NaturalPlace { YearFormed = YearFormed, ProtectedStatus = ProtectedStatus },
            _ => new Place()
        };

        place.Name = Name;
        place.Country = Country;
        place.Description = Description;
        place.Rating = Rating;
        place.Date = Date;
        place.Review = Review;
        place.Notes = Notes;

        if (!string.IsNullOrWhiteSpace(ReviewsJson))
        {
            List<KeyValuePair<string, double?>>? reviews = System.Text.Json.JsonSerializer.Deserialize<List<KeyValuePair<string, double?>>>(ReviewsJson);
            if (reviews is not null)
            {
                place.Reviews.Clear();
                foreach (KeyValuePair<string, double?> review in reviews)
                {
                    place.Reviews.Add(review);
                }
            }
        }

        return place;
    }

}
