using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour //잡몹
{
    public int monsterNum;
    //00spider 01packman 02slime 03??아무튼원거리


    readonly float[,] limitX = //좌우 한계
        { { -48, 48 }, { -55, 55 }, { -30, 30 }, };
    readonly float[,] limitY = //상하 한계
        { { -4, 12 }, { -21, 49 }, { -3, 19 }, };


    /// <summary>
    /// 공통
    /// </summary>

    Rigidbody2D rigid;
    Vector2 tp;
    Vector2 nowPosition;

    public GameObject fadeEffect;

    public Sprite appear;

    bool inAttackArea; //플레이어의 공격 범위 내에 있는지

    int hp; //체력
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

    bool withPlayer = false;
    float withPlayerTime = 0;

    Vector2 firstP; //enemyerror에 대응해 처음 위치로 돌아감


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



    /// <summary>
    /// 원거리 공격형 몬스터 - 03
    /// </summary>

    public GameObject bullet;
    float bulletTime = 0;
    public float bull_T; //탄막 생성 주기




    void Awake()
    {
        tp = transform.position;
        firstP = tp;

        //스폰 효과
        switch (monsterNum)
        {
            case 0: //거미
                GameObject ap0 = Instantiate(fadeEffect,
                    new Vector2(tp.x, tp.y - 0.4f), Quaternion.identity); //약간 아래에
                ap0.transform.GetComponent<SpriteRenderer>().sprite = appear;
                ap0.transform.localScale = 0.8f * Vector2.one; //약간 작게
                break;

            case 1: //팩맨
                GameObject ap1 = Instantiate(fadeEffect, tp, Quaternion.identity);
                ap1.transform.GetComponent<SpriteRenderer>().sprite = appear;
                break;

            case 2: //슬라임
                GameObject ap2 = Instantiate(fadeEffect,
                    new Vector2(tp.x, tp.y + 0.1f), Quaternion.identity); //약간 위에
                ap2.transform.GetComponent<SpriteRenderer>().sprite = appear;
                ap2.transform.localScale = 0.8f * Vector2.one; //약간 작게
                break;

            case 3: //???
                GameObject ap3 = Instantiate(fadeEffect, tp, Quaternion.identity);
                ap3.transform.GetComponent<SpriteRenderer>().sprite = appear;
                ap3.transform.localScale = 0.8f * Vector2.one; //약간 작게
                break;
        }

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

    } //Start End


    void Update()
    {
        Vector2 pp = Player.player.transform.position;

        //플레이어와 적 사이의 거리
        dist = Vector2.Distance(tp, pp);


        switch (monsterNum)
        {
            case 0: //spider
                //플레이어를 향해 이동 방향을 변경한다 (아프면 빨라짐)
                if (tp.x > pp.x) H = hp == maxhp ? -1 : -2;
                else H = hp == maxhp ? 1 : 2;

                Targeting();
            break;

            //packman은 Update 없음

            case 2:
                //플레이어를 향해 이동 방향을 변경한다
                H = tp.x > pp.x ? -1 : 1;

                Targeting();

                //점프를 위한 시간 변수 값 조정
                if (moving && Mathf.Abs(rigid.velocity.y) < 0.1f)
                    lezong -= Time.deltaTime;
            break;

            case 3:
                Targeting();

                //탄막 발사
                if (bulletTime <= 0 && moving)
                {
                    bulletTime = 3;
                    Instantiate(bullet, tp, Quaternion.Euler(
                        0, 0, Mathf.Rad2Deg * Mathf.Atan2(pp.y - tp.y, pp.x - tp.x)));
                }
                else bulletTime -= Time.deltaTime;
            break;

        } //switch






        //가까우면 hp 표시
        C.gameObject.SetActive(dist < 7);


        //피 닳는 시스템
        if (inAttackArea && (Input.GetMouseButtonDown(0)
            || Input.GetKeyDown("j")) && //내가 마우스가 없어서 임시로 설정한 키
            Player.curAttackCooltime >= Player.maxAttackCooltime)
        {
            hp--;
            sr.sprite = Hurt;
            Player.curAttackCooltime = 0;
            ModifyHp();
        }






        //스킬 범위 내에 있음
        if (Mathf.Abs(Player.skillP.y) < 100 &&
            Vector2.Distance(tp, Player.skillP) < 5.5f)
        {
            hp--;
            sr.sprite = Hurt;
            ModifyHp();
        }


        //무기 파생 스킬 범위 내에 있음
        if (Mathf.Abs(Player.wsP.y) < 100)
        {
            switch (Player.weaponNum)
            {
                case 0:
                    bool inX = Mathf.Abs(Player.wsP.x - tp.x) < 7.5f
                        && Mathf.Abs(Player.wsP.y - tp.y) < 1;
                    bool inY = Mathf.Abs(Player.wsP.y - tp.y) < 7.5f
                        && Mathf.Abs(Player.wsP.x - tp.x) < 1;
                    if (inX || inY)
                    {
                        hp -= 2;
                        sr.sprite = Hurt;
                        ModifyHp();
                    }
                break;
            }
        }


        //위치 저장 데미지 입음
        if (Mathf.Abs(Player.posP[0].y) < 100)
        {
            for(int i = 0; i < 2; i++)
            {
                if (Vector2.Distance(tp, Player.posP[i]) < 3)
                {
                    hp--;
                    sr.sprite = Hurt;
                    ModifyHp();
                }
            }
        }



        //쉐이망 - hp, mp 오브 확률적으로 내놓기
        if (hp <= 0)
        {
            int r = Random.Range(0, 10);
            if (r <= 1) Instantiate(hpOrb, tp, Quaternion.identity);

            r = Random.Range(0, 10);
            if (r <= 1) Instantiate(mpOrb, tp, Quaternion.identity);

            Destroy(this.gameObject);
        }




        //플레이어와 오래 닿아있으면 데미지를 준다
        if (withPlayer) withPlayerTime += Time.deltaTime;
        else withPlayerTime = 0;

        if (withPlayerTime > 0.29f)
        {
            withPlayerTime = -0.5f;
            Player.hurted = true;
        }


        //좌표가 이상해지면 돌아오는 로직
        int m = GameManager.mapNum;

        if (tp.x<limitX[m,0] || tp.x > limitX[m, 1]
            || tp.y < limitY[m, 0] || tp.y > limitY[m, 1]) transform.position = firstP;



    } //Update End


    void Targeting() //타겟팅형 몬스터를 위해
    {
        sr.flipX = tp.x < Player.player.transform.position.x - 0.1f;

        //가까우면 이동 시작
        if (dist < noticeDist) moving = true;
    }






    void FixedUpdate()
    {
        tp = transform.position;

        switch (monsterNum)
        {
            case 0: //spider
                if (moving)
                {
                    transform.Translate(H * Time.deltaTime * Vector2.right);

                    //벽에 막혀 안 움직이면 점프
                    if (Mathf.Abs(tp.x - nowPosition.x) < 0.01f)
                    {
                        rigid.AddForce(0.3f * Vector2.up, ForceMode2D.Impulse);

                        if (Mathf.Abs(tp.y - nowPosition.y) < 0.01f)
                            rigid.AddForce((sr.flipX ? 0.1f : -0.1f) * Vector2.right,
                                ForceMode2D.Impulse);
                    }

                    nowPosition = tp;
                }
            break;

            case 1: //packman
                float h = leejong ? 1 : -1;
                transform.Translate(3 * h * Time.deltaTime * Vector2.right);

                Debug.DrawRay(tp, h * transform.right, Color.green, 0.1f);
                RaycastHit2D hit =
                    Physics2D.Raycast(tp, h * transform.right, 1, pf);
                if (hit.transform != null) leejong = !leejong;

                sr.flipX = leejong;

                //nowPosition = tp;
            break;

            case 2: //slime
                //플레이어를 감지한 후에는 계속 이동
                if (moving) transform.Translate(H * Time.deltaTime * Vector2.right);

                if (lezong <= 0)
                {
                    rigid.AddForce(Vector2.up * lezonghan, ForceMode2D.Impulse);
                    lezong = 0.3f;
                }
            break;

            //??아무튼원거리는 FixedUpdate 없음
        }
    } //FixedUpdate End


    private void OnDestroy()
    {
        GameManager.killed++; //죽으면서 킬 수 올리고 감
        if (monsterNum != 1) GameManager.realkilled++;
        withPlayer = false;
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


    void ModifyHp() //hp circle 최신화
    {
        if (hp > 0) C.transform.GetComponent<SpriteRenderer>().sprite = hc[hp - 1];
    }

} //Enemy End
