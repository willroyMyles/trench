using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System;

public class Minion : MonoBehaviour
{
    MinionBase stats = new MinionBase();
    NavMeshAgent agent;
    Renderer renderer;
    FracturedWhole fracturedWhole;
    float damage = 0;
    float percentageOfDamage;

    private void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
        if (transform.GetChild(0).TryGetComponent<FracturedWhole>(out var whole)) fracturedWhole = whole;
        GV.Singleton().gameEnded += DisableScript;

        SetStats();
    }

    public void DisableScript()
    {
       // enabled = false;
    }

    private void SetAgentStats() 
    { 
        if(!agent)      
            agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.speed;
        agent.angularSpeed = stats.anugularSpeed;
        agent.acceleration = stats.acceleration;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("artillery"))
        {
            


            ApplyPushBack(collision.relativeVelocity * .015f);
            ApplyDamage(collision.gameObject.GetComponent<ArtilleryBase>().GetDamage());
            Destroy(collision.gameObject);
        }
    }

    public void ApplyPushBack(Vector3 dir)
    {
        //blow back?
        agent.Move(dir * .3f);
    }

    private void ApplyDamage(float damage)
    {
        // affect health
        this.damage += damage;
        percentageOfDamage = (stats.health - this.damage) / stats.health;
        StartCoroutine( ChangeColor(stats.healthPoints));
        SlowDownSpeedByDamagePercent();
        stats.healthPoints -= damage;
        if (percentageOfDamage <= .5f && fracturedWhole) fracturedWhole.chipOffPiece(true);

       
        
        if (stats.healthPoints <= 0) Die();
    }

    private void SlowDownSpeedByDamagePercent()
    {
        stats.speed -= stats.speed * percentageOfDamage * .3f;
        agent.speed = stats.speed;
    }

    /// <summary>
    /// Changes the color from assigned color to black to reflect health.
    ///  Also stuns minion.
    /// </summary>
    private IEnumerator ChangeColor(float healthbeforeDamage)
    {
        //consider using the coroutine just for a timer


        if (!agent) yield break;
        if (!renderer) yield break;
        if (!gameObject) yield break;

         
        var r = renderer.material.GetColor("_BaseColor").r * percentageOfDamage;
        var g = renderer.material.GetColor("_BaseColor").g * percentageOfDamage;
        var b = renderer.material.GetColor("_BaseColor").b * percentageOfDamage;

        var col = new Color(r, g, b);
        agent.isStopped = true;
        setColor(Color.grey);
        yield return new WaitForSecondsRealtime(.05f);
        if (!agent) yield break;
        setColor(col);
        agent.isStopped = false;


        //yield return null;

        //while (true)
        //{
        //    renderer.material.SetColor("_BaseColor", Color.Lerp(renderer.material.color, col, t));
        //    if (t <= healthbeforeDamage - damage) break;
        //    t -= Time.deltaTime * 10;
        //    yield return null;
        //}
        //Debug.Log("broken");
    }

    private void setColor(Color col)
    {
        if (fracturedWhole) {
            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                transform.GetChild(0).GetChild(i).GetComponent<Renderer>().material.SetColor("_BaseColor", col);
            }
        }
        else
        {
            renderer.material.SetColor("_BaseColor", Color.grey);
        }
    }

    internal void SetStats()
    {
        stats.SetSpeedbaseOnLevel();
        stats.SetHealthBasedOnLevel();
        SetAgentStats();
    }

    private void Die()
    {
        FindObjectOfType<CameraShake>().shakeWithMagnitude(.03f);
        if (fracturedWhole) fracturedWhole.Collaspe();
        GV.Singleton().UpdateScoreText();
        DisableScript();
    }
}
