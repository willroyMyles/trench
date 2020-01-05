using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturedScript : MonoBehaviour
{
    internal float lifeTime = 15.2f;
    internal float currentTime = 0;
    internal float speed = 1;
    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= lifeTime) StartCoroutine(SinkThenDestroy());
    }

    IEnumerator SinkThenDestroy()
    {
        var pos = transform.position - Vector3.up;
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * speed);
            if (transform.position.y <= pos.y) break;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("artillery"))
        {
            StartCoroutine(SinkThenDestroy());
        }
    }
}
