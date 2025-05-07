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
        /// 정보 요약
        /// - 이 함수는 유닛의 이동경로를 지정하는 함수입니다.
        /// - currentUnit : 길찾기를 진행할 유닛입니다.
        /// - attackerUnits : 공격을 진행할 유닛입니다.
        /// - defenderUntis : 방어를 진행할 유닛입니다.
        MapDirector = ArmyDataManager.Instance.MapDirector;


        // 1. 유닛  기준으로 pixel position을 잡음

        Battle_MapPixel currentUnitPixel = MapDirector.GetPixel(currentUnit.transform.position);
        Vector2Int startIndex = currentUnitPixel.PixelIndex;

        BattleBaseUnit targetUnit = currentUnit.GetUnit_Status().TargetUnit;
        Battle_MapPixel targetUnitPixel = MapDirector.GetPixel(targetUnit.transform.position);
        Vector2Int targetIndex = targetUnitPixel.PixelIndex;


        // 2. 유닛 위치 기준 장애물 맵 구성
        List<Vector2Int> mapData = new List<Vector2Int>();
        List<Vector2Int> occupied = new List<Vector2Int>();

        foreach (var pixel in pixelMap)
        {
            mapData.Add(pixel.PixelIndex);
        }

        // 현재 유닛 제외한 나머지 장애물 등록
        foreach (var unit in attackerUnits)
        {
            if (unit != currentUnit)
            {
                Battle_MapPixel unitPixel = MapDirector.GetPixel(unit.transform.position);
                Vector2Int unitIndex = unitPixel.PixelIndex;
                occupied.Add(unitIndex);
            }
        }

        // 상대 유닛 장애물 등록
        foreach (var unit in defenderUnits)
        {
            if (unit != targetUnit)
            {
                Battle_MapPixel unitPixel = MapDirector.GetPixel(unit.transform.position);
                Vector2Int unitIndex = unitPixel.PixelIndex;
                occupied.Add(unitIndex);
            }
        }
        // 3. A* 알고리즘 수행
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

                int tentativeG = gScore[current] + ((dir.x != 0 && dir.y != 0) ? 14 : 10); // 대각선 14, 직선 10

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

        // 경로 추적
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int trace = target;

        if (!parent.ContainsKey(trace) && trace != start)
            return path; // 경로 없음

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
        return 10 * (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)); // Manhattan 거리
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

