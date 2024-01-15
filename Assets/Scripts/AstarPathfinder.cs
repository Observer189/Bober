using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Статический класс для вызова поиска пути с поммощью алгоритма A*
    /// </summary>
    public static class AstarPathfinder
    {
        public static List<Vector2Int> Pathfind(Vector2Int startPos, Vector2Int targetPos,  CellState[,] grid)
        {
            FastPriorityQueue<AstarNode> openSet = new FastPriorityQueue<AstarNode>(grid.GetLength(0)
                * grid.GetLength(1));
            Dictionary<Vector2Int, AstarNode > visited = new Dictionary<Vector2Int, AstarNode>(grid.GetLength(0)
                * grid.GetLength(1));

            var startNode = new AstarNode() {position = startPos,cost = 0};
            openSet.Enqueue(startNode,0);
            visited[startPos] = startNode;
           
            while (openSet.Count > 0)
            {
                var cur = openSet.Dequeue();
                //Debug.Log(cur.position);
                if (cur.position == targetPos)
                {
                    var resList = new List<Vector2Int>();
                    while (true)
                    {
                        resList.Add(cur.position);
                        if (cur.prev == null)
                        {
                            return resList;
                        }
                        else
                        {
                            cur = cur.prev;
                        }
                    }
                }
                var neighbors = GetNeighbors(cur.position,grid);
                foreach (var neighbor in neighbors)
                {
                    if (visited.TryGetValue(neighbor,out AstarNode n))
                    {
                        if (n.cost > cur.cost + 1)
                        {
                            n.cost = (byte)(cur.cost + 1);
                            n.prev = cur;
                            var f = cur.cost + 1 + (targetPos-n.position).ManhattanMagnitude();
                            openSet.UpdatePriority(n,f);
                        }
                    }
                    else
                    {
                        var node = new AstarNode() {  position = neighbor, prev = cur,cost = (cur.cost+1)};
                        visited[neighbor] = node;
                        openSet.Enqueue(node, cur.cost + 1 + (targetPos-neighbor).ManhattanMagnitude());
                    }
                }
            }
            return null;
        }

       static List<Vector2Int> GetNeighbors(Vector2Int pos, CellState[,] grid)
       {
           var neighbors = new List<Vector2Int>();
           Vector2Int[] directions = new[]
               { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
           for (int i = 0; i < directions.Length; i++)
           {
               var neighbor = pos + directions[i];
               if (neighbor.x >=0 && neighbor.x < grid.GetLength(0) && neighbor.y >=0 &&
                   neighbor.y < grid.GetLength(1) && grid[neighbor.x, neighbor.y] != CellState.Obstacle)
               {
                   neighbors.Add(neighbor);
               }
           }

           return neighbors;
       }
    }
}

public class AstarNode:FastPriorityQueueNode
{
    public Vector2Int position;
    public int cost;
    public AstarNode prev;
}