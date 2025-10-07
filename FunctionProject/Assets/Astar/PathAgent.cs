using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathAgent : MonoBehaviour
{
    public AStarPathFinder pathFinder;
    public float moveSpeed = 3.0f;

    public IEnumerator Go(Vector2Int start, Vector2Int goal)
    {
        var path = pathFinder.FindPath(start, goal);

        if (path ==null)
        {
            yield break;    
        }


        float s = pathFinder.gridGraph.cellsize;

        foreach (var p in path)
        {
            Vector3 target = (Vector3)pathFinder.gridGraph.transform.position + new Vector3((p.x + 0.5f) * s, (p.y + 0.5f) * s, 0f);
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                //움직이기.
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
            
        }


    }
}
