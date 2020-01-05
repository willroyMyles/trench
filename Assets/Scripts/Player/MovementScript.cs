using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MovementScript : MonoBehaviour
{
    internal float pushbackDistance = 2;
    internal bool isPushedBack = false;
    internal bool isPlayerControlled = false;
    internal bool ableToMove = true;
    internal bool isFighting = false;
    internal List<GameObject> opponentsList;
    internal NavMeshAgent agent;

    private void Awake()
    {
        SetUp();
    }

    private void SetUp()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.angularSpeed = 700;
        agent.acceleration = 80;
    }

    private void Update()
    {

        if (isPlayerControlled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var mask = LayerMask.GetMask("platform");
                if (Physics.Raycast(ray, out var hit, 100, mask))
                {
                    agent.SetDestination(hit.point);
                }
            }

            if (isPushedBack)
            {
                agent.Move(-transform.forward * pushbackDistance);
                //player.SetDestination(transform.position - transform.forward * pushBackDistance);
                isPushedBack = false;
            }
        }
        else
        {
            if (ableToMove)
            {
                agent.transform.position = agent.nextPosition;
                //if (agent.transform.position == agent.nextPosition && agent.isStopped) agent.isStopped = false;
                //patrol
                if (!agent.pathPending && agent.remainingDistance < 1f)
                {
                    //agent.SetDestination(GetRandompointOnPlane());
                }
            }

        }

    }

    internal void PushPlayerBack(float bb)
    {
        pushbackDistance = bb;
        isPushedBack = true;
    }

    public void setPlayerControlled(bool playerControl)
    {
        if (!agent) SetUp();
        isPlayerControlled = playerControl;
        agent.updatePosition = playerControl;
    }

}
