using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour //잡몹
{
    public int monsterNum;
    //00spider 01packman 02slime 03??아무튼원거리

    /// <summary>
    /// 공통
    /// </summary>

    Rigidbody2D rigid;
    Vector2 nowPosition;

    bool inAttackArea; //플레이어의 공격 범위 내에 있는지

    int hp; //체력
    public int maxhp; //최대 체력

    SpriteRenderer sr;
    public Sprite Hurt;

    public GameObject hpOrb;
    public GameObject mpOrb;

    bool withPlayer = false;
    float withPlayerTime = 0;


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



    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        nowPosition = new Vector2(999,999);

        hp = maxhp;

        sr = GetComponent<SpriteRenderer>();

        inAttackArea = false;
    } //Start End


    void Update()
    {
        switch (monsterNum)
        {
            case 0: //spider
                //플레이어를 향해 이동 방향을 변경한다 (아프면 빨라짐)
                if (transform.position.x > Player.player.transform.position.x)
                    H = hp == maxhp ? -1 : -2;
                else H = hp == maxhp ? 1 : 2;

                Targeting();
            break;

            //packman은 Update 없음

            case 2:
                //플레이어를 향해 이동 방향을 변경한다
                H = transform.position.x > Player.player.transform.position.x ? -1 : 1;

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
                    Instantiate(bullet, transform.position,
                        Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(
                            Player.player.transform.position.y - transform.position.y,
                            Player.player.transform.position.x - transform.position.x)));
                }
                else bulletTime -= Time.deltaTime;
            break;

        } //switch


        //피 닳는 시스템
        if (inAttackArea && (Input.GetMouseButtonDown(0)
            || Input.GetKeyDown("j")) && //내가 마우스가 없어서 임시로 설정한 키
            Player.curAttackCooltime >= Player.maxAttackCooltime)
        {
            hp--;
            sr.sprite = Hurt;
            Player.curAttackCooltime = 0;
        }

        //스킬 범위 내에 있음
        if (Mathf.Abs(Player.skillP.y) < 20 &&
            Vector2.Distance(transform.position, Player.skillP) < 7f)
        {
            hp--;
            sr.sprite = Hurt;
        }

        //쉐이망 - hp, mp 오브 확률적으로 내놓기
        if (hp <= 0)
        {
            int r = Random.Range(0, 10);
            if (r == 1) Instantiate(hpOrb, transform.position, Quaternion.identity);

            r = Random.Range(0, 10);
            if (r == 1) Instantiate(mpOrb, transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }

        if (withPlayer) withPlayerTime += Time.deltaTime;
        else withPlayerTime = 0;

        if (withPlayerTime > 0.29f)
        {
            withPlayerTime = -0.5f;
            Player.hurted = true;
        }

    } //Update End


    void Targeting() //타겟팅형 몬스터를 위해
    {
        sr.flipX = transform.position.x <
                    Player.player.transform.position.x - 0.1f;

        //플레이어와 적 사이의 거리
        dist = Vector2.Distance(transform.position,
            Player.player.transform.position);

        //가까우면 이동 시작
        if (dist < noticeDist) moving = true;
    }


    void FixedUpdate()
    {
        switch (monsterNum)
        {
            case 0: //spider
                if (moving)
                {
                    transform.Translate(H * Time.deltaTime * Vector2.right);

                    //벽에 막혀 안 움직이면 점프
                    if (Mathf.Abs(transform.position.x - nowPosition.x) < 0.01f)
                    {
                        rigid.AddForce(0.3f * Vector2.up, ForceMode2D.Impulse);

                        if (Mathf.Abs(transform.position.y - nowPosition.y) < 0.01f)
                            rigid.AddForce((sr.flipX ? 0.1f : -0.1f) * Vector2.right,
                                ForceMode2D.Impulse);
                    }

                    nowPosition = transform.position;
                }
            break;

            case 1: //packman
                float h = leejong ? 3 : -3;
                transform.Translate(h * Time.deltaTime * Vector2.right);

                if (Mathf.Abs(transform.position.x - nowPosition.x) < 0.01f)
                    leejong = !leejong;

                sr.flipX = leejong;

                nowPosition = transform.position;
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

} //Enemy End
