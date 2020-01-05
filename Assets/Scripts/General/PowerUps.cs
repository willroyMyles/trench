using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUptype : int
{
    // artillery power ups
    /// <summary>
    /// increase fire rate
    /// </summary>
    FireRateIncrease,
    /// <summary>
    /// Decreases fire rate
    /// </summary>
    FireRateDecrease,
    /// <summary>
    /// Increases artillery damage
    /// </summary>
    DamageIncrease,
    DamageDecrease,
    //BounceIncrease,
    //BounceDecrease,
    //RicochetEnabled,
    //RicochetDisabled,
    //WallBounceEnabled,
    //WallBounceDisabled,


    //minion power ups
}

public class PowerUps : MonoBehaviour
{
    internal PowerUptype type;
    internal float lifeTime = 3;
    internal float currentTime = 0;


    public Texture fri, frd, di, dd;

    private void Awake()
    {
        GV.Singleton().powerUpAvailable = true;
        
        SetType( (PowerUptype)UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(PowerUptype)).Length));
    }

    public void SetType(PowerUptype powerUptype)
    {
        type = powerUptype;
        // should set material sprite from here
        if (type == PowerUptype.FireRateIncrease) GetComponent<Renderer>().material.SetTexture("_BaseMap", fri);
        if(type == PowerUptype.FireRateDecrease) GetComponent<Renderer>().material.SetTexture("_BaseMap", frd);
        if (type == PowerUptype.DamageIncrease) GetComponent<Renderer>().material.SetTexture("_BaseMap", di);
        if (type == PowerUptype.DamageDecrease) GetComponent<Renderer>().material.SetTexture("_BaseMap", dd);
        
    }

    private void Update()
    {
        if (currentTime < lifeTime) currentTime += Time.deltaTime;
        else Delete();
    }

    public void Delete()
    {
        GV.Singleton().powerUpAvailable = false;
        Destroy(gameObject);
    }

}
