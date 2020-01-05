using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GentleMove : MonoBehaviour
{

    internal float moveLeftInterval = 3;
    internal float moveRightInterval = 3;
    internal float distance = 3;
    internal float speed = .4f;

    Vector3 rightPos, leftPos;
    bool moveRightFirst;
    // Start is called before the first frame update
    private void Awake()
    {
        moveRightFirst = UnityEngine.Random.Range(-1, 1) == 0 ? true : false;
        rightPos = transform.position +new Vector3(moveRightInterval, 0, 0);
        leftPos = transform.position - new Vector3(moveLeftInterval, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (moveRightFirst) // move right
        {
            transform.position = Vector3.Slerp(transform.position, rightPos, speed * Time.deltaTime);
            if ( Mathf.Abs(  transform.position.x - rightPos.x ) <= .5f ) moveRightFirst = false;
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, leftPos, speed * Time.deltaTime);
            if ( Mathf.Abs( transform.position.x - leftPos.x ) <= .5f) moveRightFirst = true;
        }
    }
}
