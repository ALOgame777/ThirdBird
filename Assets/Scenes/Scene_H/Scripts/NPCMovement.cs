using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    public float wanderRadius = 10f; // NPC�� ���ƴٴ� �ݰ�
    public float wanderTimer = 2f; // ���� ��ġ�� �̵��ϴ� �ֱ�

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        SetNewDestination();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            SetNewDestination();
            timer = 0f;
        }
    }

    private void SetNewDestination()
    {
        // ���� ��ġ ����
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        // ������ ��ġ ����
        Vector3 randDirection = Random.insideUnitSphere * distance;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, distance, layermask);
        return navHit.position;
    }
}
