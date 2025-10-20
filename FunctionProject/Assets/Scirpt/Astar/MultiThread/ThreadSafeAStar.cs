using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class ThreadSafeAStar : MonoBehaviour
{
    public struct I2
    {
        public int x, y;
        public I2(int x, int y)
        {
            this.x = x; this.y = y;
        }

    }



    public static List<I2> FindPath(bool[,] walkable, int w, int h, I2 start, I2 goal, bool allowDiagonal = false)
    {
        if (!In(start, w, h) || !In(goal, w, h)) return null;
        if (!walkable[start.x, start.y] || !walkable[goal.x, goal.y]) return null;
        if (start.x == goal.x && start.y == goal.y) return new List<I2> { start };

        const int INF = int.MaxValue;
        var g = new int[w, h];
        var hcost = new int[w, h];
        var px = new int[w, h];
        var py = new int[w, h];
        var inOpen = new bool[w, h];
        var inClosed = new bool[w, h];



        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                g[x, y] = INF;
                px[x, y] = -1;
                py[x, y] = -1;
            }
        }

        int Heu(I2 a, I2 b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); //멘해튼 휴리스틱

        var open = new List<I2> { start };
        g[start.x, start.y] = 0;//시작지점이니까
        hcost[start.x, start.y] = Heu(start, goal); //도달지점 
        inOpen[start.x, start.y] = true;


        var dir4 = new I2[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
        var diag4 = new I2[] { new(1, 1), new(-1, 1), new(1, -1), new(-1, -1) };

        while (open.Count > 0)
        {

            int bi = 0, bestF = int.MaxValue, bestH = int.MaxValue;
            for (int i = 0; i < open.Count; i++)
            {
                var n = open[i];
                int f = g[n.x, n.y] + hcost[n.x, n.y];
                if (f < bestF || (f == bestF && hcost[n.x, n.y] < bestH))
                {
                    bi = i;
                    bestF = f;
                    bestH = hcost[n.x, n.y];

                }
            }

            var cur = open[bi];
            open.RemoveAt(bi);
            inOpen[cur.x, cur.y] = false;
            inClosed[cur.x, cur.y] = true;

            if (cur.x == goal.x && cur.y == goal.y)
            {
                return Reconstruct(goal, px, py);
            }

            IEnumerable<I2> Neigh()
            {
                foreach (var d in diag4)
                {
                    var np = new I2(cur.x + d.x, cur.y + d.y);
                    if (In(np, w, h) && walkable[np.x, np.y])
                    {
                        yield return np;
                    }
                }
                if (allowDiagonal)
                {
                    foreach (var d in diag4)
                    {
                        var np = new I2(cur.x + d.x, cur.y + d.y);
                        if (!In(np, w, h) || !walkable[np.x, np.y])
                        {
                            continue;
                        }

                        var a = new I2(cur.x, np.y);
                        var b = new I2(np.x, cur.y);
                        if (!walkable[a.x, a.y] || !walkable[b.x, b.y])
                        {
                            continue;
                        }

                        yield return np;

                    }
                }
            }

            foreach (var nb in Neigh())
            {
                if (inClosed[nb.x, nb.y]) continue;
                int step = (nb.x != cur.x && nb.y != cur.y) ? 14 : 10;
                int tentative = g[cur.x, cur.y] + step;

                if (tentative < g[nb.x, nb.y])
                {
                    g[nb.x, nb.y] = tentative;
                    hcost[nb.x, nb.y] = Heu(nb, goal);
                    px[nb.x, nb.y] = cur.x;
                    py[nb.x, nb.y] = cur.y;

                    if (!inOpen[nb.x, nb.y])
                    {
                        open.Add(nb);
                        inOpen[nb.x, nb.y] = true;
                    }
                }
            }




        }
        return null;


    }
    static bool In(I2 p, int w, int h) => p.x >= 0 && p.y >= 0 && p.x < w && p.y < h;

    static List<I2> Reconstruct(I2 goal, int[,] px, int[,] py)
    {
        var path = new List<I2>();
        int x = goal.x, y = goal.y;
        while (true)
        {
            path.Add(new I2(x, y));
            int nx = px[x, y], ny = py[x, y];
            if (nx == 0 && ny == 0 && path.Count > 1) break;
            if (nx == 0 && ny == 0 && path.Count == 1) break;
            x = nx; y = ny;
        }
        path.Reverse();
        return path;
    }






}
