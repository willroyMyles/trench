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

    SpawnController neuralParent;
    float distance;
    float maxDistance = 10;
    NeuralNetwork brain;
    public float fitness;
    bool bulletIncoming = true;
    bool beenHit = true;
    private void Awake()
    {
        SetUp();
        SetType(null);
    }

    private void SetUp()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("artillery"))
        {
            bulletIncoming = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("artillery"))
        {
            bulletIncoming = false;
        }
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
                if (!agent.pathPending && agent.remainingDistance < .5f)
                {
                    distance = Vector3.Distance(transform.position, GV.Singleton().playerGoal.transform.position);
                    float[] inputs = new float[5] { transform.position.x, transform.position.z, distance,Convert.ToInt32( beenHit), Convert.ToInt32( bulletIncoming )};
                    float[] outputs = brain.FeedForward(inputs);
                    var destination = new Vector3(outputs[0] * 1.6f, 0, outputs[1] * 4.4f);
                    agent.SetDestination(destination);
                    if (Vector3.Distance(transform.position, destination) <= .3f) brain.AddFitness(distance - maxDistance);
                    else brain.AddFitness(maxDistance - distance);
                    fitness = brain.GetFitness();
                    if(beenHit) beenHit = false;
                }
            }

        }

    }

    public void SetBrain(NeuralNetwork newBrain, SpawnController controller)
    {
        brain = newBrain;
        neuralParent = controller;
    }

    private Vector3 GetNextPositionBasedOffType()
    {
        Vector3 pos = transform.position;

        if(type == MinionType.Crossy) // should cross 10 times so increase by 1
        {
            pos.z += 1;
            pos.x *=  -1;
            return pos;
        }
        if(type == MinionType.Hesitant) // should move down and up slowly increase its forward distance each time.  move forward two, move back one
        {
            if (hesitatntMoved) pos.z -= 1;
            else pos.z += .5f;
            hesitatntMoved = !hesitatntMoved;
            return pos;
           // consider randomizing x
        }
        if(type == MinionType.Straighty) // moves forward in a straight line  
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

    public void MinusFromFitness()
    {
        brain.AddFitness(-100);
        beenHit = true;
    }

    internal void DieCalled()
    {
       neuralParent.UpdateNeuralNetworkList(brain);
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
