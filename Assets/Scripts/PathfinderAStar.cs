using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathfinderAStar : MonoBehaviour
{
    public MapManager mapManager;
    public float moveSpeed = 15f;

    private List<Vector2Int> path;
    private int pathIdx = 0;

    private class Node
    {
        public Vector2Int position;
        public Node parent;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;

        public Node(Vector2Int pos, Node par = null)
        {
            position = pos;
            parent = par;
            gCost = 0;
            hCost = 0;
        }
    }

    public void StartFindingPath()
    {
        transform.position = new Vector2(mapManager.GetStartPos().x * mapManager.cellSize, -mapManager.GetStartPos().y * mapManager.cellSize);
        FindPath(mapManager.GetStartPos(), mapManager.GetGoalPos());
    }

    private void FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        List<Node> openSet = new();
        HashSet<Vector2Int> closedSet = new();
        Node startNode = new(startPos);
        Node targetNode = new(targetPos);

        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.OrderBy(node => node.fCost).First();
            openSet.Remove(currentNode);
            closedSet.Add(currentNode.position);

            if (currentNode.position == targetPos)
            {
                RetracePath(currentNode);
                return;
            }

            int[] dx = { 0, 0, 1, -1 };
            int[] dy = { 1, -1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                Vector2Int neighborPos = new(currentNode.position.x + dx[i], currentNode.position.y + dy[i]);
                if (mapManager.IsWall(neighborPos.x, neighborPos.y) || closedSet.Contains(neighborPos)) continue;

                int newGCost = currentNode.gCost + 1;
                Node neighborNode = new(neighborPos, currentNode);
                neighborNode.gCost = newGCost;
                neighborNode.hCost = Mathf.Abs(neighborPos.x - targetPos.x) + Mathf.Abs(neighborPos.y - targetPos.y);

                Node existingNeighbor = openSet.FirstOrDefault(node => node.position == neighborPos);
                if (existingNeighbor == null || newGCost < existingNeighbor.gCost)
                {
                    if (existingNeighbor != null)
                        openSet.Remove(existingNeighbor);

                    openSet.Add(neighborNode);
                }
            }
        }
        Debug.Log("Path not found!");
    }

    private void RetracePath(Node endNode)
    {
        path = new List<Vector2Int>();
        Node currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
    }

    private void Update()
    {
        if (mapManager.isStarted && path != null && pathIdx < path.Count)
        {
            Vector2 targetPos = new(path[pathIdx].x * mapManager.cellSize, -path[pathIdx].y * mapManager.cellSize);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPos) < 0.01f)
            {
                Cell visitedCell = mapManager.GetCellAtPos(path[pathIdx].x, path[pathIdx].y);
                if (visitedCell != null && visitedCell.type != 3)
                {
                    visitedCell.type = 4;
                    visitedCell.sp.sprite = mapManager.visitedSprite;
                }

                pathIdx++;
                if (pathIdx == path.Count)
                    Debug.Log("Finish!");
            }
        }
    }
}
