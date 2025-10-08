using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public GridGraph gridGraph;
    public PathAgent pathAgent;
    public KeyCode clickKey = KeyCode.Mouse0;

    bool isMoving = false;


    void Reset()
    {
        pathAgent = GetComponent<PathAgent>();
    }


    private void Update()
    {
        
        


        if (Input.GetKeyDown(clickKey) && Camera.main != null)
        {
            var mouseID = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseID.z = 0f; //2D이니까 


            Vector2Int goal = gridGraph.WorldToCell(mouseID);
            if(!gridGraph.InBounds(goal)) return;


            Vector2Int start = gridGraph.WorldToCell(transform.position);

            Debug.Log($"start:{start} , goal:{goal}");
            if (!isMoving)
            {
                StartCoroutine(MoveRoutine(start, goal));
            }


        }

    }


    IEnumerator MoveRoutine(Vector2Int start, Vector2Int goal)
    {
        isMoving = true;
        yield return pathAgent.Go(start, goal);
        isMoving = false;
        
    }
}
