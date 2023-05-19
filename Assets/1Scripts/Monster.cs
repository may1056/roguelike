using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour //잡몹
{
    public int monsterNum;
    //00spider 01packman 02slime 03??아무튼원거리 04??아무튼얼음빔 05kingslime


    readonly float[,] limitX = //좌우 한계
        { { -48, 48 }, { -55, 55 }, { -30, 30 }, { -4, 50 } };
    readonly float[,] limitY = //상하 한계
        { { -4, 12 }, { -21, 49 }, { -3, 19 }, { -4, 30 } };


    /// <summary>
    /// 공통
    /// </summary>

    Rigidbody2D rigid;
    Vector2 tp;
    Vector2 nowPosition;

    public GameObject fadeEffect;

    public Sprite doubleCircle;

    bool inAttackArea; //플레이어의 공격 범위 내에 있는지

    public int hp; //체력
    public int maxhp; //최대 체력

    Transform C;
    public Sprite[] hc = new Sprite[5]; //hp circles

    Color darkpurple = new(0.3215f, 0.0588f, 0.6705f); //00
    Color darkred = new(0.7686f, 0.0862f, 0.0078f); //01, 02
    Color mol_lu = new(0.1f, 0.1f, 0.1f); //03

    SpriteRenderer sr;
    public Sprite Hurt;

    public GameObject hpOrb;
    public GameObject mpOrb;
    public GameObject coinOrb;

    bool withPlayer = false;
    float withPlayerTime = 0;

    Vector2 firstP; //enemyerror에 대응해 처음 위치로 돌아감


    public float pollution = 0; //오염 정도
    public bool polluted = false; //오염되었는지
    Transform pol;





    /// <summary>
    /// 플레이어 타겟팅형 몬스터 - 00, 02
    /// </summary>

    float dist; //플레이어와의 거리
    public float noticeDist; //플레이어 인식 가능 범위

    bool moving = false;
    float H; //이동 상?수



    /// <summary>
    /// 함정형 몬스터 - 01
    /// </summary>

    bool leejong;
    public LayerMask pf; //platform



    /// <summary>
    /// 점프형 몬스터 - 02
    /// </summary>

    float lezong = 0.3f;//점프 시간 카운트
    public float lezonghan; //점프파워

    bool k; //king
    public GameObject littleslime;



    /// <summary>
    /// 원거리 공격형 몬스터 - 03
    /// </summary>

    public GameObject bullet;
    float bulletTime = 0;
    public float bull_T; //탄막 생성 주기

    Quaternion AngleSelected;




    void Awake()
    {
        tp = transform.position;
        firstP = tp;

        DecideEffect(Color.black);

        gameObject.SetActive(false); //일단 감춤

    } //Awake End


    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        nowPosition = new Vector2(999,999);

        hp = maxhp;
        C = transform.GetChild(0);
        ModifyHp();

        sr = GetComponent<SpriteRenderer>();

        inAttackArea = false;

        k = monsterNum == 5;

        pol = transform.GetChild(1);
        switch (monsterNum)
        {
            case 0:
                pol.localPosition = new Vector2(0, -0.3f);
                pol.localScale = new Vector2(0.2f, 0.2f); break;
            case 1:
                pol.localPosition = Vector2.zero;
                pol.localScale = new Vector2(0.3f, 0.3f); break;
            case 2:
                pol.localPosition = Vector2.zero;
                pol.localScale = new Vector2(0.2f, 0.2f); break;
            case 3:
                pol.localPosition = Vector2.zero;
                pol.localScale = new Vector2(0.25f, 0.25f); break;
            case 4:
                pol.localPosition = Vector2.zero;
                pol.localScale = new Vector2(0.3f, 0.3f); break;
            case 5:
                pol.localPosition = Vector2.zero;
                pol.localScale = new Vector2(0.2f, 0.2f); break;
        }

    } //Start End


    void Update()
    {
        if (GameManager.mapouterror)
        {
            transform.position = firstP;
            moving = false;
        }

        Vector2 pp = Player.player.transform.position;

        //플레이어와 적 사이의 거리
        dist = Vector2.Distance(tp, pp);


        switch (monsterNum)
        {
            case 0: //spider
                //플레이어를 향해 이동 방향을 변경한다 (아프면 빨라짐)
                if (tp.x > pp.x) H = hp == maxhp ? -2 : -4;
                else H = hp == maxhp ? 2 : 4;

                Targeting();
                break;

            //packman은 Update 없음

            case 2: //slime
            case 5: //kingslime
                //플레이어를 향해 이동 방향을 변경한다
                H = tp.x > pp.x ? -3 : 3;
                if (k) H *= 2;

                Targeting();

                //점프를 위한 시간 변수 값 조정
                if (moving && Mathf.Abs(rigid.velocity.y) < 0.1f)
                    lezong -= Time.deltaTime;
                break;

            case 3: //??amuteunweongeori
                Targeting();

                //탄막 발사
                if (bulletTime <= 0 && moving)
                {
                    bulletTime = 3;
                    AngleSelected = Quaternion.Euler(0, 0, Mathf.Rad2Deg *
                        Mathf.Atan2(Player.player.transform.position.y - tp.y,
                        Player.player.transform.position.x - tp.x));
                    ShootBullet();
                }
                else bulletTime -= Time.deltaTime;
                break;

            case 4: //??amuteuneoleumbeam
                H = tp.x > pp.x ? -1 : 1;
                Targeting();

                //탄막 발사
                if (bulletTime <= 0 && moving)
                {
                    bulletTime = 6;
                    AngleSelected = Quaternion.Euler(0, 0, Mathf.Rad2Deg *
                        Mathf.Atan2(Player.player.transform.position.y - tp.y,
                        Player.player.transform.position.x - tp.x));
                    for (int i = 0; i < 40; i++) Invoke(nameof(ShootBullet), i * 0.025f);
                }
                else if (moving)
                {
                    bulletTime -= Time.deltaTime;
                }
                break;

        } //switch






        //가깝거나 딜을 입으면 hp 표시
        C.gameObject.SetActive(dist < 5 || hp < maxhp);


        //피 닳는 시스템
        if (inAttackArea && (Input.GetMouseButtonDown(0)
            || Input.GetKeyDown("j")) && //내가 마우스가 없어서 임시로 설정한 키
            PlayerAttack.curAttackCooltime >= PlayerAttack.maxAttackCooltime)
        {
            if (Player.player.berserker && Player.player.hp < 3) hp -= 4;
            else hp--;
            sr.sprite = Hurt;
            PlayerAttack.curAttackCooltime = 0;
            ModifyHp();
        }






        //스킬 범위 내에 있음
        if (Mathf.Abs(PlayerAttack.skillP.y) < 100 &&
            Vector2.Distance(tp, PlayerAttack.skillP) < 5.5f)
        {
            if (Player.player.berserker && Player.player.hp < 3) hp -= 4;
            else hp--;
            sr.sprite = Hurt;
            ModifyHp();
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
                        if (Player.player.berserker && Player.player.hp < 3) hp -= 8;
                        else hp -= 2;
                        sr.sprite = Hurt;
                        ModifyHp();
                    }
                break;
            }
        }

        /*
        //위치 저장 데미지 입음
        if (Mathf.Abs(Player.posP[0].y) < 100)
        {
            for(int i = 0; i < 2; i++)
            {
                if (Vector2.Distance(tp, Player.posP[i]) < 3)
                {
                    if (Player.player.berserker && Player.player.hp <= 2) hp -= 2;
                    else hp--;
                    sr.sprite = Hurt;
                    ModifyHp();
                }
            }
        }
        */

        //자동 공격 오염
        polluted = pollution == 1;

        if (polluted)
        {
            sr.sprite = Hurt;
            ModifyHp();
            Invoke(nameof(RemovePollution), 1);
        }
        else if (pollution > 0 && !polluted) pollution -= 0.3f * Time.deltaTime;

        transform.GetChild(1).GetComponent<SpriteRenderer>().color
            = new Color(0.6f, 0.4f, 1, pollution);



        //쉐이망 - hp, mp 오브 확률적으로 내놓기
        if (hp <= 0)
        {
            int r = Random.Range(0, 10);
            if (r < 1) Instantiate(hpOrb, tp, Quaternion.identity);

            r = Random.Range(0, 10);
            if (r < 2) Instantiate(mpOrb, tp, Quaternion.identity);

            r = Random.Range(0, 10);
            if (r < 4) Instantiate(coinOrb, tp, Quaternion.identity);

            GameManager.killed++; //죽으면서 킬 수 올리고 감
            GameManager.realkilled++;
            withPlayer = false;

            if (k)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    GameObject s = Instantiate(littleslime,
                        new Vector2(tp.x + i * 0.5f, tp.y), Quaternion.identity);
                    s.transform.SetParent(transform.parent);
                    s.SetActive(true);
                }
            }

            DecideEffect(Color.white);

            Destroy(this.gameObject);
        }




        //플레이어와 오래 닿아있으면 데미지를 준다
        if (withPlayer) withPlayerTime += Time.deltaTime;
        else withPlayerTime = 0;

        if (withPlayerTime > 0.29f)
        {
            withPlayerTime = -0.5f;
            if (monsterNum < 3 && Player.unbeatableTime <= 0) Player.hurted = true;
        }


        //좌표가 이상해지면 돌아오는 로직
        int m = GameManager.mapNum;

        if (tp.x<limitX[m,0] || tp.x > limitX[m, 1]
            || tp.y < limitY[m, 0] || tp.y > limitY[m, 1]) transform.position = firstP;



    } //Update End


    void Targeting() //타겟팅형 몬스터를 위해
    {
        sr.flipX = tp.x < Player.player.transform.position.x - 0.3f;

        //가까우면 이동 시작
        if (dist < noticeDist)
        {
            moving = true;
            if (transform.childCount == 3) //느낌표 보이기 (있으면)
            {
                transform.GetChild(2).gameObject.SetActive(true);
                Invoke(nameof(Noticed), 2); //2초 뒤 부숴
            }
        }
    }
    void Noticed() //느낌표 파괴 (있으면)
    {
        if (transform.childCount == 3)
            Destroy(transform.GetChild(2).gameObject);
    }


    void ShootBullet()
    {
        Instantiate(bullet, tp, AngleSelected);
    }


    public void RemovePollution() //오염 제거
    {
        if (polluted) pollution = 0;
        CancelInvoke(nameof(RemovePollution));
    }



    void FixedUpdate()
    {
        tp = transform.position;

        switch (monsterNum)
        {
            case 0: //spider
                if (moving)
                {
                    transform.Translate(H * (1 - pollution) *
                        Time.deltaTime * Vector2.right);

                    //벽에 막혀 안 움직이면 점프
                    if (Mathf.Abs(tp.x - nowPosition.x) < 0.01f)
                    {
                        rigid.AddForce(
                            0.3f * (1 - pollution) * Vector2.up, ForceMode2D.Impulse);

                        if (Mathf.Abs(tp.y - nowPosition.y) < 0.01f)
                            rigid.AddForce((sr.flipX ? 0.1f : -0.1f) * (1 - pollution) *
                                Vector2.right, ForceMode2D.Impulse);
                    }

                    nowPosition = tp;
                }
            break;

            case 1: //packman
                float h = leejong ? 1 : -1;
                transform.Translate(5 * h * (1 - pollution) *
                    Time.deltaTime * Vector2.right);

                Debug.DrawRay(tp, h * transform.right, Color.green, 0.1f);

                RaycastHit2D hit1 = Physics2D.Raycast(
                    new Vector2(tp.x, tp.y - 0.4f), h * transform.right, 1, pf);
                RaycastHit2D hit2 = Physics2D.Raycast(
                    tp, h * transform.right, 1, pf);
                RaycastHit2D hit3 = Physics2D.Raycast(
                    new Vector2(tp.x, tp.y + 0.4f), h * transform.right, 1, pf);

                if (hit1.transform != null || hit2.transform != null
                    || hit3.transform != null) leejong = !leejong;

                sr.flipX = leejong;

                //nowPosition = tp;
            break;

            case 2: //slime
            case 5: //kingslime
                //플레이어를 감지한 후에는 계속 이동
                if (moving) transform.Translate(H * (1 - pollution) *
                    Time.deltaTime * Vector2.right);

                if (lezong <= 0)
                {
                    if (monsterNum == 5 && Random.Range(0, 2) == 1)
                    {
                        //킹슬라임은 점프 때마다 확률적으로 작은 슬라임 생성
                        GameManager.realkilled--;
                        GameObject s = Instantiate(littleslime, tp, Quaternion.identity);
                        s.transform.SetParent(transform.parent);
                        s.SetActive(true);
                    }
                    rigid.AddForce(Vector2.up * (1 - pollution) * lezonghan,
                        ForceMode2D.Impulse);
                    lezong = 0.3f;
                }
            break;

            //??아무튼원거리는 FixedUpdate 없음

            case 4: //??아무튼얼음빔
                if (moving && bulletTime < 5 && bulletTime > 1)
                    transform.Translate(H * Time.deltaTime * Vector2.right);
                break;
        }

    } //FixedUpdate End




    void DecideEffect(Color c)
    {
        switch (monsterNum)
        {
            case 0: //거미
                MakeEffect(new Vector2(tp.x, tp.y - 0.4f), c, 0.8f);
                break;

            case 1: //팩맨
                MakeEffect(tp, c, 1);
                break;

            case 2: //슬라임
                MakeEffect(new Vector2(tp.x, tp.y + 0.1f), c, 0.8f);
                break;

            case 3: //???
                MakeEffect(tp, c, 0.8f);
                break;

            case 4: //???
                MakeEffect(tp, c, 0.9f);
                break;

            case 5: //킹슬라임
                MakeEffect(new Vector2(tp.x, tp.y + 0.1f), c, 1.5f);
                break;
        }
    }
    void MakeEffect(Vector2 v, Color c, float sc)
    {
        GameObject eff = Instantiate(fadeEffect, v, Quaternion.identity);
        SpriteRenderer effsr = eff.GetComponent<SpriteRenderer>();
        effsr.sprite = doubleCircle;
        effsr.color = c;
        eff.transform.localScale = sc * Vector2.one;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        int l = collision.gameObject.layer;
        if (l == 11 || l == 13) withPlayer = true;
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        int l = collision.gameObject.layer;
        if (l >= 11 && l <= 13) withPlayer = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            inAttackArea = true; //들어간다
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            inAttackArea = false; //빠져나온다

        int l = collision.gameObject.layer;
        if (l >= 11 && l <= 13) withPlayer = false;
    }


    public void ModifyHp() //hp circle 최신화
    {
        if (hp > 0 && hp <= 5)
        {
            C.transform.GetComponent<SpriteRenderer>().sprite = hc[hp - 1];
            if (C.childCount == 1) Destroy(C.GetChild(0).gameObject);
        }
        else if (hp > 5 && hp <= 10)
        {
            C.transform.GetComponent<SpriteRenderer>().sprite = hc[hp - 6];
            C.transform.GetChild(0).
                transform.GetComponent<SpriteRenderer>().sprite = hc[4];
        }
    }

} //Enemy End
