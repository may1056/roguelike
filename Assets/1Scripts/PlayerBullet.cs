using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public bool severe; //심각한지

    public float bulletSpeed;

    public GameObject fadeEffect;
    public Sprite doubleCircle;

    SpriteRenderer sr;

    float t;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        t = 1;

        if (severe) //심각한 탄알일 경우
        {
            GetComponent<CircleCollider2D>().isTrigger = false;
            //GetComponent<Rigidbody2D>().gravityScale = 0.5f;
            transform.localScale = 2f * Vector2.one;
        }

        float rand = 0.1f * Random.Range(0, 10);
        sr.color = new Color(rand, rand, rand); //무채색 랜덤

    } //Start End


    void Update()
    {
        if (severe) //심각한 탄알일 경우
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
                    pb.GetComponent<PlayerBullet>().severe = false;
                    pb.GetComponent<CircleCollider2D>().isTrigger = true;
                    //pb.GetComponent<Rigidbody2D>().gravityScale = 0;
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
                MakeEffect(transform.position, Color.gray);
                Destroy(gameObject);
            }
        }


        Vector2 tp = transform.position;

        transform.Translate(bulletSpeed * t * Time.deltaTime * Vector2.right);


        if (Mathf.Abs(tp.x) > 100 || Mathf.Abs(tp.y) > 100) Destroy(gameObject);

    } //Update End


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9) //플랫폼
        {
            //MakeEffect(Color.gray);
            //Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy") && severe) //적
        {
            collision.transform.GetComponent<Monster>().hp--;
            collision.transform.GetComponent<Monster>().ModifyHp();

            MakeEffect(collision.transform.position, Color.red);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9 && !severe) //플랫폼
        {
            MakeEffect(transform.position, Color.gray);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Enemy")) //적
        {
            other.GetComponent<Monster>().hp--;
            other.GetComponent<Monster>().ModifyHp();

            MakeEffect(transform.position, Color.red);
            Destroy(gameObject);
        }
    }



    void MakeEffect(Vector2 v, Color c)
    {
        GameObject eff = Instantiate(fadeEffect, v, Quaternion.identity);
        SpriteRenderer effsr = eff.GetComponent<SpriteRenderer>();
        effsr.sprite = doubleCircle;
        effsr.color = c;
    }

} //PlayerBullet End
