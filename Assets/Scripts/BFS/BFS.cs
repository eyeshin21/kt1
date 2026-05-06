using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class BFS
{
    // These arrays are used to get row and column
    // numbers of 4 neighbours of a given cell
    static int[] rowNum = { -1, 0, 0, 1 };
    static int[] colNum = { 0, -1, 1, 0 };

    // check whether given cell (row, col) 
    // is a valid cell or not.
    static bool isValid(int x, int y, int width, int height)
    {
        // return true if row number and 
        // column number is in range
        return (x >= 0) && (x < width) &&
               (y >= 0) && (y < height);
    }

    static int Distance(Point start, Point end)
    {
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
    }
    
    // function to find the shortest path between
    // a given source cell to a destination cell.
    public static List<Point> Search(bool[,] grid, Point src)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        List<Point> points = new List<Point>();
        points.Add(src);
        if (src.y == height - 1)
        {
            return points;
        }

        bool[,] visited = new bool[width, height];

        // Mark the source cell as visited
        visited[src.x, src.y] = true;

        // Create a queue for BFS
        Queue<QueueNode> q = new Queue<QueueNode>();

        // Distance of source cell is 0
        QueueNode s = new QueueNode(src, 0);
        q.Enqueue(s); // Enqueue source cell

        // Do a BFS starting from source cell
        while (q.Count != 0)
        {
            QueueNode curr = q.Peek();
            Point pt = curr.pt;

            // If we have reached the destination cell,
            // we are done
            if (pt.y == height - 1 && grid[pt.x, pt.y])
            {
                Point lastPt = null;

                int index = 1;
                while (points.Count - index >= 0)
                {
                    int tempIndex = points.Count - index;
                    if (points[tempIndex].x == pt.x && points[tempIndex].y == pt.y)
                    {
                        lastPt = points[tempIndex];
                        break;
                    }
                    index++;
                }

                List<Point> pointRemoves = new List<Point>();
                int startLoopID = points.Count - (index + 1);
                if (startLoopID >= 0)
                {
                    for (int i = points.Count - 1; i >= 0; i--)
                    {
                        if (points.Count - i < index)
                        {
                            pointRemoves.Add(points[i]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = startLoopID; i >= 0; i--)
                    {
                        if (Distance(lastPt, points[i]) != 1)
                        {
                            pointRemoves.Add(points[i]);
                        }
                        else
                        {
                            lastPt = points[i];
                        }
                    }
                }

                for (int i = 0; i < pointRemoves.Count; i++)
                {
                    points.Remove(pointRemoves[i]);
                }
                return points;
            }

            // Otherwise dequeue the front cell 
            // in the queue and enqueue
            // its adjacent cells
            q.Dequeue();

            int countValid = 0;

            for (int i = 0; i < 4; i++)
            {
                int dx = pt.x + rowNum[i];
                int dy = pt.y + colNum[i];

                // if adjacent cell is valid, has path 
                // and not visited yet, enqueue it.
                if (isValid(dx, dy, width, height) && grid[dx, dy] && !visited[dx, dy])
                {
                    countValid++;
                    // mark cell as visited and enqueue it
                    visited[dx, dy] = true;
                    Point point = new Point(dx, dy);
                    QueueNode Adjcell = new QueueNode(point, curr.dist + 1);
                    points.Add(point);
                    q.Enqueue(Adjcell);
                }
            }

            if (countValid == 0)
            {
                points.Remove(pt);
            }
        }

        // Return -1 if destination cannot be reached
        return null;
    }    
    
    // function to find the shortest path between
    // a given source cell to a destination cell.
    public static List<Point> Search(bool[,] grid, Point src, Point target)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        List<Point> points = new List<Point>();
        points.Add(src);
        if (src.x == target.x && src.y == target.y)
        {
            return points;
        }

        bool[,] visited = new bool[width, height];

        // Mark the source cell as visited
        visited[src.x, src.y] = true;

        // Create a queue for BFS
        Queue<QueueNode> q = new Queue<QueueNode>();

        // Distance of source cell is 0
        QueueNode s = new QueueNode(src, 0);
        q.Enqueue(s); // Enqueue source cell

        // Do a BFS starting from source cell
        while (q.Count != 0)
        {
            QueueNode curr = q.Peek();
            Point pt = curr.pt;

            // If we have reached the destination cell,
            // we are done
            if (pt.x == target.x && pt.y == target.y && grid[pt.x, pt.y])
            {
                Point lastPt = null;

                int index = 1;
                while (points.Count - index >= 0)
                {
                    int tempIndex = points.Count - index;
                    if (points[tempIndex].x == pt.x && points[tempIndex].y == pt.y)
                    {
                        lastPt = points[tempIndex];
                        break;
                    }
                    index++;
                }

                List<Point> pointRemoves = new List<Point>();
                int startLoopID = points.Count - (index + 1);
                if (startLoopID >= 0)
                {
                    for (int i = points.Count - 1; i >= 0; i--)
                    {
                        if (points.Count - i < index)
                        {
                            pointRemoves.Add(points[i]);
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = startLoopID; i >= 0; i--)
                    {
                        if (Distance(lastPt, points[i]) != 1)
                        {
                            pointRemoves.Add(points[i]);
                        }
                        else
                        {
                            lastPt = points[i];
                        }
                    }
                }

                for (int i = 0; i < pointRemoves.Count; i++)
                {
                    points.Remove(pointRemoves[i]);
                }
                return points;
            }

            // Otherwise dequeue the front cell 
            // in the queue and enqueue
            // its adjacent cells
            q.Dequeue();

            int countValid = 0;

            for (int i = 0; i < 4; i++)
            {
                int dx = pt.x + rowNum[i];
                int dy = pt.y + colNum[i];

                // if adjacent cell is valid, has path 
                // and not visited yet, enqueue it.
                if (isValid(dx, dy, width, height) && grid[dx, dy] && !visited[dx, dy])
                {
                    countValid++;
                    // mark cell as visited and enqueue it
                    visited[dx, dy] = true;
                    Point point = new Point(dx, dy);
                    QueueNode Adjcell = new QueueNode(point, curr.dist + 1);
                    points.Add(point);
                    q.Enqueue(Adjcell);
                }
            }

            if (countValid == 0)
            {
                points.Remove(pt);
            }
        }

        // Return -1 if destination cannot be reached
        return null;
    }
    
    public static List<Point> SearchShooter(bool[,] grid, Point src)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        List<Point> points = new List<Point>();

        bool[,] visited = new bool[width, height];

        // Mark the source cell as visited
        visited[src.x, src.y] = true;

        // Create a queue for BFS
        Queue<QueueNode> q = new Queue<QueueNode>();

        // Distance of source cell is 0
        QueueNode s = new QueueNode(src, 0);
        q.Enqueue(s); // Enqueue source cell

        // Do a BFS starting from source cell
        while (q.Count != 0)
        {
            QueueNode curr = q.Peek();
            Point pt = curr.pt;

            q.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int dx = pt.x + rowNum[i];
                int dy = pt.y + colNum[i];

                // if adjacent cell is valid, has path 
                // and not visited yet, enqueue it.
                if (isValid(dx, dy, width, height) && !visited[dx, dy])
                {
                    Point point = new Point(dx, dy);
                    visited[dx, dy] = true;

                    if (grid[dx, dy])
                    {
                        QueueNode Adjcell = new QueueNode(point, curr.dist + 1);
                        q.Enqueue(Adjcell);
                    }
                    else
                    {
                        points.Add(point);
                    }
                }
            }
        }

        return points;
    }
}
