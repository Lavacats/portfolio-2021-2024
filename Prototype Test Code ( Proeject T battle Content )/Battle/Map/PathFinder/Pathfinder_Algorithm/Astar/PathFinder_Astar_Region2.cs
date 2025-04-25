using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder_Astar_Region2 
{
    struct PQNode : IComparable<PQNode>
    {
        public int F;
        /// <summary>
        /// �޸���ƽ �Ÿ� ������ Heuristic cost
        /// Heuristic cost function : f(n) = g(n) + h(n)
        /// F : ���� ��� N ������ �ִ� ��� g(n)�� ��ǥ �������� ���� �̵� ��� h(n) �� ���� �� ����̴�.
        /// </summary>
  
        public int G;
        /// <summary>
        /// ��Ŭ����� �Ÿ� ( Euclidean distance ) 
        /// ��� ��忡�� ���� ��� n���� �����ϱ� ���� �ִ� ���
        /// </summary>
        
        public int H;
        /// <summary>
        /// ����ź �Ÿ� ( Manhattan distance )
        /// 
        /// ���� ��� n���� ��ǥ �������� ���� �̵� ������� , Heuristic �޸���ƽ �Ÿ� �������̶�� �Ѵ�.
        /// </summary>

        public Vector2Int Pos;

        //public Battle_MapPixel MapPixel;
        /// <summary>
        /// MapPixel �� �ִ� 
        /// </summary>

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
    private Dictionary<Vector2Int, PQNode> MapData =  new Dictionary<Vector2Int, PQNode> ();

    private int Map_Width;
    private int Map_Height;

    public void AddMapData(Battle_MapPixel mapPixel)
    {
        if (MapData.ContainsKey(mapPixel.PixelIndex)==false)
        {
            PQNode node = new PQNode()
            {
                F = 0,
                G = 0,
                H = 0,
                Pos = mapPixel.PixelIndex,
            };
            MapData[mapPixel.PixelIndex] = node;
        }
    }

    private List<Vector2Int> AStarPathFind(Vector2Int currentPosition, Vector2Int arrivePosition)
    {
        /// Step 0:
        /// ���������� ���� ã�� ���� ���� ���
        /// F = G + H
        /// F = ���� ���� ( �������� ������ ��ο� ���� �޶����� 
        /// G = ���������� ���� ��ǥ���� �̵��ϴµ� ��� ���
        /// H = �޸���ƽ,  �������� �󸶳� ��������� ���� ����ġ
        
        List<Vector2Int> paths = new List<Vector2Int>();

        /// �湮 ���� �˻縦 ���� �迭�� ������ ���� ���� parent �迭
        /// �迭�� dictionary���� ���ϰ� �� ���� ȿ�����̴�.
        int[,] costCountArray = new int[Map_Width, Map_Height];             // open
        bool[,] closed = new bool[Map_Width, Map_Height];
        PQNode[,] pathParent = new PQNode[Map_Width, Map_Height];

        /// (x,y) �� ���� ���� �߰����� �ִ°�?
        /// �߰����� ���� ��� MaxValue�� ���´�
        /// �߰��� ��� F = G + H
        for (int x = 0; x < Map_Width; x++)
        {
            for (int y = 0; y < Map_Height; y++)
            {
                costCountArray[y, x] = int.MaxValue;
            }
        }

        /// Open �迭���� ���� ���� ��带 ������������ ť
        PriorityQueue<PQNode> pqQueue = new PriorityQueue<PQNode>();

        /// H = ����ź �Ÿ� 
        /// 2���� ��ǥ���� �������� �������� �ּҰŸ�
        int H_value = (int)(10 * (MathF.Abs(arrivePosition.y - currentPosition.y) + MathF.Abs(arrivePosition.x- currentPosition.x)));


        /// �켱���� ť�� ���� ��� ���� ����
        pqQueue.Push(
             new PQNode()
             {
                 F = costCountArray[currentPosition.x, currentPosition.y],
                 G = 0,
                 H = H_value,
                 //MapPixel.PixelIndex.X= currentPosition.x,
                 //Y = currentPosition.Y,
             }
        );


        pathParent[currentPosition.x, currentPosition.y] = new PQNode() { };

        while (pqQueue.Count > 0)
        {
            PQNode node = pqQueue.Pop(); // ť���� �ϳ� ����


            /// ������ ��ǥ�� ���� ��θ� ã�Ƽ� �� ���� ��η� ���� �̹� �湮üũ�� �Ϸ�� ���
            //if (closed[node.x, node.y])
            //{
            //    continue;
            //}


        }




        return paths;
    }

}
