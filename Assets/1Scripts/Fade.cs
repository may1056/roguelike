using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour //서서히 사라지는 효과
{
    private SpriteRenderer sr;
    float fadetimegoes = 0;


    //한글 주석 으알아ㅣ너ㅣㅇ란알미ㅏㅈ댜ㅗㅜㅜㅇ피ㅗ타제버주드숱포켜

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        fadetimegoes += 3 * Time.deltaTime;
        if (fadetimegoes > 1) Destroy(gameObject);
        sr.color = new Color(1, 1, 1, 1 - fadetimegoes);
    }
}
