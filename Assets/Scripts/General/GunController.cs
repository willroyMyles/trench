using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    RotationController rotationController;
    SpawnController spawnController;
    FireController fireController;
    LineController lineController;
    GunBase stats = new GunBase();


    float leftMostRotation = -90;
    float rightmostRotation = 90;
    Vector3 firstTouch, movedTouch = Vector3.zero;
    bool isPlayerController = false;
    bool ableToShoot = false;
    internal LineRenderer lineRenderer;
    bool isSingleMode = false;
    
    void Start()
    {
        isPlayerController = gameObject.tag == "player one" ? true : false;
        lineRenderer = GetComponent<LineRenderer>();
        ConfirgueLineRenderer();
        FindControllers();
        ConfigureColor();
        ConfirgueMode();
        GV.Singleton().gameEnded += DisableScript;
    }

    public void DisableScript()
    {
        enabled = false;
    }

    private void ConfirgueLineRenderer()
    {
        lineRenderer.enabled = false;
        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .05f;
        lineRenderer.startColor = new Color(.5f, .6f, .9f, .5f);
        lineRenderer.endColor = new Color(.5f, .6f, .9f, 0f);
    }

    private void ConfirgueMode()
    {
        //if game mode is single, opponent will have no gun, only spwners
        if(GV.Singleton().gameModes == GameModes.Single && !isPlayerController)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            isSingleMode = true;
        }
    }

    private void FindControllers()
    {
        rotationController = GetComponentInChildren<RotationController>();
        fireController = GetComponentInChildren<FireController>();
        spawnController = GetComponent<SpawnController>();
        lineController = GetComponentInChildren<LineController>();

        spawnController.SetUp(isPlayerController);
        lineController.SetUp(isPlayerController);

        fireController.SetFirerate(stats.fireRate);
    }

    private void ConfigureColor()
    {
        if (!isPlayerController) GetComponent<Renderer>().material.SetColor("_BaseColor", GV.Singleton().enemyColor);
    }

    // Update is called once per frame
    void Update()
    {


        if (isPlayerController) moveByPlayer();
        else moveByAi();

        if (Input.GetMouseButtonDown(0) && GV.Singleton().powerUpAvailable)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var mask = LayerMask.GetMask("power up");
            if (Physics.Raycast(ray, out var hit, 30, mask))
            {
                GetPowerup(hit.collider.gameObject);
            }
        }
    }

    private void GetPowerup(GameObject gameObject)
    {
        var powerup = gameObject.GetComponent<PowerUps>().type;

        if (powerup == PowerUptype.FireRateIncrease) fireController.SetFirerate(stats.ModifyFireRate( true));
        if (powerup == PowerUptype.FireRateDecrease) fireController.SetFirerate(stats.ModifyFireRate( false));
        if (powerup == PowerUptype.DamageIncrease) fireController.SetFirerate(stats.ModifyFireRate( false));
        if (powerup == PowerUptype.DamageDecrease) fireController.SetFirerate(stats.ModifyFireRate( false));


        StartCoroutine(MovePowerupTobase(gameObject));

        //should flash gun controller

    }

    private IEnumerator MovePowerupTobase(GameObject gameObject)
    {
        //should flash controller

        while (false)
        {
            gameObject.transform.position = Vector3.Lerp( gameObject.transform.position, this.gameObject.transform.position, .1f * Time.deltaTime);
        //    if (gameObject.transform.position.z >= transform.position.z - 1) break;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(.2f);
        Destroy(gameObject);
    }

    private void moveByAi()
    {

        if (GV.Singleton().IsSingleMode())
        {
            // no gun movement
        }
        else
        {
            var adjust = 180;
            var movement = ((firstTouch - movedTouch));
            var rot = movement.x / 500 * 270;
            rotationController.gameObject.transform.eulerAngles = new Vector3(0, Mathf.Clamp(rot + adjust, leftMostRotation + adjust, rightmostRotation + adjust), 0);
            DrawLine();
        }
        // spawn controller
    }

    private void moveByPlayer()
    {
        var movement = ((firstTouch - movedTouch));
        var rot = movement.x / 500 * 90;
        rotationController.gameObject.transform.eulerAngles = new Vector3(0, Mathf.Clamp(rot, leftMostRotation, rightmostRotation), 0);
        DrawLine();

    }

    private void DrawLine()
    {
        if (ableToShoot)
        {
            lineRenderer.SetPosition(0, fireController.gameObject.transform.position);
            lineRenderer.SetPosition(1, fireController.gameObject.transform.position + fireController.gameObject.transform.up * 10);
        }

    }

    private void OnMouseDown()
    {
        firstTouch = Input.mousePosition;
        fireController.ResetCoolDownTime();
        ableToShoot = true;
        fireController.setAutoFire(ableToShoot);
        lineRenderer.enabled = ableToShoot;
      
    }

    private void OnMouseDrag()
    {
        movedTouch = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        firstTouch = movedTouch = Vector3.zero;
        ableToShoot = false;
        fireController.setAutoFire(ableToShoot);
        lineRenderer.enabled = ableToShoot;

    }
}
