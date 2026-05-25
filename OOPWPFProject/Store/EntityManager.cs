namespace OOPWPFProject.Store;

public class EntityManager<T>
{
	private readonly List<T> values = [];

	public T this[int idx]
	{
		get =>
			idx < 0 || idx >= values.Count ? throw new ArgumentOutOfRangeException() : values[idx];
		set
		{
			if (idx < 0 || idx >= values.Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			values[idx] = value;
		}
	}

	public void Add(T item) => values.Add(item);

	public bool Remove(T item) => values.Remove(item);

	public IEnumerable<T> GetAll() => values;
}
