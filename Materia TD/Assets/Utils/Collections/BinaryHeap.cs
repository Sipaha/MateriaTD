using System.Collections.Generic;

public class BinaryHeap<T> where T : class, new() {

    public struct Node
    {
        public T data;
        public float value;

        public Node(T obj, float value)
        {
            data = obj;
            this.value = value;
        }

        public override bool Equals(object obj)
        {
			return obj is Node && ((Node)obj).data.Equals(data);
        }

        public override int GetHashCode()
        {
            return data.GetHashCode();
        }
    }

    private List<Node> list = new List<Node>();

    public int Size
    {
        get
        {
            return list.Count;
        }
    }

    public bool Add(T obj, float value)
    {
        T t = new T();
        return Add(obj, value, out t);
    }

    public bool Add(T obj, float value, out T replaced)
    {
        Node node = new Node(obj, value);
        int idx = list.IndexOf(node);
        replaced = default(T);
        if (idx >= 0)
        {
            Node equalNode = list[idx];
            if (equalNode.value > value)
            {
                list[idx] = node;
                Heapify(idx);
                replaced = equalNode.data;
                return true;
            }
            return false;
        }
        else
        {
            list.Add(node);
            int i = list.Count - 1;
            int parent = (i - 1) / 2;

            while (i > 0 && list[parent].value > list[i].value)
            {
                Node temp = list[i];
                list[i] = list[parent];
                list[parent] = temp;

                i = parent;
                parent = (i - 1) / 2;
            }
            return true;
        }
    }

    public void Heapify(int i)
    {
        for (; ; )
        {
            int leftChild = 2 * i + 1;
            int rightChild = 2 * i + 2;
            int smallestChild = i;

            if (leftChild < list.Count && list[leftChild].value < list[smallestChild].value)
            {
                smallestChild = leftChild;
            }

            if (rightChild < list.Count && list[rightChild].value < list[smallestChild].value)
            {
                smallestChild = rightChild;
            }

            if (smallestChild == i) break;

            Node temp = list[i];
            list[i] = list[smallestChild];
            list[smallestChild] = temp;
            i = smallestChild;
        }
    }

    public T Pop()
    {
        Node result = list[0];
        int last = list.Count - 1;
        list[0] = list[last];
        list.RemoveAt(last);
        Heapify(0);
        return result.data;
    }

    public void Clear()
    {
        list.Clear();
    }
}
