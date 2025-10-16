using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class GridGraph : MonoBehaviour
{
    [Header("Grid")]
    public int width;
    public int height;
    public float cellsize = 1f;

    [Header("Collision")]
    public LayerMask obstacleMask; //장애물
    public Grid[,] nodes; //2차원 배열로 그리드 생성

    private void Awake()
    {
        Build();
        
    }

    public void Build()
    {
        nodes = new Grid[width, height];
        Vector2  origin = (Vector2)transform.position; //맵만들기

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 world = origin + new Vector2(x + 0.5f, y + 0.5f) * cellsize;
                bool block = Physics2D.OverlapBox(world, Vector2.one * (cellsize * 0.9f), 0, obstacleMask);
                nodes[x,y] = new Grid (new Vector2Int(x,y), !block); //장애물이 있으면 false, 없으면 true

            }
        
        }

    }

    public bool InBounds(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;

    public Grid Get(Vector2Int p) => nodes[p.x, p.y];



    public IEnumerable<Grid> GetNeighbor(Grid g, bool allowDiagonal = false)
    {
        var dir4 = new Vector2Int[]
            {
                new Vector2Int(1,0),new Vector2Int(-1,0)
                ,new Vector2Int(0,1),new Vector2Int(0,-1)
            };

        foreach (var d in dir4)
        {
            var np =  g.pos + d;
            if (InBounds(np) && Get(np).walkable)
            {
                yield return Get(np);
            }


        }
    }


    private void OnDrawGizmos()
    {
        if (nodes == null)
        {
            return;
        }

        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one* cellsize);

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            var n = nodes[x, y];
            Gizmos.color = n.walkable ? Color.white : Color.red;
            Gizmos.DrawWireCube(new Vector3(x + 0.5f, y + 0.5f, 0), Vector3.one * 0.95f);
        }
    }


    public Vector3 CellToWorldCenter(Vector2Int cell)
    {
        var origin = (Vector2)transform.position;

        return new Vector3(origin.x + (cell.x + 0.5f) * cellsize, origin.y + (cell.y + 0.5f) * cellsize, 0f);


    }

    public Vector2Int WorldToCell(Vector3 world)
    {
        var origin = (Vector2)transform.position;
        var local = (Vector2)world - origin;

        int x = Mathf.FloorToInt(local.x / cellsize);
        int y = Mathf.FloorToInt(local.y / cellsize);


        return new Vector2Int(x, y);
    }






}
