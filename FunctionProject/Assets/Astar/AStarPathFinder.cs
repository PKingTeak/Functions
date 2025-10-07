using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AStarPathFinder : MonoBehaviour
{
    public GridGraph gridGraph;

    int Heuristic(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    Grid PopLowestF(List<Grid> list)
    {
        int best = 0;

        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].fCost < list[best].fCost || (list[i].fCost == list[best].fCost && list[i].hCost < list[best].hCost))
            {
                best = i;
            }
        }

        var node = list[best];
        list.RemoveAt(best);
        return node;
    }



    public List<Vector2Int> FindPath(Vector2Int start,Vector2Int goal , bool allowDiagonal = false)
    {
        foreach (var n in gridGraph.nodes)
        {
            n.gCost = int.MaxValue;
            n.hCost = 0;
            n.parent = null;
        }

        var open = new List<Grid>();
        var closed = new HashSet<Grid>();


        if (!gridGraph.InBounds(start) || !gridGraph.Get(start).walkable)
        {
            return null;
        }

        var startNode = gridGraph.Get(start);
        var goalNode = gridGraph.Get(goal);

        if (startNode == goalNode) return null;

        startNode.gCost = 0;
        startNode.hCost = Heuristic(start, goal);
        open.Add(startNode);



        while (open.Count > 0)
        { 
            var cur = PopLowestF(open);
            if (cur == goalNode)
            {
                return Reconstruct(goalNode); //경로 재구성
               
            }

            closed.Add(cur);

            foreach (var nb in gridGraph.GetNeighbor(cur, allowDiagonal)) //인접노드 검색
            {
                if (closed.Contains(nb)) continue;

                int tentative = cur.gCost + 10;

                if (tentative < nb.gCost)
                {
                    nb.parent = cur;
                    nb.gCost = tentative;
                    nb.hCost = Heuristic(nb.pos, goal);


                    if (!open.Contains(nb))
                    {
                        open.Add(nb);
                    }
                }
            }




        }

        return null;




    }



    List<Vector2Int> Reconstruct(Grid goal) //경로를 되짚어 리스트로 반환
    {
        var list = new List<Vector2Int>();
        var cur = goal;
        while (cur != null)
        {
            list.Add(cur.pos);
            cur = cur.parent;

        }
        list.Reverse();
        return list;
    }

    [ContextMenu("Test FindPath")]
    void TestFind()
    {
        var path = FindPath(new Vector2Int(0, 0), new Vector2Int(gridGraph.width - 1, gridGraph.height - 1));
        if (path == null) Debug.Log("No Path");
        else Debug.Log("Path len: " + path.Count);
    }
}



/*
1. open에 시작노드 넣기
2. open에서 fCost 제일 낮은 노드 꺼내기
3. 목표 도착 시 종료
4. 이웃 탐색하면서 gCost 계산
5. 더 짧은 경로면 parent 갱신
6. open에 추가
 * 
 */