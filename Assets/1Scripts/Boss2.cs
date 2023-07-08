using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Boss2 : MonoBehaviour
{
    //두 번째 보스 - 자연의 섭리
    public static Boss2 boss2;

    Rigidbody2D rigid;
    BoxCollider2D col;
    SpriteRenderer sr;

    float t; //시간

    public int hp;
    public Text hpText; //임시 체력 텍스트
    public Image hpBAR;
    public Image hpCASE;

    Player player;
    public Camera cam, uicam;
    SpriteRenderer bgsr;


    public bool phase2 = false;


    //pattern0
    public bool hide = false;
    public GameObject jjab; //프리팹 2ssoB
    bool orbitRotating = true;
    float dashx, dashy;
    int mynum;
    public Vector2[] orbitCenter = //궤도 중심
        { Vector2.zero, new Vector2(-6.5f, -2), new Vector2(6.5f, -2), new Vector2(-11.5f, 2.5f), new Vector2(11.5f, 2.5f) };
    public float[] orbitRadius = //궤도 반지름
        { 7.5f, 6.5f, 6.5f, 4.5f, 4.5f };
    GameObject[] jjabs = new GameObject[4];
    public GameObject pxb1; //1 pixel bullet
    public GameObject hpOrb, mpOrb, coinOrb;

    //pattern1
    public GameObject rain;
    //readonly float[] rainR = { 0.3f, 0.4f, 0.4f, 0.4f, 0.5f, 0.5f, 0.6f, 0.6f, 0.7f, 0.8f },
    //    rainG = { 0.6f, 0.6f, 0.7f, 0.8f, 0.7f, 0.8f, 0.7f, 0.8f, 0.8f, 0.9f };
    public Sprite rainfrom;

    //pattern2
    public GameObject pxb3; //3 pixel bullet
    public Sprite bulletborder; //탄막 껍데기 스프라이트

    //pattern3
    public GameObject fadeEffect;
    public Sprite bigpx;
    int[] x = new int[12];
    int[] y = new int[12];
    int thenumberofsquares;
    public Sprite redfrom;


    public Sprite Iamboss;

    Tilemap ground, block;


    public Sprite Empty;
    public Sprite doubleCircle;


    Vector2 tp;
    bool inAttackArea; //플레이어의 공격 범위 내에 있는지
    public float pollution = 0; //오염 정도
    public bool polluted = false; //오염되었는지


    bool hm;
    Color hardc = new(1, 0.6f, 0.6f), easyc = new(0.6f, 0.7f, 1);
    public GameObject worldEnder;



    void Awake()
    {
        boss2 = this;
        gameObject.SetActive(false);
    }


    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        t = 0;

        player = Player.player;
        cam.orthographicSize = 12;
        uicam.orthographicSize = 12;
        bgsr = player.transform.GetChild(2).GetComponent<SpriteRenderer>();

        hm = GameManager.hardmode;

        thenumberofsquares = hm ? 6 : 3;

        Invoke(nameof(DashAttack), 1);

        hp = 80; //임시

        InvokeRepeating(nameof(FollowEffect), 0.1f, 0.1f);

        //<- 보스
        GameObject iab = Instantiate(fadeEffect, new Vector2(
            transform.position.x + 2, transform.position.y), Quaternion.identity);
        iab.transform.SetParent(transform);
        iab.GetComponent<SpriteRenderer>().sprite = Iamboss;
        iab.GetComponent<Fade>().k = 0.02f;

        ground = GameManager.gameManager.transform.GetChild(0).GetChild(0).GetComponent<Tilemap>();
        block = GameManager.gameManager.transform.GetChild(0).GetChild(1).GetComponent<Tilemap>();

        Soundmanager.soundmanager.bossbgm[1].Play();

        GameManager.gameManager.OnEnable();
        Player.player.OnEnable();
        PlayerAttack.playerAtk.OnEnable();

    } //Start End




    void Update()
    {
        cam.transform.position = -10 * Vector3.forward;
        uicam.transform.position = -10 * Vector3.forward;
        player.transform.GetChild(2).transform.position = Vector2.zero;

        tp = transform.position;


        ModifyHp();

        if (hp <= 0) //발광
        {
            transform.position = Vector2.zero;

            if (hp > -5)
            {
                CancelInvoke();
                hp = -10;
                t = 100;
                InvokeRepeating(nameof(Craziness),1,0.2f);
                sr.color = Color.white;
                if (jjabs[1] != null)
                {
                    for (int i = 0; i < 4; i++) Destroy(jjabs[i].gameObject);
                }
                col.isTrigger = true;
                MakeEffect(doubleCircle, Color.white);

                hpCASE.transform.GetChild(1).gameObject.SetActive(false);

                ground.color = Color.white;
                block.gameObject.SetActive(false);

                transform.GetChild(2).gameObject.SetActive(true);

                bgsr.color = Color.white;
            }

            t += Time.deltaTime;
            Soundmanager.soundmanager.bossbgm[1].volume = t < 130 ? (130 - t) / 30 : 0;
            if (t > 120) CancelInvoke(nameof(Craziness));
            if (t > 130) worldEnder.SetActive(true); //권한 위임
        }


        else //페이즈 1, 2
        {
            if (hp <= 30 && !phase2)
            {
                phase2 = true;
                thenumberofsquares = hm ? 12 : 6;
                hpText.color = new Color(1, 0, 0, 0.3f);

                ground.color = GameManager.hardmode ? hardc : easyc;
                block.color = GameManager.hardmode ? hardc : easyc;
                for (int i = 0; i < 20; i++)
                {
                    ParticleSystem.MainModule p2main =
                        ground.transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>().main;
                    p2main.startColor = hm ? hardc : easyc;
                }
                ground.transform.GetChild(0).gameObject.SetActive(true);

                ParticleSystem.MainModule p2centermain =
                    transform.GetChild(1).GetComponent<ParticleSystem>().main;
                p2centermain.startColor = hm ? hardc : easyc;
                transform.GetChild(1).gameObject.SetActive(true);

                bgsr.color = hm ? hardc : easyc;
            }


            if (hide) sr.color = Color.white;

            if (orbitRotating)
            {
                t -= 0.02f * (100 - hp) * (1 - pollution) * Time.deltaTime;

                transform.position = new Vector2(
                    orbitRadius[mynum] * Mathf.Cos(t) + orbitCenter[mynum].x,
                    orbitRadius[mynum] * Mathf.Sin(t) + orbitCenter[mynum].y);

                col.isTrigger = true;
            }
            else
            {
                rigid.velocity = (1 - Time.deltaTime) * rigid.velocity;

                MakeEffect(sr.sprite, sr.color);

                col.isTrigger = false;
            }

            //이하 Monster.cs에서 가져옴

            //피 닳는 시스템
            if (inAttackArea && (Input.GetMouseButtonDown(0)
                || Input.GetKeyDown("j")) && //내가 마우스가 없어서 임시로 설정한 키
                PlayerAttack.curAttackCooltime >= PlayerAttack.maxAttackCooltime
                 && GameManager.prgEnd)
            {
                Apa(Color.red);
                hp -= Player.player.atkPower;
                if (Player.player.purple) //보라 수정: 치명타
                {
                    int r = Random.Range(0, 10);
                    if (r < 2)
                    {
                        hp--;
                        Debug.Log("치명");
                        player.MakeEffect(new Vector2(tp.x, tp.y + 2), player.critical, 5, 1);
                    }
                }
                PlayerAttack.curAttackCooltime = 0;
            }
            //스킬 범위 내에 있음
            if (Mathf.Abs(PlayerAttack.skillP.y) < 100 &&
                Vector2.Distance(tp, PlayerAttack.skillP) < 5.5f)
            {
                Apa(Color.red);
                hp--;
                if (Player.player.purple) //보라 수정: 치명타
                {
                    int r = Random.Range(0, 10);
                    if (r < 2)
                    {
                        hp--;
                        Debug.Log("치명");
                        player.MakeEffect(new Vector2(tp.x, tp.y + 2), player.critical, 5, 1);
                    }
                }
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
                            Apa(Color.red);
                            hp -= 2 * Player.player.atkPower;
                            if (Player.player.purple) //보라 수정: 치명타
                            {
                                int r = Random.Range(0, 10);
                                if (r < 2)
                                {
                                    hp -= 2;
                                    Debug.Log("치명");
                                    player.MakeEffect(new Vector2(tp.x, tp.y + 2), player.critical, 5, 1);
                                }
                            }
                            if (Player.player.poison)
                                Invoke(nameof(AfterDamage), Random.Range(1, 20));
                        }
                        break;
                }
            }
            //자동 공격 오염
            polluted = pollution == 1;
            if (polluted) Invoke(nameof(RemovePollution), 1);
            else if (pollution > 0 && !polluted) pollution -= 0.3f * Time.deltaTime;
            transform.GetChild(0).GetComponent<SpriteRenderer>().color
                = new Color(1, 1, 1, pollution);
            transform.GetChild(0).gameObject.SetActive(pollution > 0);
        }

    } //Update End





    void Craziness() //발광
    {
        for (int i = 0; i < (t < 110 ? (hm ? 15 : 5) : (hm ? 25 : 15)); i++)
        {
            GameObject b = Instantiate(pxb1, tp,
                Quaternion.Euler(0, 0, 10 * i + hp));
            b.GetComponent<SpriteRenderer>().color = Color.gray;
            b.GetComponent<Bullet>().bulletSpeed = Random.Range(4, 6);
            b.transform.localScale = 0.1f * Random.Range(5, 21) * Vector2.one;

            GameObject _b = Instantiate(pxb1, tp,
                Quaternion.Euler(0, 0, -10 * i - hp));
            _b.GetComponent<SpriteRenderer>().color = Color.black;
            _b.GetComponent<Bullet>().bulletSpeed = Random.Range(4, 6);
            _b.transform.localScale = 0.1f * Random.Range(5, 21) * Vector2.one;

            GameObject __b = Instantiate(pxb1, tp,
                Quaternion.Euler(0, 0, Random.Range(0, 360)));
            __b.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0.75f, 0.75f);
            __b.GetComponent<Bullet>().bulletSpeed = Random.Range(3, 5);
            __b.transform.localScale = 0.1f * Random.Range(5, 21) * Vector2.one;

            GameObject ___b = Instantiate(pxb1, tp,
                Quaternion.Euler(0, 0, Random.Range(0, 360)));
            ___b.GetComponent<SpriteRenderer>().color = new Color(0.25f, 0.25f, 0.25f);
            ___b.GetComponent<Bullet>().bulletSpeed = Random.Range(5, 7);
            ___b.transform.localScale = 0.1f * Random.Range(5, 21) * Vector2.one;
        }
        hp -= 10 + Random.Range(0, 3); //발광 때부터 체력의 의미가 랜덤으로 감소하는 숫자로 변질됨.
    }



    void DashAttack() //패턴0-A. 반짝거리면서 플레이어에게 빠르게 달려든다
    {
        if (jjabs[1] != null)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject newOrb = Random.Range(0, 3) switch //???이게 뭐시여
                {
                    0 => hpOrb,
                    1 => mpOrb,
                    _ => coinOrb,
                };
                Instantiate(newOrb, jjabs[i].transform.position, Quaternion.identity);
                Destroy(jjabs[i].gameObject);
            }

            if (Player.player.pink) Instantiate(hpOrb);
            if (Player.player.blue) Instantiate(mpOrb);
        }
        hide = false; //숨지 않아 - 숨는다는 의미는 분신들 사이에 숨어있다.
        orbitRotating = false; //궤도에서 벗어난다

        dashx = Player.player.transform.position.x - tp.x;
        dashy = Player.player.transform.position.y - tp.y;

        rigid.AddForce((10 + 0.2f * (100 - hp) * (1 - pollution)) //자동 공격 오염 받음. 무려
            * (hm ? 1 : 0.5f) * new Vector2(dashx, dashy).normalized, ForceMode2D.Impulse); //가속. 플레이어한테.

        for (int i = 0; i < (phase2 ? (hm ? 18 : 12) : (hm ? 9 : 6)); i++)
            Invoke(nameof(DashBullet), i * (0.9f / (phase2 ? (hm ? 18 : 12) : (hm ? 9 : 6)))); //뒤로 탄막 뿌림

        if (!IsInvoking(nameof(HideMyself))) Invoke(nameof(HideMyself), 1);
    }
    void DashBullet()
    {
        sr.color = new Color(Random.Range(0, 11) * 0.1f,
                Random.Range(0, 11) * 0.1f, Random.Range(0, 11) * 0.1f); //랜덤 색상 지정

        GameObject db = Instantiate(pxb1, tp, Quaternion.Euler( //pixel bullet 1px
            0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan2(dashy, dashx))); //dashbullet
        db.transform.GetComponent<SpriteRenderer>().color = sr.color;
        db.transform.GetComponent<Bullet>().bulletSpeed = 0.7f;

        MakeEffect(bulletborder, sr.color);
    }
    void HideMyself() //패턴0-B. 텔레포트하면서 랜덤 궤도에서 도는 분신 생성
    {
        mynum = Random.Range(0, 5); //5개 중 하나

        bool my = false;
        for (int i = 0; i < 5; i++)
        {
            if (i == mynum) my = true;
            else
            {
                int I = my ? i - 1 : i;
                jjabs[I] = Instantiate(jjab);
                jjabs[I].GetComponent<Boss2JJAB>().num = i;
            }
        }
        MakeEffect(doubleCircle, Color.white);

        orbitRotating = true; //궤도를 돌기 시작

        hide = true; //숨는다
        InvokeRepeating(nameof(JjabBullet), 0, phase2 ? 1.5f : 2);
    }
    void JjabBullet() //패턴0-C. 발각되기 전까지는 초록 탄막 발사
    {
        Color Green = new(Random.Range(0, 6) * 0.1f, Random.Range(6, 11) * 0.1f, Random.Range(0, 6) * 0.1f); //다양한 초록색
        int Speed = Random.Range(1, 6); //1~5 속도 랜덤
        if (phase2) Speed += 1; //2페이즈는 더 빠름

        for (int i = 0; i < 4; i++) //작은 i
        {
            for (int j = 0; j < (phase2 ? (hm ? 12 : 6) : (hm ? 8 : 4)); j++) //한 번에 탄막을 몇 번 쏘느냐?
            {
                Vector2 jtp = jjabs[i].transform.position; //분신의 위치
                GameObject jjabB = Instantiate(pxb1, jtp, Quaternion.Euler(0, 0, //jjabBullet
                    t * 360 + j * (360 / (phase2 ? (hm ? 12 : 6) : (hm ? 8 : 4))))); //t는 시간에 따라 요상하게 변하는 변수, j*90 (0<=j<=3)
                jjabB.GetComponent<SpriteRenderer>().color = Green;
                jjabB.GetComponent<Bullet>().bulletSpeed = Speed;

                GameObject eff = Instantiate(fadeEffect, jtp, Quaternion.identity);
                SpriteRenderer effsr = eff.GetComponent<SpriteRenderer>();
                effsr.sprite = bulletborder;
                effsr.color = Green;
            }
        }
    }
    public void PlayerKnows()
    {
        hide = false; //숨는 것 끝
        sr.color = Color.white;
        CancelInvoke(nameof(JjabBullet)); //짭에서 초록 탄막 이제 그만 쏴

        for (int i = 0; i < 4; i++)
            jjabs[i].GetComponent<Boss2JJAB>().playerknows = true; //분신들도 알아차렸다!

        MakeRainFrom(true, true, 1);
        if (phase2) MakeRainFrom(false, true, 1);

        //데미지를 입으면 드러나고, 1초 뒤에 비가 내리고 2초 뒤에 붉은 탄막 날리고 4초 뒤에 1차 스퀘어 6초 뒤에 2차 스퀘어 8초 뒤에 처음부터 반복
        Invoke(nameof(Rain), 1);
        Invoke(nameof(LetBullet), 2);
        Invoke(nameof(Square), 4);
        Invoke(nameof(Square), 6);
        Invoke(nameof(DashAttack), 8);
    }



    void Rain() //패턴1. 1px 비가 내린다
    {
        for (int i = 0; i < (hm ? (phase2 ? 50 : 80) : 10); i++)
            Invoke(nameof(RainMaker), i * 1.99f / (hm ? (phase2 ? 40 : 60) : 10));

        MakeRainFrom(true, false, 0.3f);
        if (phase2) MakeRainFrom(false, false, 0.3f);
    }
    void RainMaker()
    {
        //페이즈1: 위쪽에서 떨어짐
        GameObject bi = Instantiate(rain, new Vector2(
            Random.Range(-179, 180) * 0.1f, 8.9f), Quaternion.identity);
        bi.GetComponent<SpriteRenderer>().color
            = new Color(Random.Range(0, 5) * 0.1f, Random.Range(0, 5) * 0.1f, Random.Range(7, 11) * 0.1f); //다양한 푸른색

        if (phase2) //페이즈2: 아래쪽에서도 올라감
        {
            GameObject bi2 = Instantiate(rain, new Vector2(
                Random.Range(-179, 180) * 0.1f, -8.9f), Quaternion.identity);
            bi2.GetComponent<SpriteRenderer>().color
                = new Color(Random.Range(0, 5) * 0.1f, Random.Range(0, 5) * 0.1f, Random.Range(7, 11) * 0.1f); //다양한 푸른색
            bi2.GetComponent<Rigidbody2D>().gravityScale = -1;
        }
    }
    void MakeRainFrom(bool ceiling, bool ud, float k)
    {
        GameObject rf = Instantiate(fadeEffect, (ceiling ? 9.5f : -9.5f) * Vector2.up,
            Quaternion.Euler(0, 0, ceiling ? 0 : 180));
        Fade rff = rf.GetComponent<Fade>();
        rff.up_down = ud;
        rff.k = k;
        rf.GetComponent<SpriteRenderer>().sprite = rainfrom;
    }



    void LetBullet() //패턴2. 3px 탄막이 2px 탄막을 뿌리고 2px 탄막이 1px 탄막을 뿌림
    {
        for (int i = 0; i < (phase2 ? (hm ? 8 : 4) : (hm ? 4 : 2)); i++)
            Invoke(nameof(ReleaseBullet), i * (0.9f / (phase2 ? (hm ? 9 : 6) : (hm ? 5 : 3))));
    }
    void ReleaseBullet()
    {
        Instantiate(pxb3, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));

        MakeEffect(bulletborder, new Color(1, 0.4f, 0));
    }



    void Square() //패턴3. 배경을 18분할하여 랜덤 선정 데미지
    {
        //x좌표는 -15, -9, -3, 3, 9, 15 중 하나, y좌표는 -6, 0, 6 중 하나
        for (int i = 0; i < thenumberofsquares; i++)
        {
            bool equal = true;
            while (equal)
            {
                equal = false;
                x[i] = Random.Range(0, 6) * 6 - 15;
                y[i] = Random.Range(-1, 2) * 6;
                for (int j = 0; j < i; j++)
                {
                    if (x[i] == x[j] && y[i] == y[j]) equal = true;
                }
            }
        }

        MakeSquare(0.5f, true, 1);
        Invoke(nameof(DeathSquare), 1);

        MakeRedFrom(true, true, 1);
        MakeRedFrom(false, true, 1);
    }
    void DeathSquare()
    {
        MakeSquare(1, false, 2);

        MakeRedFrom(true, false, 2);
        MakeRedFrom(false, false, 2);

        float px = player.transform.position.x, py = player.transform.position.y;
        bool area = false;
        for (int i = 0; i < thenumberofsquares; i++)
        {
            if (x[i] - 3 < px && px < x[i] + 3 && y[i] - 3 < py && py < y[i] + 3)
                area = true;

            if (i < thenumberofsquares / 2)
            {
                //붉은 탄막 (왼)
                GameObject left = Instantiate(pxb1, new Vector2(
                    -17.9f, Random.Range(-89, 90) * 0.1f), Quaternion.identity);
                RedBullet(left.transform);
                //붉은 탄막 (오)
                GameObject right = Instantiate(pxb1, new Vector2(
                    17.9f, Random.Range(-89, 90) * 0.1f), Quaternion.Euler(0, 0, 180));
                RedBullet(right.transform);
            }
        }

        if (area && Player.unbeatableTime <= 0) Player.hurted = true;
    }
    void RedBullet(Transform rb)
    {
        rb.GetComponent<SpriteRenderer>().color = Color.red;
        rb.GetComponent<Bullet>().bulletSpeed = Random.Range(1, 6);
        rb.localScale = 1.25f * Vector2.one;
    }
    void MakeSquare(float red, bool ud, int k)
    {
        for (int i = 0; i < thenumberofsquares; i++)
        {
            GameObject sq = Instantiate(fadeEffect,
                new Vector2(x[i], y[i]), Quaternion.identity);
            Fade sqf = sq.GetComponent<Fade>();
            sqf.up_down = ud;
            sqf.k = k;
            SpriteRenderer sqsr = sq.GetComponent<SpriteRenderer>();
            sqsr.sprite = bigpx;
            sqsr.color = new Color(red, 0, 0);
        }
    }
    void MakeRedFrom(bool wall, bool ud, int k)
    {
        GameObject rf = Instantiate(fadeEffect, (wall ? 18.5f : -18.5f) * Vector2.right,
            Quaternion.Euler(0, 0, wall ? 0 : 180));
        Fade rff = rf.GetComponent<Fade>();
        rff.up_down = ud;
        rff.k = k;
        rf.GetComponent<SpriteRenderer>().sprite = redfrom;
    }




    void MakeEffect(Sprite s, Color c)
    {
        GameObject eff = Instantiate(fadeEffect, tp, Quaternion.identity);
        SpriteRenderer effsr = eff.GetComponent<SpriteRenderer>();
        effsr.sprite = s;
        effsr.color = c;
    }


    void FollowEffect()
    {
        MakeEffect(sr.sprite, sr.color);
    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!orbitRotating && collision.gameObject.CompareTag("Player"))
            Player.hurted = true;
    }


    public void RemovePollution() //오염 제거
    {
        if (polluted) pollution = 0;
        CancelInvoke(nameof(RemovePollution));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            inAttackArea = true; //들어간다
            Debug.Log("들어가");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            inAttackArea = false; //빠져나온다
    }
    public void Apa(Color c)
    {
        if (hp > 0)
        {
            GameObject hurt = Instantiate(fadeEffect, transform.position,
                Quaternion.identity);

            SpriteRenderer hsr = hurt.transform.GetComponent<SpriteRenderer>();
            //hsr.flipX = sr.flipX;
            hsr.sortingOrder = 21;
            hsr.sprite = Empty;
            hsr.color = c;

            hurt.GetComponent<Fade>().k = 5;

            if (hide) PlayerKnows();
        }
    }
    void ModifyHp() //hp bar 최신화
    {
        hpText.text = hp.ToString(); //임시

        if (hp > 0) hpBAR.rectTransform.sizeDelta = new Vector2(hp * 4, 70);
        else hpBAR.gameObject.SetActive(false);

        if (t > 100 && t < 120)
            hpCASE.rectTransform.sizeDelta = new Vector2(16 * (120 - t), 70);
        else if (t >= 120) hpCASE.gameObject.SetActive(false);
    }
    public void RepeatAD() //AfterDamage() 반복
    {
        Invoke(nameof(AfterDamage), Random.Range(1, 20));
    }
    void AfterDamage() //poison 아이템 - Invoke용
    {
        if (hp > 0)
        {
            Apa(Color.green);
            hp--;

            if (Player.player.purple) //보라 수정: 치명타
            {
                int r = Random.Range(0, 10);
                if (r < 2)
                {
                    hp--;
                    Debug.Log("치명");
                    player.MakeEffect(new Vector2(tp.x, tp.y + 2), player.critical, 5, 1);
                }
            }
        }
    }



} //Boss2 End
