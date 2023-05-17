using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int pbType; //player bullet type
    //0: 총지팡이 일반 탄알, 1: 총지팡이 심각한 탄알, 2: 자동 공격 탄알

    public float bulletSpeed;

    public GameObject fadeEffect;
    public Sprite doubleCircle;

    SpriteRenderer sr;

    float t;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        t = 1;

        switch (pbType)
        {
            case 0: GrayRandom(); break;

            case 1:
                GrayRandom();
                GetComponent<CircleCollider2D>().isTrigger = false;
                transform.localScale = 2f * Vector2.one;
                break;
        }

    } //Start End


    void GrayRandom() //무채색 랜덤
    {
        float rand = 0.1f * Random.Range(0, 10);
        sr.color = new Color(rand, rand, rand);
    }


    void Update()
    {
        if (pbType == 1) //심각한 탄알일 경우
        {
            t -= 0.5f * Time.deltaTime;

            if (t <= 0.5f)
            {
                float rand = 0.1f * Random.Range(0, 10);
                sr.color = new Color(rand, rand, rand); //무채색 랜덤 계속 바뀜
            }

            if (t <= 0)
            {
                //transform.Translate(10 * Vector2.up);
                for(int i = 0; i < 8; i++) //8방향으로 심각하지 않은 탄알 날리기
                {
                    GameObject pb = Instantiate(this.gameObject,
                        transform.position, Quaternion.Euler(0, 0, 45 * i));
                    pb.transform.localScale = Vector2.one;
                    pb.GetComponent<PlayerBullet>().pbType = 0;
                    pb.GetComponent<CircleCollider2D>().isTrigger = true;
                    pb.transform.GetChild(0).gameObject.SetActive(false);
                }
                Destroy(gameObject);
            }
        }
        else //심각하지 않은 탄알은 그냥 시간 되면 없어지기
        {
            t -= Time.deltaTime;

            if (t <= 0)
            {
                MakeEffect(transform.position, Color.gray, pbType == 0 ? 1 : 0.4f);
                Destroy(gameObject);
            }
        }


        Vector2 tp = transform.position;

        transform.Translate(bulletSpeed * t * Time.deltaTime * Vector2.right);


        if (Mathf.Abs(tp.x) > 100 || Mathf.Abs(tp.y) > 100) Destroy(gameObject);

    } //Update End


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && pbType == 1) //적
        {
            collision.transform.GetComponent<Monster>().hp--;
            collision.transform.GetComponent<Monster>().ModifyHp();

            MakeEffect(collision.transform.position, Color.red, 1);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9 && pbType != 1) //플랫폼
        {
            MakeEffect(transform.position, Color.gray, 1);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Enemy")) //적
        {
            Monster m = other.GetComponent<Monster>();

            switch (pbType)
            {
                case 0:
                    m.hp--;
                    m.ModifyHp();
                    MakeEffect(transform.position, Color.red, 1);
                    break;

                case 2:
                    if (m.pollution > 0.5f) m.pollution = 1;
                    else m.pollution += 0.5f;
                    if (m.polluted)
                    {
                        m.hp--;
                        m.ModifyHp();
                        m.pollution = 0.5f;
                        MakeEffect(transform.position, new Color(0.6f, 0.4f, 1), 0.7f);
                    }
                    break;
            }
            Destroy(gameObject);
        }

    } //OnTriggerEnter2D End



    void MakeEffect(Vector2 v, Color c, float sc)
    {
        GameObject eff = Instantiate(fadeEffect, v, Quaternion.identity);
        SpriteRenderer effsr = eff.GetComponent<SpriteRenderer>();
        effsr.sprite = doubleCircle;
        effsr.color = c;
        eff.transform.localScale = sc * Vector2.one;
    }

} //PlayerBullet End
