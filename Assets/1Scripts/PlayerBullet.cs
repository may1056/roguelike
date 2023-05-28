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

    float r1, r2;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        switch (pbType)
        {
            case 0:
                r1 = Random.Range(3, 8) * 0.2f;
                r2 = Random.Range(3, 8) * 0.2f;
                break;

            case 1:
                r1 = Random.Range(3, 8) * 0.2f;
                r2 = Random.Range(3, 8) * 0.2f;
                GetComponent<CircleCollider2D>().isTrigger = false;
                transform.localScale = 2f * Vector2.one;
                break;

            case 2: r1 = 1; r2 = 1; break;
        }

        t = r2;

    } //Start End



    void Update()
    {
        if (pbType == 1) //심각한 탄알일 경우
        {
            t -= 0.5f * Time.deltaTime;

            if (t <= r2 * 0.5f)
            {
                float rand = 0.1f * Random.Range(0, 10);
                sr.color = new Color(rand, rand, rand); //무채색 랜덤 계속 바뀜
            }

            if (t <= 0)
            {
                //transform.Translate(10 * Vector2.up);
                for (int i = 0; i < 8; i++) //8방향으로 심각하지 않은 탄알 날리기
                {
                    GameObject pb = Instantiate(this.gameObject,
                        transform.position, Quaternion.Euler(0, 0, 45 * i));
                    pb.transform.localScale = Vector2.one;
                    pb.GetComponent<PlayerBullet>().pbType = 0;
                    pb.GetComponent<CircleCollider2D>().isTrigger = true;
                    pb.transform.GetChild(0).gameObject.SetActive(false);
                    float rand = 0.1f * Random.Range(0, 10);
                    pb.GetComponent<SpriteRenderer>().color
                        = new Color(rand, rand, rand); //무채색 랜덤
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

        transform.Translate(bulletSpeed * (pbType == 0 ? r1 : 1) * t * Time.deltaTime * Vector2.right);


        if (Mathf.Abs(tp.x) > 100 || Mathf.Abs(tp.y) > 100) Destroy(gameObject);

    } //Update End


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") &&
            collision.gameObject.layer == 7 && pbType == 1) //적
        {
            Monster m = collision.transform.GetComponent<Monster>();
            m.Apa(Color.red);
            m.hp -= Player.player.atkPower;
            if (Player.player.purple) //보라 수정: 치명타
            {
                int r = Random.Range(0, 10);
                if (r < 2)
                {
                    m.hp--;
                    Debug.Log("치명");
                }
            }
            if (Player.player.poison) m.RepeatAD();

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
                    m.Apa(Color.red);
                    m.hp -= Player.player.atkPower;
                    if (Player.player.purple) //보라 수정: 치명타
                    {
                        int r = Random.Range(0, 10);
                        if (r < 2)
                        {
                            m.hp--;
                            Debug.Log("치명");
                        }
                    }
                    if (Player.player.poison) m.RepeatAD();
                    MakeEffect(transform.position, Color.red, 1);
                    Destroy(gameObject);
                    break;

                case 2:
                    if (m.pollution > 0.5f) m.pollution = 1;
                    else m.pollution += 0.5f;
                    if (m.polluted)
                    {
                        m.pollution = 0.5f;
                        m.Apa(Color.red);
                        m.hp--; //자동 공격은 버서커 딜증 대상 아님
                        if (Player.player.purple) //보라 수정: 치명타
                        {
                            int r = Random.Range(0, 10);
                            if (r < 2)
                            {
                                m.hp--;
                                Debug.Log("치명");
                            }
                        }
                        if (Player.player.poison) m.RepeatAD();
                        MakeEffect(transform.position, new Color(0.6f, 0.4f, 1), 0.7f);
                        CancelInvoke(nameof(m.RemovePollution));
                    }
                    Destroy(gameObject);
                    break;
            }
        }

        if (other.CompareTag("Boss2")) //자연의 섭리
        {
            Boss2 b2 = other.transform.GetComponent<Boss2>();
            if (b2.hide) b2.PlayerKnows();

            switch (pbType)
            {
                case 0:
                case 1:
                    b2.Apa(Color.red);
                    b2.hp -= Player.player.atkPower;
                    if (Player.player.purple) //보라 수정: 치명타
                    {
                        int r = Random.Range(0, 10);
                        if (r < 2)
                        {
                            b2.hp--;
                            Debug.Log("치명");
                        }
                    }
                    if (Player.player.poison) b2.RepeatAD();

                    MakeEffect(other.transform.position, Color.red, 1);
                    break;

                case 2:
                    if (b2.pollution > 0.5f) b2.pollution = 1;
                    else b2.pollution += 0.5f;
                    if (b2.polluted)
                    {
                        b2.pollution = 0.5f;
                        b2.Apa(Color.red);
                        b2.hp--; //자동 공격은 버서커 딜증 대상 아님
                        if (Player.player.purple) //보라 수정: 치명타
                        {
                            int r = Random.Range(0, 10);
                            if (r < 2)
                            {
                                b2.hp--;
                                Debug.Log("치명");
                            }
                        }
                        if (Player.player.poison) b2.RepeatAD();
                        MakeEffect(transform.position, new Color(0.6f, 0.4f, 1), 0.7f);
                        CancelInvoke(nameof(b2.RemovePollution));
                    }
                    Destroy(gameObject);
                    break;
            }
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
