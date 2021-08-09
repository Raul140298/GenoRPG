using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPathMovementScript : MonoBehaviour
{
    public Vector2[] localWaypoints;
    public Vector2 velocity;
    public float speed;
    public bool cyclic, inBattle = false, isntMove;
    Vector2[] globalWaypoints;
    int fromWaypointIndex;
    float percentBetweenWaypoints;

    // Start is called before the first frame update
    void Start()
    {
        globalWaypoints = new Vector2[localWaypoints.Length];
        Vector2 v2 = transform.position;
        for(int i=0; i < localWaypoints.Length; i++)
		{
            globalWaypoints[i] = localWaypoints[i] + v2;
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (inBattle == false && isntMove == false)
		{
            velocity = CalculatePlatformMovement();
            transform.Translate(velocity);
        }        
    }

    Vector2 CalculatePlatformMovement()
	{
        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector2.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;

        Vector2 newPos = Vector2.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], percentBetweenWaypoints);
        
        if(percentBetweenWaypoints >= 1)
		{
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if(!cyclic)
			{
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
		}
        Vector2 v2 = transform.position;
        return newPos - v2;
    }

	private void OnDrawGizmos()
	{
		if(localWaypoints.Length != 0)
		{
            Gizmos.color = Color.red;
            float size = 0.05f;

            for(int i=0; i < localWaypoints.Length; i++)
			{
                Vector2 v2 = transform.position;
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + v2;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
}
