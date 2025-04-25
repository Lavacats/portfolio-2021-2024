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
        /// 휴리스틱 거리 측정값 Heuristic cost
        /// Heuristic cost function : f(n) = g(n) + h(n)
        /// F : 현재 노드 N 까지의 최단 비용 g(n)과 목표 노드까지의 예상 이동 비용 h(n) 을 더한 총 비용이다.
        /// </summary>
  
        public int G;
        /// <summary>
        /// 유클리디안 거리 ( Euclidean distance ) 
        /// 출발 노드에서 현재 노드 n까지 도달하기 위한 최단 비용
        /// </summary>
        
        public int H;
        /// <summary>
        /// 맨하탄 거리 ( Manhattan distance )
        /// 
        /// 현재 노드 n에서 목표 노드까지의 예상 이동 비용으로 , Heuristic 휴리스틱 거리 측정값이라고 한다.
        /// </summary>

        public Vector2Int Pos;

        //public Battle_MapPixel MapPixel;
        /// <summary>
        /// MapPixel 에 있는 
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
        /// 목적지까지 길을 찾기 위한 점수 계산
        /// F = G + H
        /// F = 최종 점수 ( 작을수록 좋으며 경로에 따라 달라진다 
        /// G = 시작점에서 목적 좌표까지 이동하는데 드는 비용
        /// H = 휴리스틱,  목적지에 얼마나 가까운지에 대한 가중치
        
        List<Vector2Int> paths = new List<Vector2Int>();

        /// 방문 여부 검사를 위한 배열과 지나온 길을 갖는 parent 배열
        /// 배열이 dictionary보다 부하가 더 적고 효율적이다.
        int[,] costCountArray = new int[Map_Width, Map_Height];             // open
        bool[,] closed = new bool[Map_Width, Map_Height];
        PQNode[,] pathParent = new PQNode[Map_Width, Map_Height];

        /// (x,y) 로 가는 길을 발견한적 있는가?
        /// 발견하지 못한 경우 MaxValue를 갖는다
        /// 발견한 경우 F = G + H
        for (int x = 0; x < Map_Width; x++)
        {
            for (int y = 0; y < Map_Height; y++)
            {
                costCountArray[y, x] = int.MaxValue;
            }
        }

        /// Open 배열에서 가장 좋은 노드를 가져오기위한 큐
        PriorityQueue<PQNode> pqQueue = new PriorityQueue<PQNode>();

        /// H = 멘하탄 거리 
        /// 2차원 좌표에서 시작점과 도착점의 최소거리
        int H_value = (int)(10 * (MathF.Abs(arrivePosition.y - currentPosition.y) + MathF.Abs(arrivePosition.x- currentPosition.x)));


        /// 우선순위 큐에 현재 노드 정보 저장
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
            PQNode node = pqQueue.Pop(); // 큐에서 하나 꺼냄


            /// 동일한 좌표를 여러 경로를 찾아서 더 따른 경로로 인해 이미 방문체크가 완료된 경우
            //if (closed[node.x, node.y])
            //{
            //    continue;
            //}


        }




        return paths;
    }

}
