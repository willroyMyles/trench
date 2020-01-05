using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    bool isPlayerController = false;
    internal float slowTimeSpeed = 10;

    public void SetUp(bool playerControlled)
    {
        isPlayerController = playerControlled;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerController)
        {
            if (other.CompareTag("enemyMinion")) // have to get parent because emepty gameobjcets with colliders doesnt work seemingly
            {
                //game over
                Debug.Log("game finished");
                GV.Singleton().SetGameEnded(true);
                //StartCoroutine(SlowTimeAndEnd(other.gameObject.transform));
            }
        }
        else
        {
            //check who came into who (snicker)
        }
    }

    private IEnumerator SlowTimeAndEnd(Transform transform)
    {

        //zoom in on player

        //slow time and end
        Time.timeScale = 1 / slowTimeSpeed;
        Time.fixedDeltaTime /= slowTimeSpeed;
        yield return new WaitForSecondsRealtime(2);
    }
}
