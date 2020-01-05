using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public GameObject gameController;
    internal bool isPlayerControlled;

    float coolDownRate = 0;
    float spawnRate = 9;
    float powerUpSpwanRate = 12;
    float powerUpCoolDown = 0;
    bool shouldSpawnPowerUp = false;
    bool shouldSpawn;
    public bool autoSpawn = true;
    bool ableToMove = false;
    float distanceToMove = .8f;

    // to manage neural nets
    int[] neuralNetworklayer = new int[] { 5, 50, 40, 2 };
    List<NeuralNetwork> neuralNetworksList = new List<NeuralNetwork>();
    NeuralNetwork nextNetworkToSpawn;
    int maxBrainsToKeep = 1;


    private void Awake()
    {
        if (GV.Singleton() == null) Instantiate(gameController);
    }
    public void SetUp(bool isPlayer)
    {
        isPlayerControlled = isPlayer;
        GV.Singleton().levelUpdated += ModifySpawnRate;
        GV.Singleton().gameEnded += stopSpawning;
    }

    public void stopSpawning()
    {
        this.enabled = false;
    }
    public void ModifySpawnRate(bool decrease = true)
    {
        if (spawnRate <= 3) return;
        if (decrease) spawnRate -= .5f;
        else spawnRate += .5f;
    }



    void Update()
    {
        if (!isPlayerControlled)
        {
            if (coolDownRate < spawnRate)
            {
                coolDownRate += Time.deltaTime;
            }
            else
            {
                coolDownRate = spawnRate;
                if (autoSpawn) Spawn();
            }

            if( powerUpCoolDown < powerUpSpwanRate)
            {
                powerUpCoolDown += Time.deltaTime;
            }
            else
            {
                powerUpCoolDown = powerUpSpwanRate;
                SpawnPowerUp();
            }

            if (shouldSpawn)
            {
                coolDownRate = 0;
                shouldSpawn = false;
            }
        }
        // if (Input.GetMouseButtonDown(0)) Fire();
    }

    private void SpawnPowerUp()
    {
        if (GV.Singleton().IsSingleMode())
        {
            Instantiate(GV.Singleton().powerUp, GV.Singleton().getRandompointOnPlane(), Quaternion.identity);
        }
        else
        {
            //spawn one with increase and one with decrease
        }
        powerUpCoolDown = 0;

    }

    public void UpdateNeuralNetworkList(NeuralNetwork brain)
    {
        neuralNetworksList.Add(brain);
    }

    public void Spawn()
    {
        if (coolDownRate == spawnRate)
        {
            shouldSpawn = true;
            var spawnPos = gameObject.transform.position + new Vector3(UnityEngine.Random.Range( -1,1), 0,0);
            var minion = Instantiate(GV.Singleton().PlayerMinion, spawnPos, Quaternion.identity);
            minion.GetComponentInChildren<Renderer>().material.SetColor("_BaseColor", GV.Singleton().enemyColor);

            //change all the parts in minion 
            for( int i =0; i < minion.transform.GetChild(0).childCount; i++)
            {
                minion.transform.GetChild(0).GetChild(i).GetComponent<Renderer>().material.SetColor("_BaseColor", GV.Singleton().enemyColor);
            }

            // add brain to minion
            if(neuralNetworksList.Count >= maxBrainsToKeep)
            {
                neuralNetworksList.Sort();
                var brain = neuralNetworksList.First();
                brain.Mutate();
                neuralNetworksList.Remove(brain);
                minion.GetComponent<MinionMovement>().SetBrain(brain, this);

            }
            else
            {
                var brain = new NeuralNetwork(neuralNetworklayer);
                brain.Mutate();
                minion.GetComponent<MinionMovement>().SetBrain(brain, this);
            }



            


            //if(neuralNetworksList.Count >= maxBrainsToKeep)
            //{
            //    //cull half and assign regular brain

            //    neuralNetworksList.Sort();
            //    var brain = neuralNetworksList.First();
            //    neuralNetworksList.Remove(brain);
            //    brain.Mutate();
            //    minion.GetComponent<MinionMovement>().InitiateNeuralNetwork(brain, this, GV.Singleton().playerGoal);

            //}
            //else
            //{
            //    var brain = new NeuralNetwork(neuralNetworklayer);
            //    brain.Mutate();
            //    neuralNetworksList.Add(brain);
            //    minion.GetComponent<MinionMovement>().InitiateNeuralNetwork(brain, this, GV.Singleton().playerGoal);
            //}
          


            //bullet.GetComponent<ArtilleryBase>().setUpBall(transform.up, gameObject.tag);

            //push player back
            //consider moving turret

            StartCoroutine(movePlayerCoroutine(minion));

        }
    }

    private IEnumerator movePlayerCoroutine(GameObject minion)
    {
        var offset = new Vector3(0, 0, - distanceToMove);
        var pos = minion.transform.position + offset;
        while (true)
        {
            minion.transform.position = Vector3.Lerp(minion.transform.position, minion.transform.position + offset, 6 * Time.deltaTime);
            if (minion.transform.position.z < pos.z) break;
            yield return null;
        }
        ableToMove = true;
        minion.GetComponent<MinionMovement>().setPlayerControlled(isPlayerControlled);
    }

    public bool getCanFire()
    {
        return coolDownRate == spawnRate;
    }

    public void setAutoFire(bool val)
    {
        autoSpawn = val;
    }
}
