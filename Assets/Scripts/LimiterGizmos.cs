using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimiterGizmos : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private Transform groundlevel;

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(start.position, new Vector2(start.position.x, start.position.y +1000));
        Gizmos.DrawLine(start.position, new Vector2(start.position.x, start.position.y -1000));

        Gizmos.DrawLine(end.position, new Vector2(end.position.x, end.position.y +1000));
        Gizmos.DrawLine(end.position, new Vector2(end.position.x, end.position.y -1000));

        Gizmos.DrawLine(groundlevel.position, new Vector2(groundlevel.position.x +1000, groundlevel.position.y));
        Gizmos.DrawLine(groundlevel.position, new Vector2(groundlevel.position.x -1000, groundlevel.position.y));
    }

}
