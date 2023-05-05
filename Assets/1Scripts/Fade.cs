using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour //������ ������� ȿ��
{
    private SpriteRenderer sr;
    float fadetimegoes = 0;

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
