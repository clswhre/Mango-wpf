namespace OOPWPFProject.Models.Helpers;

public class EntityManager<T>
{
    private readonly List<T> values = [];

    public T this[int idx]
    {
        get {
            if (idx < 0 || idx >= values.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                return values[idx];
            }
        }
        set
        {
            if (idx < 0 || idx >= values.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            values[idx] = value;
        }
    }

    public void Add(T place)
    {
        values.Add(place);
    }

    public bool Remove(T place)
    {
        return values.Remove(place);
    }

    public IEnumerable<T> GetAll()
    {
        return values;
    }

    public void DisplayAll()
    {
        foreach (var place in values)
        {
            Console.WriteLine(place.ToString());
        }
    }

}
