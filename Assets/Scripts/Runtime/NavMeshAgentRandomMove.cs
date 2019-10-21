using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentRandomMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    public float wanderRange = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        if (!agent)
            enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
        {
            Random.InitState((int)Time.time + gameObject.GetHashCode());
            var target = Random.insideUnitSphere * wanderRange;
            target.y = 0;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(target, out hit, 1f, NavMesh.AllAreas))
                agent.destination = hit.position;
        }
        anim.SetFloat("speed", agent.velocity.magnitude);
    }
}
