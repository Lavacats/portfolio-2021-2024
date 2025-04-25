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


    // �� ��ü�� ũ��
    public int width;
    public int height;


    // Ÿ�ϵ鿡 ���� Grid ���� �迭
    private bool[,] GridTile;


    private List<Pos> AStarPathFind(Pos currentPosition, Pos destPosition)
    {
        /// ���������� ���� ã�� ���� ���� ���
        /// F = G + H
        /// F = ���� ���� ( �������� ������ ��ο� ���� �޶����� 
        /// G = ���������� ���� ��ǥ���� �̵��ϴµ� ��� ���
        /// H = �޸���ƽ,  �������� �󸶳� ��������� ���� ����ġ

        List<Pos> paths = new List<Pos>();

        /// �湮 ���� �˻縦 ���� �迭�� ������ ���� ���� parent �迭
        bool[,] closed = new bool[width, height];
        Pos[,] parent = new Pos[width, height];

        /// (x,y) �� ���� ���� �߰����� �ִ°�?
        /// �߰����� ���� ��� MaxValue�� ���´�
        /// �߰��� ��� F = G + H
        int[,] open = new int[width, height];

        // �ʱ�ȭ
        for(int x=0;x<width;x++)
        {
            for(int y=0;y<height;y++)
            {
                open[y, x] = int.MaxValue;
            }
        }

        /// Open �迭���� ���� ���� ��带 ������������ ť
        PriorityQueue<PQNode> pqQueue = new PriorityQueue<PQNode>();

        /// F = G + H
        /// �ڱ� �ڽ��� �̵���Ű�Ƿ� G�� 0���� ����Ѵ�
        /// 10�� �⺻ cost
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

            /// ������ ��ǥ�� ���� ��θ� ã�Ƽ� �� ���� ��η� ���� �̹� �湮üũ�� �Ϸ�� ���
            if (closed[node.X, node.Y])
            {
                continue;
            }
            closed[node.X, node.Y] = true;

            if (node.X == destPosition.X && node.Y == destPosition.Y)
                break;

            /// �����¿�� �̵��� �� �ִ� ��ǥ�� ����
            for (int deltaX = -1; deltaX < 2; deltaX++)
            {
                for (int deltaY = -1; deltaY < 2; deltaY++)
                {
                    int cost = 10;

                    /// �ڱ� �ڽ��� ��ġ ����
                    if (deltaX == 0 && deltaY == 0)
                        continue;

                    /// �밢���� ��� �ڽ�Ʈ�� �� �� ���δ�
                    if ((Mathf.Abs(deltaX) == Mathf.Abs(deltaY)))
                    {
                        cost = 14;
                    }

                    int nextX = node.X + deltaX;
                    int nextY = node.Y = deltaY;

                    // Ž���� ��ġ�� ���� ũ�⺸�� ũ�ų� ���� ���� ��ŵ
                    if (nextX >= GridTile.GetLength(0) || nextX < 0 || nextY >= GridTile.GetLength(0) || nextY < 0)
                        continue;

                    // Ž���� ��ġ�� ���� ��� ��ŵ
                    if (!GridTile[nextX, nextY])
                        continue;

                    // �̹� �湮�ߴ� ����� ��ŵ
                    if (closed[nextX, nextY])
                        continue;

                    // ��� ���
                    Debug.Log(cost);
                    int g = node.G + cost;
                    int h = 10 * (int)(MathF.Abs(destPosition.X - nextX) + Mathf.Abs(destPosition.Y - nextY));

                    // �� ���� �ٸ� ��θ� ã�Ҵ°�
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


        /// ã�� �� �ִ� ���� �ִ°�?
        while (parent[destX, destY] != null)
        {
            /// �������κ��� ������������ �Ž��� �ö� ���������� parentPos[destX,destY]  == currentPostion�� �ȴ�.
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

