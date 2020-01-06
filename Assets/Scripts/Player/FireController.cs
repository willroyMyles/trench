using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public GameObject bulletPrefab;

    float fireRate = .8f;
    float nextFire = 0;
    float coolDownTime = 0;
    float spawnDistance = .26f;
    bool fire = false;
    bool autoFire = false;
    bool barrelMoving = false;

    GameObject lightAndMuzzleHolder;
    internal float damageAddOn = 0;

    Vector3 finalPosition;

    void Start()
    {
        lightAndMuzzleHolder = transform.GetChild(0).gameObject;
        lightAndMuzzleHolder.SetActive(false);

    }

    IEnumerator FlashMuzzle()
    {
        lightAndMuzzleHolder.SetActive(true);
        yield return new WaitForSecondsRealtime(.01f);
        lightAndMuzzleHolder.SetActive(false);
    }


    public void SetFirerate(float rate)
    {
        fireRate = rate;
    }

    public void setDamageAddOn(float addOn)
    {
        damageAddOn = addOn;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDownTime < fireRate)
        {
            coolDownTime += Time.deltaTime;
        }
        else
        {
            coolDownTime = fireRate;
            if (autoFire) Fire();

        }

        if (fire)
        {

            coolDownTime = 0;
            fire = false;
        }
        // if (Input.GetMouseButtonDown(0)) Fire();
    }

    public void Fire()
    {
        if (coolDownTime == fireRate)
        {
            fire = true;
            var spawnPos = gameObject.transform.position + gameObject.transform.up * spawnDistance;

            StartCoroutine(FlashMuzzle());
            StartCoroutine(Camera.main.GetComponent<CameraShake>().kick());
            var bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            bullet.GetComponent<ArtilleryBase>().setUpBall( dir : transform.up,whoIBelongTo: gameObject.tag, damageAddOn: damageAddOn);

            //instantiate shell
            Instantiate(GV.Singleton().shell, transform.position + new Vector3(.3f, 0, .2f), GV.Singleton().shell.transform.rotation);
            
            //push player back
            //consider moving turret
           // if(!barrelMoving)   StartCoroutine(MoveBarrel()); // isnt noticable

        }
    }

    private IEnumerator MoveBarrel()
    {
        barrelMoving = true;
        var vec = transform.position - new Vector3(0, 0f, 0.05f);
        while (true)
        {
            //move it back
            transform.position = Vector3.Lerp(transform.position, vec, 200 * Time.deltaTime);
            yield return null;
            if (transform.position == vec) break;
        }
        //yield return new WaitForSecondsRealtime(1);
        vec = transform.position + new Vector3(0, 0, 0.05f);
        while (true)
        {
            //move it forward
            transform.position = Vector3.Lerp(transform.position, vec, 10 / Time.deltaTime);
            yield return null;
            if (transform.position == vec) break;
        }
        barrelMoving = false;
    }

    public bool getCanFire()
    {
        return coolDownTime == fireRate;
    }

    public void setAutoFire(bool val)
    {
        autoFire = val;
    }
}
