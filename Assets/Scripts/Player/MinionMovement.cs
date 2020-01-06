using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MinionMovement : MonoBehaviour
{
    internal bool isPushedBack = false;
    internal bool isPlayerControlled = false;
    internal bool ableToMove = false;
    bool hesitatntMoved = true;
    internal NavMeshAgent agent;
    MinionType type;

    private void Awake()
    {
        SetUp();
        SetType(null);
    }

    private void SetUp()
    {
        agent = GetComponent<NavMeshAgent>();
        
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
                //agent.Move(-transform.forward * pushbackDistance);
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
                if (!agent.pathPending && agent.remainingDistance < .01f)
                {
                    agent.SetDestination(GetNextPositionBasedOffType());
                }
            }

        }

    }

    private Vector3 GetNextPositionBasedOffType()
    {
        Vector3 pos = transform.position;

        if(type == MinionType.Crossy) // should cross 10 times so increase by 1
        {
            pos.z -= 1;
            if (pos.x > 0) pos.x = Mathf.Abs(UnityEngine.Random.Range(0, GV.Singleton().playgroundSize.x)) * -1;
            else pos.x = Mathf.Abs(UnityEngine.Random.Range(0, GV.Singleton().playgroundSize.x));
            return pos;
        }
        if(type == MinionType.Hesitant) // should move down and up slowly increase its forward distance each time.  move forward two, move back one with random x
        {
            if (hesitatntMoved) pos.z -= 2.7f;
            else pos.z += 2.2f;
            pos.x = UnityEngine.Random.Range(-GV.Singleton().playgroundSize.x / 2, GV.Singleton().playgroundSize.x/2);
            hesitatntMoved = !hesitatntMoved;
            return pos;
           // consider randomizing x
        }
        if(type == MinionType.Straighty) // moves forward in a straight line  at reduced speed?
        {

        }

        if (type == MinionType.Random) { }


        return GV.Singleton().getRandompointOnPlane();
    }

    internal void PushPlayerBack(float bb)
    {
        //pushbackDistance = bb;
        isPushedBack = true;
    }

    public void setPlayerControlled(bool playerControl)
    {
        if (!agent) SetUp();
        isPlayerControlled = playerControl;
        agent.updatePosition = playerControl;
        ableToMove = !playerControl;
        gameObject.tag = playerControl ? "playerMinion" : "enemyMinion"; 
    }


    /// <summary>
    /// sets type specified. 
    /// If no type specified, it generates one at random.
    /// </summary>
    /// <param name="type"></param>
    private void SetType(MinionType? type)
    {
        if (type.HasValue) this.type = type.Value;
        else
        {
            this.type = (MinionType)UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(MinionType)).Length);
        }
    }
}
