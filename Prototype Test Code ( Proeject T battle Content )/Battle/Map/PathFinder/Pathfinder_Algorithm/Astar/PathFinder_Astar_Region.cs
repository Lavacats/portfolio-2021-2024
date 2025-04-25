using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pos
{
    public int X;
    public int Y;

    public Pos() { }
    public Pos(int _x, int _y)
    {
        X = _x;
        Y = _y;
    }
}
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


    // 맵 전체의 크기
    public int width;
    public int height;


    // 타일들에 대한 Grid 정보 배열
    private bool[,] GridTile;


    private List<Pos> AStarPathFind(Pos currentPosition, Pos destPosition)
    {
        /// 목적지까지 길을 찾기 위한 점수 계산
        /// F = G + H
        /// F = 최종 점수 ( 작을수록 좋으며 경로에 따라 달라진다 
        /// G = 시작점에서 목적 좌표까지 이동하는데 드는 비용
        /// H = 휴리스틱,  목적지에 얼마나 가까운지에 대한 가중치

        List<Pos> paths = new List<Pos>();

        /// 방문 여부 검사를 위한 배열과 지나온 길을 갖는 parent 배열
        bool[,] closed = new bool[width, height];
        Pos[,] parent = new Pos[width, height];

        /// (x,y) 로 가는 길을 발견한적 있는가?
        /// 발견하지 못한 경우 MaxValue를 갖는다
        /// 발견한 경우 F = G + H
        int[,] open = new int[width, height];

        // 초기화
        for(int x=0;x<width;x++)
        {
            for(int y=0;y<height;y++)
            {
                open[y, x] = int.MaxValue;
            }
        }

        /// Open 배열에서 가장 좋은 노드를 가져오기위한 큐
        PriorityQueue<PQNode> pqQueue = new PriorityQueue<PQNode>();

        /// F = G + H
        /// 자기 자신을 이동시키므로 G는 0으로 계산한다
        /// 10은 기본 cost
        open[currentPosition.X, currentPosition.Y] = (int)(10 * (MathF.Abs(destPosition.Y - currentPosition.Y) + MathF.Abs(destPosition.X - currentPosition.X)));

        pqQueue.Push(
            new PQNode()
            {
                F = open[currentPosition.X, currentPosition.Y],
                G = 0,
                X = currentPosition.X,
                Y = currentPosition.Y,
            }
        );

        parent[currentPosition.X, currentPosition.Y] = new Pos(currentPosition.X, currentPosition.Y);

        while (pqQueue.Count > 0)
        {
            PQNode node = pqQueue.Pop();

            /// 동일한 좌표를 여러 경로를 찾아서 더 따른 경로로 인해 이미 방문체크가 완료된 경우
            if (closed[node.X, node.Y])
            {
                continue;
            }
            closed[node.X, node.Y] = true;

            if (node.X == destPosition.X && node.Y == destPosition.Y)
                break;

            /// 상하좌우로 이동할 수 있는 좌표를 예약
            for (int deltaX = -1; deltaX < 2; deltaX++)
            {
                for (int deltaY = -1; deltaY < 2; deltaY++)
                {
                    int cost = 10;

                    /// 자기 자신의 위치 제외
                    if (deltaX == 0 && deltaY == 0)
                        continue;

                    /// 대각선인 경우 코스트를 좀 더 높인다
                    if ((Mathf.Abs(deltaX) == Mathf.Abs(deltaY)))
                    {
                        cost = 14;
                    }

                    int nextX = node.X + deltaX;
                    int nextY = node.Y = deltaY;

                    // 탐색할 위치가 맵의 크기보다 크거나 작은 경우는 스킵
                    if (nextX >= GridTile.GetLength(0) || nextX < 0 || nextY >= GridTile.GetLength(0) || nextY < 0)
                        continue;

                    // 탐색할 위치가 벽인 경우 스킵
                    if (!GridTile[nextX, nextY])
                        continue;

                    // 이미 방문했던 노드라면 스킵
                    if (closed[nextX, nextY])
                        continue;

                    // 비용 계산
                    Debug.Log(cost);
                    int g = node.G + cost;
                    int h = 10 * (int)(MathF.Abs(destPosition.X - nextX) + Mathf.Abs(destPosition.Y - nextY));

                    // 더 빠른 다른 경로를 찾았는가
                    if (open[nextX, nextY] < (g + h))
                        continue;

                    open[nextY, nextX] = (g + h);
                    pqQueue.Push(new PQNode()
                    {
                        F = (g + h),
                        G = g,
                        X = nextX,
                        Y = nextY,
                    }
                    );
                    parent[nextX, nextY] = new Pos(node.X, node.Y);
                }
            }
        }
        int destX = destPosition.X;
        int destY = destPosition.Y;


        /// 찾을 수 있는 길이 있는가?
        while (parent[destX, destY] != null)
        {
            /// 목적지로부터 시작지점까지 거슬러 올라가 최종적으로 parentPos[destX,destY]  == currentPostion이 된다.
            if (parent[destX,destY].X != destX || parent[destX,destY].Y!=destY)
            {
                paths.Add(new Pos(destX,destY));
                Pos pos = parent[destX,destY];
                destX = pos.X;
                destY = pos.Y;
            }
            else
            {
                break;
            }
        }
        paths.Add(new Pos(destX,destY));
        paths.Reverse();

        return paths;
    }
}

