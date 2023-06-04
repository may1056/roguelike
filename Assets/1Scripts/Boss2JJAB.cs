using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2JJAB : MonoBehaviour
{
    SpriteRenderer sr;

    public int num;

    Vector2 myCenter;
    float myRadius;

    float t;


    public bool playerknows = false;


    public GameObject fadeEffect;
    public Sprite doubleCircle;



    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        myCenter = Boss2.boss2.orbitCenter[num];
        myRadius = Boss2.boss2.orbitRadius[num];

        t = Random.Range(0, 10); //시간 랜덤 시작
        MyPosition();

        MakeEffect(doubleCircle, Color.white);
        InvokeRepeating(nameof(FollowEffect), 0.1f, 0.1f);
    }


    void Update()
    {
        t += 0.02f * (100 - Boss2.boss2.hp) * Time.deltaTime;
        MyPosition();

        if (playerknows) sr.color = Color.gray; //보스 발각
        else sr.color = new Color(0.5f, 1, 0.5f); //초록이기는 한데 좀 밝은 색

    } //Update End


    void MyPosition()
    {
        transform.position = new Vector2(
            myRadius * Mathf.Cos(t) + myCenter.x,
            myRadius * Mathf.Sin(t) + myCenter.y);
    }


    void MakeEffect(Sprite s, Color c)
    {
        GameObject eff =
            Instantiate(fadeEffect, transform.position, Quaternion.identity);
        SpriteRenderer effsr = eff.GetComponent<SpriteRenderer>();
        effsr.sprite = s;
        effsr.color = c;
    }


    void FollowEffect()
    {
        MakeEffect(sr.sprite, sr.color);
    }

    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerknows && other.CompareTag("Player")
            && Player.unbeatableTime <= 0) Player.hurted = true;
    }
    */

} //Boss2JJAB End
