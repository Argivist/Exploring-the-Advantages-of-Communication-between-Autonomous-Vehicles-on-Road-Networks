using System.Linq;



// Unity-compatible Priority Queue (Min Heap)
public class PriorityQueue<TElement, TPriority> where TPriority : System.IComparable<TPriority>
{
    private List<(TElement element, TPriority priority)> elements = new List<(TElement, TPriority)>();

    public int Count => elements.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        elements.Add((element, priority));
        elements.Sort((x, y) => x.priority.CompareTo(y.priority)); // Sort by priority
    }

    public TElement Dequeue()
    {
        var item = elements[0].element;
        elements.RemoveAt(0);
        return item;
    }
}