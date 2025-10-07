using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{
    public Vector2Int pos;

    public bool walkable;

    public int gCost; //시작-> 현재
    public int hCost; //현재-> 목표
    public int fCost => gCost + hCost;


    public Grid parent;

    public Grid(Vector2Int p, bool w)
    {
        pos = p;
        walkable = w;
        gCost = hCost = int.MaxValue; //최단경로를 찾을꺼니까 가장 큰값으로 대입
        parent = null;

    }
}
