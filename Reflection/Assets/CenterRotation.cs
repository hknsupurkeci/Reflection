using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterRotation : MonoBehaviour
{
    private float angel = 250;
    void Update()
    {
        //angel += 1;
        transform.RotateAround(transform.position, -transform.forward, angel * Time.deltaTime);
    }
}
