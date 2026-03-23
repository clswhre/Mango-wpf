namespace OOPWPFProject.Models;

public class EntityManager<T>
{
    private readonly List<T> places = [];

    public T this[int idx]
    {
        get {
            if (idx < 0 || idx >= places.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                return places[idx];
            }
        }
        set
        {
            if (idx < 0 || idx >= places.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            places[idx] = value;
        }
    }

    public void Add(T place)
    {
        places.Add(place);
    }

    public IEnumerable<T> GetAll()
    {
        return places;
    }

    public void DisplayAll()
    {
        foreach (var place in places)
        {
            Console.WriteLine(place.ToString());
        }
    }

}
