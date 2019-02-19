using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentRandomMove : MonoBehaviour
{
    private NavMeshAgent agent;
    public float wanderRange = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent)
            enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.hasPath)
        {
            var target = Random.insideUnitCircle * wanderRange;
            NavMeshHit hit;
            NavMesh.FindClosestEdge(new Vector3(target.x, 0f, target.y), out hit, NavMesh.AllAreas);
            agent.destination = hit.position;
        }
    }
}
