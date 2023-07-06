using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Straight : MonoBehaviour
{
    public float moveSpeed, moveAngle;

    void Update()
    {
        transform.Translate(moveSpeed *
            new Vector2(Mathf.Cos(Mathf.Deg2Rad * moveAngle), Mathf.Sin(Mathf.Deg2Rad * moveAngle)));
    }

} //Straight End
