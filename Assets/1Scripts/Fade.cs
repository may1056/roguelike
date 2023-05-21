using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour //서서히 사라지는 효과
{
    private SpriteRenderer sr;
    float fadetimegoes = 0;
    public float k = 3;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        fadetimegoes += k * Time.deltaTime;
        if (fadetimegoes > 1) Destroy(gameObject);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1 - fadetimegoes);
    }
}
