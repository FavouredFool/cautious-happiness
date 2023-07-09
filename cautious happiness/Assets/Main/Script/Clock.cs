using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform hourHand;

    public Transform minuteHand;

    public float t = 0;

    public float speed = 5;
    public void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            t -= speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            t += speed * Time.deltaTime;
        }

        if (t >= 1)
        {
            t = t - 1;
        }

        if (t < 0)
        {
            t = 1 - t;
        }

        TToHandMovement(t);
    }

    public void TToHandMovement(float t)
    {
        hourHand.transform.rotation = Quaternion.Euler(0, 0, 360 * (1-t));

        float tNew = t * 12;
        minuteHand.transform.rotation = Quaternion.Euler(0, 0, 360 * (1-tNew));
    }
}
