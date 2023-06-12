using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R : MonoBehaviour
{
    float t = 0;
    private void Update()
    {
        if (GameManager.gameManager.making) t += 0.1f * Time.deltaTime;
        else t = 0;
        transform.rotation = Quaternion.Euler(0, 0, t * t);
    }
}
