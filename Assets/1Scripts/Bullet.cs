using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour //탄막
{
    public int bulletType;
    //0: 일반 딜, 1: 슬로우, 2: 지속 딜, 4: 비

    public float bulletSpeed;


    Rigidbody2D rigid;
    SpriteRenderer sr;


    public GameObject fadeEffect;
    public Sprite doubleCircle;

    public GameObject pxb2, pxb1;



    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        switch (bulletType)
        {
            case 5:
                rigid.AddTorque(30);
                InvokeRepeating(nameof(Next2px), 1, 1);
                break;

            case 6:
                rigid.AddTorque(15);
                InvokeRepeating(nameof(Next1px), 1, 1);
                break;
        }

    } //Start End



    void Update()
    {
        Vector2 tp = transform.position;

        transform.Translate(bulletSpeed * Time.deltaTime * Vector2.right);

        if (Mathf.Abs(tp.x) > 100 || Mathf.Abs(tp.y) > 100)
        {
            Destroy(gameObject);
            Debug.Log("밖");
        }


        //스킬 범위 내에 있음
        if (Mathf.Abs(PlayerAttack.skillP.y) < 100 &&
            Vector2.Distance(tp, PlayerAttack.skillP) < 5.5f)
        {
            Destroy(gameObject);
            Debug.Log("스");
        }



        //무기 파생 스킬 범위 내에 있음
        Vector2 wsp = PlayerAttack.wsP;

        if (Mathf.Abs(wsp.y) < 100)
        {
            switch (PlayerAttack.weaponNum.Item1)
            {
                case 0:
                    bool inX = Mathf.Abs(wsp.x - tp.x) < 7.5f
                        && Mathf.Abs(wsp.y - tp.y) < 1;
                    bool inY = Mathf.Abs(wsp.y - tp.y) < 7.5f
                        && Mathf.Abs(wsp.x - tp.x) < 1;
                    if (inX || inY)
                    {
                        Destroy(gameObject);
                        Debug.Log("무");
                    }
                    break;
            }
        }

        /*
        //위치 저장에 의한 파괴
        if (Mathf.Abs(Player.posP[0].y) < 100)
        {
            for (int i = 0; i < 2; i++)
            {
                if (Vector2.Distance(tp, Player.posP[i]) < 3) Destroy(gameObject);
            }
        }*/

        if (bulletType >= 4 && bulletType <= 7 &&
            (tp.x > 19 || tp.x < -19 || tp.y > 9 || tp.y < -9))
        {
            if (sr != null) MakeEffect(sr.color, 1);
            Destroy(gameObject);
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
                case 4: case 5: case 6: case 7: MakeEffect(sr.color, 0.1f); break;
            }
            Destroy(gameObject);
            Debug.Log("콜");
        } //플랫폼
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        int l = other.gameObject.layer;

        if (l == 9) //그라운드
        {
            switch (bulletType)
            {
                case 0: MakeEffect(Color.gray, 0.5f); break;
                case 1: MakeEffect(new Color(0.56f, 0.71f, 0.84f), 1); break;
                case 4: case 5: case 6: case 7:
                    if (sr != null) MakeEffect(sr.color, 1);
                    Destroy(gameObject); break;
            }
        }

        if (l == 8) //블록
        {
            switch (bulletType)
            {
                case 4: MakeEffect(sr.color, 0.1f); Destroy(gameObject); break;
                case 5: MakeEffect(sr.color, 0.3f); Destroy(gameObject); break;
                case 6: MakeEffect(sr.color, 0.2f); Destroy(gameObject); break;
                case 7: MakeEffect(sr.color, 0.1f); Destroy(gameObject); break;
            }
        }

        if (l == 11 || l == 13) //플레이어
        {
            switch (bulletType)
            {
                case 0:
                    if (Player.unbeatableTime <= 0) Player.hurted = true;
                    MakeEffect(Color.red, 0.5f);
                    Destroy(gameObject);
                    break;

                case 1:
                    if (Player.unbeatableTime <= 0)
                    {
                        Player pl = other.transform.GetComponent<Player>();
                        pl.slowtime = 2;
                        if (pl.slow > 0.98f) pl.slow = 1;
                        else pl.slow += 0.02f;
                    }
                    Destroy(gameObject);
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
                    Destroy(gameObject);
                    break;
            }
        }

        if (l == 21)
        {
            switch (bulletType)
            {
                case 4:
                case 5:
                case 6:
                case 7:
                    if (Player.unbeatableTime <= 0) Player.hurted = true;
                    MakeEffect(Color.red, 1);
                    Destroy(gameObject);
                    break;
            }
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


    void Next2px()
    {
        Instantiate(pxb2, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
    }
    void Next1px()
    {
        Instantiate(pxb1, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
    }

} //Bullet End
