using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldEnder : MonoBehaviour
{
    float t = 0;
    public Camera cam;
    public Image white;

    void Start()
    {
        white.gameObject.SetActive(true);
    }

    void Update()
    {
        t += 5 * Time.deltaTime;
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, t * t);

        cam.transform.position = 0.05f * new Vector2(Random.Range(-(int)t, (int)t + 1), Random.Range(-(int)t, (int)t + 1));
        cam.orthographicSize = 12 + t * 0.1f;

        if (t > 40 && t < 45) white.color = new Color(1, 1, 1, 0.2f * (t - 40));
    }

} //WorldEnder End
