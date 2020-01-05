using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturedWhole : MonoBehaviour
{
  public void chipOffPiece( bool shouldChip = false)
    {
        if (shouldChip)
        {
            chip( transform.GetChild(UnityEngine.Random.Range(0, transform.childCount)).gameObject);
        }
    }

    private IEnumerator changeColor(GameObject child)
    {
        child.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.black);
        yield return null;
    }

    private void chip(GameObject child)
    {
        //turn on mesh collider
        child.GetComponent<MeshCollider>().enabled = true;

        //change color gradually to black

        //remove child from parent
        child.transform.parent = null;

        //attatch rigid body to child
        child.AddComponent<Rigidbody>();

        //attatch fractured script
        child.AddComponent<FracturedScript>();

        StartCoroutine(changeColor(child));
    }

    public void Collaspe()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
           chip( transform.GetChild(i).gameObject);
        }

        Destroy(transform.parent.gameObject);

    }
}
