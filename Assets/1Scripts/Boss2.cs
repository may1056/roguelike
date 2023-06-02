using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    Player player;
    public GameObject cam;


    public bool phase2 = false;


    //pattern0
    public bool hide = false;
    public GameObject jjab;
    bool orbitRotating = true;
    int mynum;
    public Vector2[] orbitCenter = //궤도 중심
        { Vector2.zero, new Vector2(-6.5f, -2), new Vector2(6.5f, -2), new Vector2(-11.5f, 2.5f), new Vector2(11.5f, 2.5f) };
    public float[] orbitRadius = //궤도 반지름
        { 7.5f, 6.5f, 6.5f, 4.5f, 4.5f };
    GameObject[] jjabs = new GameObject[4];
    public GameObject pxb1; //1 pixel bullet
    public GameObject hpOrb, mpOrb;

    //pattern1
    public GameObject rain;
    readonly float[] rainR = { 0.3f, 0.4f, 0.4f, 0.4f, 0.5f, 0.5f, 0.6f, 0.6f, 0.7f, 0.8f },
        rainG = { 0.6f, 0.6f, 0.7f, 0.8f, 0.7f, 0.8f, 0.7f, 0.8f, 0.8f, 0.9f };

    //pattern2
    public GameObject pxb3; //3 pixel bullet

    //pattern3
    public GameObject fadeEffect;
    public Sprite bigpx;
    int[] x = new int[12];
    int[] y = new int[12];
    int thenumberofsquares = 6;




    public Sprite Empty;
    public Sprite doubleCircle;


    Vector2 tp;
    bool inAttackArea; //플레이어의 공격 범위 내에 있는지
    public float pollution = 0; //오염 정도
    public bool polluted = false; //오염되었는지




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
        cam.GetComponent<Camera>().orthographicSize = 11;

        Invoke(nameof(DashAttack), 1);
        //InvokeRepeating(nameof(Rain), 5, T);
        //InvokeRepeating(nameof(LetBullet), 8, T);
        //InvokeRepeating(nameof(Square), 12, T);
        //InvokeRepeating(nameof(Square), 14, T);

        hp = 80; //임시

        InvokeRepeating(nameof(FollowEffect), 0.05f, 0.05f);
    }




    void Update()
    {
        if (hp <= 30)
        {
            phase2 = true;
            thenumberofsquares = 12;
            hpText.color = new Color(1, 0, 0, 0.3f);
        }

        cam.transform.position = -10 * Vector3.forward;


        if (hide) sr.color = Color.white;

        if (orbitRotating)
        {
            t -= 0.05f * (90 - hp) * (1 - pollution) * Time.deltaTime;
            transform.SetPositionAndRotation(new Vector2(
                orbitRadius[mynum] * Mathf.Cos(t) + orbitCenter[mynum].x,
                orbitRadius[mynum] * Mathf.Sin(t) + orbitCenter[mynum].y),
                Quaternion.Euler(0, 0, t));

            col.isTrigger = true;
        }
        else
        {
            rigid.velocity = (1 - Time.deltaTime) * rigid.velocity;

            sr.color = new Color(Random.Range(0, 11) * 0.1f,
                Random.Range(0, 11) * 0.1f, Random.Range(0, 11) * 0.1f);

            MakeEffect(doubleCircle, sr.color);
            MakeEffect(sr.sprite, sr.color);

            col.isTrigger = false;
        }




        if (hp <= 0) SceneManager.LoadScene(3);



        //이하 Monster.cs에서 가져옴

        tp = transform.position;
        ModifyHp();
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
                }
            }
        }
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
                            }
                        }
                        if (Player.player.poison)
                            Invoke(nameof(AfterDamage), Random.Range(1, 30));
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

    } //Update End






    void DashAttack() //패턴0-A. 반짝거리면서 플레이어에게 빠르게 달려든다
    {
        if (t < -4)
        {
            for (int i = 0; i < 4; i++)
            {
                Instantiate(Random.Range(0, 3) == 0 ? mpOrb : hpOrb,
                    jjabs[i].transform.position, Quaternion.identity);
                Destroy(jjabs[i].gameObject);
            }
        }
        hide = false;
        orbitRotating = false;

        rigid.AddForce((10 + 0.5f * (90 - hp) * (1 - pollution)) * new Vector2(
            Player.player.transform.position.x - tp.x,
            Player.player.transform.position.y - tp.y).normalized,
            ForceMode2D.Impulse);

        if (!IsInvoking(nameof(HideMyself))) Invoke(nameof(HideMyself), 1);
    }
    void HideMyself() //패턴0-B. 텔레포트하면서 랜덤 궤도에서 도는 분신 생성
    {
        mynum = Random.Range(0, 5);

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

        orbitRotating = true;

        hide = true;
        InvokeRepeating(nameof(JjabBullet), 0, phase2 ? 0.5f : 4);
    }
    void JjabBullet() //패턴0-C. 발각되기 전까지는 초록 탄막 발사
    {
        Color Green = new(Random.Range(0, 5) * 0.1f,
                Random.Range(7, 11) * 0.1f, Random.Range(0, 5) * 0.1f);
        int Speed = Random.Range(1, 6);
        if (phase2) Speed += 2;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < (phase2 ? 1 : 8); j++)
            {
                Vector2 jtp = jjabs[i].transform.position;
                GameObject jjabB = Instantiate(pxb1, jtp, Quaternion.Euler(0, 0,
                    phase2 ? Mathf.Rad2Deg * Mathf.Atan2(player.transform.position.y
                    - jtp.y, player.transform.position.x - jtp.x) : t * 360 + j * 45));
                jjabB.GetComponent<SpriteRenderer>().color = Green;
                jjabB.GetComponent<Bullet>().bulletSpeed = Speed;
            }
        }
    }
    public void PlayerKnows()
    {
        hide = false;
        sr.color = Color.white;
        CancelInvoke(nameof(JjabBullet));

        for (int i = 0; i < 4; i++)
            jjabs[i].GetComponent<Boss2JJAB>().playerknows = true;

        //데미지를 입으면 드러나고, 1초 뒤에 비가 내리고 2초 뒤에 붉은 탄막 날리고 4초 뒤에 1차 스퀘어 6초 뒤에 2차 스퀘어 10초 뒤에 처음부터 반복
        Invoke(nameof(Rain), 1);
        Invoke(nameof(LetBullet), 2);
        Invoke(nameof(Square), 4);
        Invoke(nameof(Square), 6);
        Invoke(nameof(DashAttack), 8);
    }



    void Rain() //패턴1. 1px 비가 내린다
    {
        for (int i = 0; i < 20; i++)
            Invoke(nameof(RainMaker), i * 0.1f);
    }
    void RainMaker()
    {
        //페이즈1: 위쪽에서 떨어짐
        GameObject bi = Instantiate(rain, new Vector2(
            Random.Range(-189, 190) * 0.1f, 8.9f), Quaternion.identity);
        int n = Random.Range(0, 10);
        bi.GetComponent<SpriteRenderer>().color
            = new Color(rainR[n], rainG[n], 1); //보기 좋은 푸른색

        if (phase2) //페이즈2: 아래쪽에서도 올라감
        {
            GameObject bi2 = Instantiate(rain, new Vector2(
            Random.Range(-189, 190) * 0.1f, -8.9f), Quaternion.identity);
            int n2 = Random.Range(0, 10);
            bi2.GetComponent<SpriteRenderer>().color
                = new Color(rainR[n2], rainG[n2], 1);
            bi2.GetComponent<Rigidbody2D>().gravityScale = -1;
        }
    }



    void LetBullet() //패턴2. 3px 탄막이 2px 탄막을 뿌리고 2px 탄막이 1px 탄막을 뿌림
    {
        for (int i = 0; i < (phase2 ? 8 : 4); i++)
            Invoke(nameof(ReleaseBullet), phase2 ? i * 0.125f : i * 0.25f);
    }
    void ReleaseBullet()
    {
        Instantiate(pxb3,
            transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
    }



    void Square() //패턴3. 배경을 18분할하여 그중 6 or 12개 랜덤 선정 데미지
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
    }
    void DeathSquare()
    {
        MakeSquare(1, false, 2);

        float px = player.transform.position.x, py = player.transform.position.y;
        bool area = false;
        for (int i = 0; i < thenumberofsquares; i++)
        {
            if (x[i] - 3 < px && px < x[i] + 3 && y[i] - 3 < py && py < y[i] + 3)
                area = true;
        }
        if (area && Player.unbeatableTime <= 0) Player.hurted = true;
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
        GameObject hurt = Instantiate(fadeEffect, transform.position,
            Quaternion.identity);

        SpriteRenderer hsr = hurt.transform.GetComponent<SpriteRenderer>();
        //hsr.flipX = sr.flipX;
        hsr.sortingOrder = 5;
        hsr.sprite = Empty;
        hsr.color = c;

        hurt.GetComponent<Fade>().k = 5;

        if (hide) PlayerKnows();
    }
    void ModifyHp() //hp bar 최신화
    {
        hpText.text = hp.ToString(); //임시

        hpBAR.rectTransform.sizeDelta = new Vector2(hp * 4, 70);
    }
    public void RepeatAD() //AfterDamage() 반복
    {
        Invoke(nameof(AfterDamage), Random.Range(1, 30));
    }
    void AfterDamage() //poison 아이템 - Invoke용
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
            }
        }
    }



} //Boss2 End
