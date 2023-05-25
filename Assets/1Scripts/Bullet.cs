using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour //탄막
{
    public int bulletType;
    //0: 일반 딜, 1: 슬로우, 2: 지속 딜

    public float bulletSpeed;

    public GameObject fadeEffect;
    public Sprite doubleCircle;

    float r1, r2;



    void Start()
    {

        r1 = Random.Range(0, 10) * 0.2f;
    }

    void Update()
    {
        Vector2 tp = transform.position;

        transform.Translate(bulletSpeed * (bulletType == 0 ? r1 : 1)
            * Time.deltaTime * Vector2.right);

        if (Mathf.Abs(tp.x) > 100 || Mathf.Abs(tp.y) > 100) Destroy(gameObject);


        //스킬 범위 내에 있음
        if (Mathf.Abs(PlayerAttack.skillP.y) < 100 &&
            Vector2.Distance(tp, PlayerAttack.skillP) < 5.5f) Destroy(gameObject);



        //무기 파생 스킬 범위 내에 있음
        Vector2 wsp = PlayerAttack.wsP;

        if (Mathf.Abs(wsp.y) < 100)
        {
            switch (Player.weaponNum)
            {
                case 0:
                    bool inX = Mathf.Abs(wsp.x - tp.x) < 7.5f
                        && Mathf.Abs(wsp.y - tp.y) < 1;
                    bool inY = Mathf.Abs(wsp.y - tp.y) < 7.5f
                        && Mathf.Abs(wsp.x - tp.x) < 1;
                    if (inX || inY) Destroy(gameObject);
                    break;
            }
        }


        //위치 저장에 의한 파괴
        if (Mathf.Abs(Player.posP[0].y) < 100)
        {
            for (int i = 0; i < 2; i++)
            {
                if (Vector2.Distance(tp, Player.posP[i]) < 3) Destroy(gameObject);
            }
        }

    } //Update End


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.tag == "Attack")
        {
            switch (bulletType)
            {
                case 0: MakeEffect(Color.gray, 0.5f); break;
                case 1: MakeEffect(new Color(0.56f, 0.71f, 0.84f), 1); break;
            }
            Destroy(gameObject);
        } //플랫폼
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9) //플랫폼
        {
            switch (bulletType)
            {
                case 0: MakeEffect(Color.gray, 0.5f); break;
                case 1: MakeEffect(new Color(0.56f, 0.71f, 0.84f), 1); break;
            }
            Destroy(gameObject);
        }

        int l = other.gameObject.layer;
        if (l == 11 || l == 13) //플레이어
        {
            switch (bulletType)
            {
                case 0:
                    if (Player.unbeatableTime <= 0) Player.hurted = true;
                    MakeEffect(Color.red, 0.5f);
                    break;

                case 1:
                    if (Player.unbeatableTime <= 0)
                    {
                        Player pl = other.transform.GetComponent<Player>();
                        pl.slowtime = 2;
                        if (pl.slow > 0.965f) pl.slow = 1;
                        else pl.slow += 0.035f;
                    }
                    break;

                case 2:
                    if (Player.unbeatableTime <= 0)
                    {
                        Player pl = other.transform.GetComponent<Player>();
                        pl.burntime = 1;
                        if (pl.burn > 0.965f && pl.burn < 1)
                        {
                            pl.burn = 1;
                            pl.RepeatEx();
                        }
                        else pl.burn += 0.035f;
                    }
                    break;
            }
            Destroy(gameObject);
        }

    } //OnTriggerEnter2D End


    void MakeEffect(Color c, float sc)
    {
        GameObject eff =
            Instantiate(fadeEffect, transform.position, Quaternion.identity);
        SpriteRenderer effsr = eff.GetComponent<SpriteRenderer>();
        effsr.sprite = doubleCircle;
        effsr.color = c;
        eff.transform.localScale = sc * Vector2.one;
    }

} //Bullet End
