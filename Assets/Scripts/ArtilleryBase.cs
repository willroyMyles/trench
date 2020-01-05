using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryBase : MonoBehaviour
{
    float lifeTime = 2.2f;
    float currentTime = 0f;
    float bulletSpeed = 10f;
    float speedincrease = 1.3f;
    float speedDecrease = 1.3f;
    Vector3 velocity;

    internal float damage = 1f;
    internal float damageAddOn = 0f;
    float damageFallOff;
    internal float damageFallOffAmount = 2;
    Rigidbody rigidbody;

    bool isDefelectable = true;

    public float GetDamage()
    {
        return damage + damageAddOn;
    }

    private void Awake()
    {
        damageFallOff = damage / damageFallOffAmount;
        rigidbody = GetComponent<Rigidbody>();
        tag = "artillery";
    }
    void Start()
    {

    }

    public void setUpBall(Vector3 dir, float damageAddOn, string whoIBelongTo = "")
    {
        velocity = dir * bulletSpeed;
        GetComponent<Rigidbody>().velocity = velocity;
        this.damageAddOn = damageAddOn;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= lifeTime) Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            //Destroy(gameObject);
            //find way to get bounce?
            //rigidbody.AddForce(transform.forward * 5, ForceMode.Impulse);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isDefelectable)
        {

        }

    }
}
