using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public List<Transform> _walkLines;

    float _t = 0;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _t += 0.1f;
        }

        transform.position = MoveCharacter(_t, _walkLines);
    }

    public Vector3 MoveCharacter(float t, List<Transform> walkLines)
    {
        if (Math.Abs(t - 4) < 0.001f)
        {
            t = 0;
        }

        float totalDistance = 0;

        for (int i = 0; i < walkLines.Count; i++)
        {
            Transform startTransform = walkLines[i];
            Transform endTransform = walkLines[i % (walkLines.Count-1)];

            totalDistance += (endTransform.position - startTransform.position).magnitude;
        }

        float goalDist = totalDistance * t;

        float accumilatedDistance = 0;

        for (int i = 0; i < walkLines.Count; i++)
        {
            Transform startTransform = walkLines[i];
            Transform endTransform = walkLines[i % (walkLines.Count - 1)];

            float sectionDistance = (endTransform.position - startTransform.position).magnitude;

            accumilatedDistance += sectionDistance;

            if (accumilatedDistance >= goalDist)
            {
                float startDistance = accumilatedDistance - sectionDistance;

                float TBetween = Mathf.InverseLerp(startDistance, accumilatedDistance, goalDist);

                return startTransform.position + (endTransform.position - startTransform.position) * TBetween;
                
            }
        }

        return Vector3.zero;
    }


    public float DistanceToTValue()
    {
        return -1;
    }
}
