using OOPWPFProject.Models.Places;

namespace OOPWPFProject.Services.DB;

public class PlaceDto
{
	public int Id { get; set; }
	public PlaceType Type { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Country { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public double? Rating { get; set; } = null;
	public DateOnly? Date { get; set; } = null;
	public bool IsVisited { get; set; }

	// Historical
	public int? YearBuilt { get; set; }
	public int? Significance { get; set; }

	// Natural
	public int? YearFormed { get; set; }
	public bool? ProtectedStatus { get; set; }

	public static PlaceDto FromPlace(Place place)
	{
		ArgumentNullException.ThrowIfNull(place);

		var dto = new PlaceDto
		{
			Id = place.Id,
			Name = place.Name,
			Country = place.Country,
			Description = place.Description,
			Rating = place.Rating,
			Date = place.Date,
			IsVisited = place.IsVisited,
		};

		switch (place)
		{
			case HistoricalPlace historical:
				dto.Type = PlaceType.Historical;
				dto.YearBuilt = historical.YearBuilt;
				dto.Significance = historical.Significance;
				break;
			case NaturalPlace natural:
				dto.Type = PlaceType.Natural;
				dto.YearFormed = natural.YearFormed;
				dto.ProtectedStatus = natural.ProtectedStatus;
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
			PlaceType.Historical => new HistoricalPlace
			{
				YearBuilt = YearBuilt,
				Significance = Significance ?? 0,
			},
			PlaceType.Natural => new NaturalPlace
			{
				YearFormed = YearFormed,
				ProtectedStatus = ProtectedStatus,
			},
			_ => new NormalPlace(),
		};

		place.Id = Id;
		place.Name = Name;
		place.Country = Country;
		place.Description = Description;
		place.Rating = Rating;
		place.Date = Date;
		place.IsVisited = IsVisited;

		return place;
	}
}
