using System.Collections.ObjectModel;

namespace OOPWPFProject.Models.PlaceRelated;

public interface IReviewable
{

    public ObservableCollection<KeyValuePair<string, double?>> Reviews
    {
        get;
    }

    public double? Rating
    {
        get; set;
    }

    public abstract void AddReview( string review );
    public abstract void RemoveReview( string review );
    public abstract double GetAverageRating();
}
