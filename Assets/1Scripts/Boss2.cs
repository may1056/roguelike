using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    //두 번째 보스 - 자연의 섭리

    float T; //주기
    float t; //시간


    public GameObject rain;
    public GameObject pxb3; //3 pixel bullet


    void Start()
    {
        T = 15; //임시
        t = 0;

        InvokeRepeating(nameof(Rain), 5, T);
        InvokeRepeating(nameof(LetBullet), 10, T);
    }




    void Update()
    {
        t += 0.5f * Time.deltaTime;
        transform.position = 7.5f * new Vector2(Mathf.Cos(t), Mathf.Sin(t));
    }



    void Rain() //패턴 1. 비
    {
        for (int i = 0; i < 20; i++)
            Invoke(nameof(RainMaker), i * 0.1f);
    }
    void RainMaker()
    {
        GameObject bi = Instantiate(rain, new Vector2(
            Random.Range(-189, 190) * 0.1f, 9.9f), Quaternion.identity);

        bi.GetComponent<SpriteRenderer>().color
            = new Color(0, Random.Range(0, 5) * 0.1f, 1); //푸른색
    }


    void LetBullet()
    {
        for (int i = 0; i < 4; i++) Instantiate(pxb3,
                transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
    }

} //Boss2 End
