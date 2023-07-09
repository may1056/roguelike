using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour //탄막
{
    public int bulletType;
    //0: 일반 딜, 1: 슬로우, 2: 지속 딜, 8: 실명

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

            case 9:
                Transform ps = transform.GetChild(1);
                ps.localScale = 0.75f * transform.localScale;
                ps.gameObject.SetActive(true);
                break;
        }

    } //Start End



    void Update()
    {
        Vector2 tp = transform.position;

        transform.Translate(bulletSpeed * Time.deltaTime * Vector2.right);

        if (Mathf.Abs(tp.x) > 200 || Mathf.Abs(tp.y) > 200)
        {
            switch (bulletType)
            {
                case 0: case 1: case 2: case 8: case 9: DestroyReverb(); break;
                default: Destroy(gameObject); break;
            }
        }


        //스킬 범위 내에 있음
        if (Mathf.Abs(PlayerAttack.skillP.y) < 200 &&
            Vector2.Distance(tp, PlayerAttack.skillP) < 5.5f)
        {
            switch (bulletType)
            {
                case 0: case 1: case 2: case 8: case 9: DestroyReverb(); break;
                default: Destroy(gameObject); break;
            }
        }



        //무기 파생 스킬 범위 내에 있음
        Vector2 wsp = PlayerAttack.wsP;

        if (Mathf.Abs(wsp.y) < 200)
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
                        switch (bulletType)
                        {
                            case 0: case 1: case 2: case 8: case 9: DestroyReverb(); break;
                            default: Destroy(gameObject); break;
                        }
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

        switch (bulletType)
        {
            case 4: case 5: case 6: case 7: case 9:
                if (tp.x > 19 || tp.x < -19 || tp.y > 9 || tp.y < -9)
                {
                    //if (sr != null) MakeEffect(sr.color, 1);
                    Destroy(gameObject);
                }
                break;
        }

    } //Update End


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.tag == "Attack")
        {
            switch (bulletType)
            {
                case 0: MakeEffect(Color.gray, 0.5f); DestroyReverb(); break;

                case 1: MakeEffect(new Color(0.56f, 0.71f, 0.84f), 1); DestroyReverb(); break;
                case 2: MakeEffect(new Color(1, 0.6313f, 0), 1); DestroyReverb(); break;
                case 8: MakeEffect(Color.black, 1); DestroyReverb(); break;
                case 9: MakeEffect(Color.white, 1); DestroyReverb(); break;

                case 4: case 5: case 6: case 7: //MakeEffect(sr.color, 0.1f);
                    Destroy(gameObject); break;
            }
            //Debug.Log("콜");
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
                case 2: MakeEffect(new Color(1, 0.6313f, 0), 1); break;
                case 8: MakeEffect(Color.black, 1); break;

                case 4: case 5: case 6: case 7:
                    //if (sr != null) MakeEffect(sr.color, 1);
                    Destroy(gameObject); break;

                case 9:
                    if (sr != null) MakeEffect(sr.color, 1);
                    DestroyReverb(); break;
            }
        }

        if (l == 8) //블록
        {
            switch (bulletType)
            {
               case 4: case 5: case 6: case 7:
                    //if (sr != null) MakeEffect(sr.color, 1);
                    Destroy(gameObject); break;

                case 9:
                    if (sr != null) MakeEffect(sr.color, 1);
                    DestroyReverb(); break;
            }
        }

        if (l == 11 || l == 13) //플레이어
        {
            switch (bulletType)
            {
                case 0:
                    if (Player.unbeatableTime <= 0) Player.hurted = true;
                    MakeEffect(Color.red, 0.5f);
                    DestroyReverb();
                    break;

                case 1:
                    if (Player.unbeatableTime <= 0)
                    {
                        Player pl = other.transform.GetComponent<Player>();
                        pl.slowtime = 2;
                        if (pl.slow > 0.975f) pl.slow = 1;
                        else pl.slow += 0.025f;
                    }
                    DestroyReverb();
                    break;

                case 2:
                    if (Player.unbeatableTime <= 0)
                    {
                        Player pl = other.transform.GetComponent<Player>();
                        pl.burntime = 1;
                        if (pl.burn > 0.97f && pl.burn < 1)
                        {
                            pl.burn = 1;
                            pl.RepeatEx();
                        }
                        else pl.burn += 0.03f;
                    }
                    DestroyReverb();
                    break;

                case 8:
                    if (Player.unbeatableTime <= 0)
                    {
                        Player pl = other.transform.GetComponent<Player>();
                        pl.darktime = 2;
                        if (pl.dark > 0.975f && pl.dark < 1)
                        {
                            pl.dark = 1;
                            if (PlayerAttack.playerAtk.mp > 0) PlayerAttack.playerAtk.mp--;
                        }
                        else pl.dark += 0.025f;
                    }
                    DestroyReverb();
                    break;
            }
        }

        if (l == 21) //Touhou
        {
            switch (bulletType)
            {
                case 4: case 5: case 6: case 7:
                    if (Player.unbeatableTime <= 0) Player.hurted = true;
                    //MakeEffect(Color.red, 1);
                    Destroy(gameObject);
                    break;

                case 9: //효과 삼합 발광 탄알
                    if (Player.unbeatableTime <= 0)
                    {
                        Player.hurted = true;
                        Player pl = Player.player;

                        pl.slowtime = 2;
                        if (pl.slow > 0.5f) pl.slow = 1;
                        else pl.slow += 0.5f;

                        pl.burntime = 1;
                        if (pl.burn > 0.5f && pl.burn < 1)
                        {
                            pl.burn = 1;
                            pl.RepeatEx();
                        }
                        else pl.burn += 0.5f;

                        pl.darktime = 2;
                        if (pl.dark > 0.5f && pl.dark < 1)
                        {
                            pl.dark = 1;
                            if (PlayerAttack.playerAtk.mp > 0) PlayerAttack.playerAtk.mp--;
                        }
                        else pl.dark += 0.5f;
                    }
                    MakeEffect(Color.red, 1);
                    DestroyReverb();
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
        GameObject PXB = Instantiate(pxb2, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        PXB.GetComponent<Bullet>().bulletSpeed = Random.Range(2, 6);

        SpriteRenderer PXBsr = PXB.GetComponent<SpriteRenderer>();
        PXBsr.color = new Color(PXBsr.color.r + 0.1f * Random.Range(-2, 1),
            PXBsr.color.g + 0.1f * Random.Range(-1, 2), PXBsr.color.b + 0.1f * Random.Range(0, 3));
    }
    void Next1px()
    {
        GameObject PXB = Instantiate(pxb1, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        PXB.GetComponent<Bullet>().bulletSpeed = Random.Range(3, 8);

        SpriteRenderer PXBsr = PXB.GetComponent<SpriteRenderer>();
        PXBsr.color = new Color(PXBsr.color.r + 0.1f * Random.Range(-2, 1),
            PXBsr.color.g + 0.1f * Random.Range(-1, 2), PXBsr.color.b + 0.1f * Random.Range(0, 3));
    }


    void DestroyReverb() //파티클 시스템이 내장되어 있는 탄알 - 파티클 효과가 탄알 파괴와 동시에 사라지지 않게 하기
    {
        //if (transform.childCount == 2) Destroy(transform.GetChild(0).gameObject);

        if (transform.childCount == 1)
        {
            ParticleSystem.MainModule psmain = transform.GetChild(0).GetComponent<ParticleSystem>().main;
            psmain.loop = false;

            transform.DetachChildren();
        }

        Destroy(gameObject);
    }

} //Bullet End
