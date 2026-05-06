[System.Serializable]
public class QueueNode
{
    // The coordinates of a cell
    public Point pt;

    // cell's distance of from the source
    public int dist;

    public QueueNode(Point pt, int dist)
    {
        this.pt = pt;
        this.dist = dist;
    }
}
