using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour //서서히 사라지는 효과
{
    private SpriteRenderer sr;
    float t; //fadetimegoes

    public bool up_down = false; //true: 짙어짐(up), false: 옅어짐(down)
    public float k = 3;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (up_down) t = 0; //t 증가

        else //t 감소
        {
            t = 1;
            k = -k;
        }

    } //Start End

    void Update()
    {
        t += k * Time.deltaTime;

        if (up_down ? t > 1 : t < 0) Destroy(gameObject);

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, t);

    } //Update End

} //Fade End
