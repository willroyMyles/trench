using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update


    float magnitude = .01f;
    float duration = .1f;
    float initialDuration = .1f;
    bool shouldShake = false;
    Vector3 originalPosition, secondPos;
    bool cameraMoving = false;

    private void Start()
    {
        originalPosition = transform.position;
        secondPos = transform.position + new Vector3(0, 0, -1);
    }

    public void shakeWithMagnitude(float mag)
    {
        if (shouldShake) return;
        shouldShake = true;
        StartCoroutine(shake(mag));
    }
    public void Shake()
    {
        if (shouldShake) return;
        shouldShake = true;
        StartCoroutine(shake(null));
    }

    private IEnumerator shake(float? mag)
    {
        float temp = mag.HasValue ? mag.Value : magnitude;

        while ((duration >= 0))
        {
            transform.position = originalPosition + Random.insideUnitSphere * temp;
            duration -= Time.deltaTime;
            yield return null;
        }

        duration = initialDuration;
        transform.position = originalPosition;
        shouldShake = false;
    }

    public IEnumerator kick()
    {

        if (cameraMoving) yield break;
        cameraMoving = true;
        float t = 0;
       
        float speed = 1f;

        while ((t <= speed * Time.deltaTime))
        {
            transform.position = Vector3.Lerp(transform.position, secondPos, speed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        while ((t > 0))
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, speed * Time.deltaTime);
            t -= Time.deltaTime;
            yield return null;
        }

        cameraMoving = false;
    }

    private void Update()
    {
        //if (shouldShake)
        //{
        //    if (duration >= 0)
        //    {
        //        transform.position = originalPosition + Random.insideUnitSphere * magnitude;
        //        duration -= Time.deltaTime;
        //    }
        //    else
        //    {
        //        shouldShake = false;
        //        duration = initialDuration;
        //        transform.position = originalPosition;
        //    }
        //}
    }
}
