using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PathFinder_Astar_Region
{
    struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
            {
                return 0;
            }
            return F < other.F ? 1 : -1;
        }
    }
    private Battle_MapDirector MapDirector;




    public List<Vector2Int> FindPathForUnit(
        BattleBaseUnit currentUnit,
        List<BattleBaseUnit> attackerUnits,
        List<BattleBaseUnit> defenderUnits,
        List<Battle_MapPixel> pixelMap )
    {
        /// ���� ���
        /// - �� �Լ��� ������ �̵���θ� �����ϴ� �Լ��Դϴ�.
        /// - currentUnit : ��ã�⸦ ������ �����Դϴ�.
        /// - attackerUnits : ������ ������ �����Դϴ�.
        /// - defenderUntis : �� ������ �����Դϴ�.
        MapDirector = ArmyDataManager.Instance.MapDirector;


        // 1. ����  �������� pixel position�� ����

        Battle_MapPixel currentUnitPixel = MapDirector.GetPixel(currentUnit.transform.position);
        Vector2Int startIndex = currentUnitPixel.PixelIndex;

        BattleBaseUnit targetUnit = currentUnit.GetUnit_Status().TargetUnit;
        Battle_MapPixel targetUnitPixel = MapDirector.GetPixel(targetUnit.transform.position);
        Vector2Int targetIndex = targetUnitPixel.PixelIndex;


        // 2. ���� ��ġ ���� ��ֹ� �� ����
        List<Vector2Int> mapData = new List<Vector2Int>();
        List<Vector2Int> occupied = new List<Vector2Int>();

        foreach (var pixel in pixelMap)
        {
            mapData.Add(pixel.PixelIndex);
        }

        // ���� ���� ������ ������ ��ֹ� ���
        foreach (var unit in attackerUnits)
        {
            if (unit != currentUnit)
            {
                Battle_MapPixel unitPixel = MapDirector.GetPixel(unit.transform.position);
                Vector2Int unitIndex = unitPixel.PixelIndex;
                occupied.Add(unitIndex);
            }
        }

        // ��� ���� ��ֹ� ���
        foreach (var unit in defenderUnits)
        {
            if (unit != targetUnit)
            {
                Battle_MapPixel unitPixel = MapDirector.GetPixel(unit.transform.position);
                Vector2Int unitIndex = unitPixel.PixelIndex;
                occupied.Add(unitIndex);
            }
        }
        // 3. A* �˰��� ����
        return AStarPathFindWithObstacle(startIndex, targetIndex, mapData, occupied);
    }

    public List<Vector2Int> AStarPathFindWithObstacle(
        Vector2Int start,
        Vector2Int target,
        List<Vector2Int> mapData,
        List<Vector2Int> occupied)
    {
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();

        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        PriorityQueue<PQNode> openQueue = new PriorityQueue<PQNode>();

        gScore[start] = 0;
        fScore[start] = Heuristic(start, target);
        openQueue.Push(new PQNode { X = start.x, Y = start.y, G = 0, F = fScore[start] });

        while (openQueue.Count > 0)
        {
            PQNode node = openQueue.Pop();
            Vector2Int current = new Vector2Int(node.X, node.Y);

            if (closedSet.Contains(current))
                continue;
            closedSet.Add(current);

            if (current == target)
                break;

            foreach (Vector2Int dir in GetDirections())
            {
                Vector2Int neighbor = current + dir;

                if (!mapData.Contains(neighbor) || occupied.Contains(neighbor) || closedSet.Contains(neighbor))
                    continue;

                int tentativeG = gScore[current] + ((dir.x != 0 && dir.y != 0) ? 14 : 10); // �밢�� 14, ���� 10

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    parent[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, target);
                    openQueue.Push(new PQNode
                    {
                        X = neighbor.x,
                        Y = neighbor.y,
                        G = tentativeG,
                        F = fScore[neighbor]
                    });
                }
            }
        }

        // ��� ����
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int trace = target;

        if (!parent.ContainsKey(trace) && trace != start)
            return path; // ��� ����

        while (trace != start)
        {
            path.Add(trace);
            trace = parent[trace];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return 10 * (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)); // Manhattan �Ÿ�
    }

    private List<Vector2Int> GetDirections()
    {
        return new List<Vector2Int>
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, -1),
                    new Vector2Int(1, 1),
                    new Vector2Int(1, -1),
                    new Vector2Int(-1, 1),
                    new Vector2Int(-1, -1)
                };
    }


}

