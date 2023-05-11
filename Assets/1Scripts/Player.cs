using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour //플레이어
{
    public GameManager manager;

    public static Player player;
    //public static으로 설정한 변수는 다른 스크립트에서 맘대로 퍼갈 수 있다

    Rigidbody2D rigid;
    SpriteRenderer sr;
    Animator anim;


    Transform atk; //공격 범위
    Transform bg; //배경
    Transform td; //대쉬 끝 위치
    public Transform po; //저장된 위치 표시 오브젝트
    Transform cs; //슬라이드 가능 알림 화살표


    public Sprite[] players = new Sprite[3]; //지금은 사용 안 하는 중!!!!!!!!
    //애니메이션 만들기 귀찮아서 임시방편으로 스프라이트 교체용
    //0: 평소 상태(멈춤), 1: 걷기(달리기), 2: 뛰기(점프)

    public int hp;
    public int maxhp;

    float hurtTime = 0; //피격 시 사용할 시간 변수
    public static bool hurted;
    int inEnemies = 0;

    public float speed = 0; //달리기 속도

    public static float maxAttackCooltime = 0.2f; // 쿨타임 시간
    public static float curAttackCooltime = 0; // 현재 쿨타임
    public float attackspeed = 0; // 공격 속도
    public static Vector2 attackP;
    public Sprite attackSprite;
    bool attackuse = false;

    SpriteRenderer attacksr;
    //공격 오브젝트의 스프라이트렌더러, flipX 때문에 필요할 듯

    //무기는 획득 시 플레이어의 자손 목록 두 번째에 들어가도록 합시다!!!
    //(첫 번째 자리는 카메라의 차지)


    bool isWalking = false;
    bool isJumping = false;
    public float jumpPower; //뛰는 힘
    float notJumpTime = 0; //가만히 있는 시간


    //bool isDashing = false;
    bool onceDashed = false; //공중에서 대쉬를 이미 했는지
    //public float maxDash; //최대 대쉬 가능 시간
    //public float dashSpeed; //대쉬 빠르기
    //float dashTime = 0; //대쉬하는 시간
    public GameObject dashEffect;
    public Sprite dashSprite;
    public float dashDist; //가능한 대쉬 거리
    public LayerMask lg; //Ground

    Vector2 pos = Vector2.zero; //위치 저장
    bool posSaved = false;


    bool inGround; //끼임
    float inGroundTime;


    public int mp;
    public int maxmp;
    public float cooltime = 0;

    bool skilluse; //스킬 시전하는지
    public static Vector2 skillP; //스킬 원 위치
    public Sprite skillSprite;

    bool isSliding; //플랫폼 내려가는 중인지
    Vector2 slideP;
    public LayerMask lb; //Block

    public static bool getOrb;


    public float respawnCool;


    void Awake()
    {
        player = this; //이게 나다

        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        atk = transform.GetChild(1);
        bg = transform.GetChild(2);
        td = transform.GetChild(3);
        cs = transform.GetChild(4);

        //플레이어의 0번째 자손인 Attack의 스프라이트렌더러를 끌고 온다.
        //허접한 근접 공격을 만들기 위함.
        attacksr = atk.GetComponent<SpriteRenderer>();
        attacksr.color = new Color(1, 1, 1, 0);

        getOrb = false;

        inGround = false;
        inGroundTime = 0;

        respawnCool = 20;

    } //Awake End


    void Update()
    {
        bg.transform.localPosition = -0.1f * transform.position; //배경 이동

        //점프
        if ((Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.Space))
            && !isJumping)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isJumping = true;
            sr.sprite = players[2];
        }

        //연직 방향 속력이 거의 0인 상태가 0.1초 이상이면 점프 중단
        if (Mathf.Abs(rigid.velocity.y) < 0.01f) notJumpTime += Time.deltaTime;
        else notJumpTime = 0;
        isJumping = notJumpTime < 0.1f;

        //방향 전환
        if (Input.GetButton("Horizontal"))
            sr.flipX = Input.GetAxisRaw("Horizontal") == -1;

        bool F = sr.flipX; //하도 많이 써서 정의함

        //근?접 공격 오브젝트도 뒤집고 위치 변경. 이건 좀 많이 손봐야 한다.
        attacksr.flipX = F;
        atk.transform.localPosition = new Vector2(F ? -2f : 2f, 0);


        if (!isJumping) //점프 ㄴ
        {
            onceDashed = false;

            //if (isWalking) sr.sprite = players[1]; //걷고 있으면 걷는 스프라이트
            //else sr.sprite = players[0]; //멈춰 있으면 멈춘 스프라이트
        }


        //이하 내용은 대쉬 관련

        Vector2 d = F ? -1 * transform.right : transform.right; //direction

        RaycastHit2D hit;
        float[] gx = new float[5]; //감지한 Ground의 x좌표

        for (int i = -2; i <= 2; i++)
        {
            Vector2 v = new(transform.position.x, transform.position.y + i * 0.4f);
            hit = Physics2D.Raycast(v, d, dashDist, lg); //레이 맞은 것 저장

            Debug.DrawRay(v, d * dashDist, Color.red, 0.1f); //시각화

            gx[i + 2] = transform.position.x + (F ? -1 : 1)
                * (hit.transform != null ? hit.distance : dashDist);
        }
        float tpx = transform.position.x;

        float m = F ?
                Mathf.Max(gx[0], gx[1], gx[2], gx[3], gx[4]) + 0.7f :
                Mathf.Min(gx[0], gx[1], gx[2], gx[3], gx[4]) - 0.7f;


        //마우스 우클릭 대쉬
        if (!onceDashed && (Input.GetMouseButtonDown(1)
            || Input.GetKeyDown("k"))) //k는 임시 대쉬 키
        {
            //isDashing = true;
            onceDashed = true;
            //gameObject.layer = 12; //12PlayerDash

            transform.position = new Vector2(m, transform.position.y);

            tpx = transform.position.x - tpx;
            for (int i = 0; i < 10; i++) Instantiate(dashEffect,
                new Vector2(transform.position.x - tpx * i * 0.1f, transform.position.y),
                Quaternion.identity);

        } //Dash

        //대쉬 도달 위치 표시
        td.transform.position = new Vector2(m, transform.position.y);


        //위치 저장
        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown("l"))
        {
            if (posSaved)
            {
                transform.position = pos;
                posSaved = false;
            }
            else
            {
                pos = transform.position;
                po.transform.position = pos;
                posSaved = true;
            }
            po.gameObject.SetActive(posSaved);
        }




        //일반공격 쿨타임, 애니메이션
        if (curAttackCooltime <= maxAttackCooltime + 2)
            curAttackCooltime += Time.deltaTime;

        attackuse = (Input.GetMouseButton(0) || Input.GetKey("j"))
            && (curAttackCooltime >= maxAttackCooltime); //j는 임시 공격 키

        if (attackuse)
        {
            float x = F ? -2 : 2;
            attackP = new Vector2(transform.position.x + x, transform.position.y);
            attacksr.color = new Color(1, 1, 1, 1);
        }
        else attacksr.color = new Color(1, 1, 1, 0);


        //스킬
        if (cooltime > 0) cooltime -= Time.deltaTime;

        skilluse = cooltime <= 0 && Input.GetKeyDown("z") && mp >= 1;

        if (skilluse) //약한 스킬
        {
            cooltime = 3;
            float x = F ? -6 : 6;
            skillP = new Vector2(transform.position.x + x, transform.position.y);
            MakeEffect(skillP, skillSprite, -2);
            mp--;
            manager.ChangeHPMP();
        }
        else skillP = new Vector2(9999, 9999); //저 멀리


        if (hurted)
        {
            hp--;
            manager.ChangeHPMP();
            hurtTime = 1;
            hurted = false;
        }

        if (inEnemies == 0) hurted = false;

        if (hurtTime >= 0) Hurt(); //아플 때
        else sr.color = Color.white; //기본

        if (hp > maxhp) hp = maxhp;
        if (hp <= 0) SceneManager.LoadScene(0); //쉐이망



        //내려갈 수 있는가?
        Debug.DrawRay(transform.position, -1 * transform.up, Color.blue, 0.1f);
        RaycastHit2D B; //block
        B = Physics2D.Raycast(transform.position, -1 * transform.up, 1, lb);
        RaycastHit2D G; //ground
        G = Physics2D.Raycast(transform.position, -1 * transform.up, 1, lg);
        cs.gameObject.SetActive(B.transform != null && G.transform == null);

        //플랫폼 내려가기
        if (Input.GetKeyDown("s"))
        {
            isSliding = true;
            this.gameObject.layer = 13; //13PlayerSlide
            slideP = transform.position; //원래 위치 저장
        }

        if (slideP.y - transform.position.y > 2) SlideCheck();




        if (getOrb)
        {
            manager.ChangeHPMP();
            getOrb = false;
        }
        if (mp > maxmp) mp = maxmp;


        if (inGround)
        {
            inGroundTime += Time.deltaTime;

            if (inGroundTime > 1)
            {
                //transform.position = outGroundP;
                inGround = false;
            }
        }
        else inGroundTime = 0;

    } //Update End


    void FixedUpdate()
    {
        //좌우 이동 (등속, 손 떼면 바로 멈춤)
        float h = Input.GetAxisRaw("Horizontal");
        // 속도 기본 값 10 + speed
        transform.Translate((10+speed) * Time.deltaTime * new Vector2(h, 0));

        isWalking = h != 0;

        /*
        if (dashTime >= maxDash) //대쉬 시간
        {
            isDashing = false;
            rigid.velocity = new Vector2(0, 0);
            gameObject.layer = 11; //11Player
            dashTime = 0;
        }

        if (isDashing) //대쉬 중이다
        {
            dashTime += Time.deltaTime;
            rigid.velocity = new Vector2(dashSpeed * (sr.flipX ? -1.5f : 1.5f), 0);
            MakeEffect(transform.position, dashSprite, -1);
        }
        */

        //if (onceDashed) //대쉬 후 이펙트는 계속 만들어줌
            //Instantiate(dashEffect, transform.position, Quaternion.identity);

    } //FixedUpdate End


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //플랫폼 닿으면 점프 상태 냅다 해제 (잘 안 먹힘..)
        if (collision.gameObject.CompareTag("Platform")) isJumping = false;

        if (collision.gameObject.CompareTag("Enemy")) inEnemies++;

        SlideCheck();
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) inEnemies--;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Respawn"))
            SceneManager.LoadScene(0); //낙사

        if (other.gameObject.layer == 9) //9Ground
        {
            //outGroundP = new Vector2(transform.position.x - rigid.velocity.x,
            //    transform.position.y - rigid.velocity.y);
            rigid.AddForce(-2 * new Vector2(rigid.velocity.x, rigid.velocity.y));
            inGround = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8) //8Block
            SlideCheck();

        if (other.gameObject.layer == 9) //9Ground
            inGround = false;
    }


    void SlideCheck() //isSliding 끄기
    {
        if (isSliding)
        {
            isSliding = false;
            this.gameObject.layer = 11; //11Player
        }
    }


    void Hurt() //잠깐 붉은색 되었다가 서서히 회복
    {
        sr.color = new Color(1, 1 - hurtTime, 1 - hurtTime);
        hurtTime -= 4 * Time.deltaTime;
    }


    //position, sprite, layer
    void MakeEffect(Vector2 p, Sprite s, int l) //Fade 스크립트가 붙은 오브젝트 생성
    {
        GameObject effect = Instantiate(dashEffect, p, Quaternion.identity);
        SpriteRenderer esr = effect.transform.GetComponent<SpriteRenderer>();
        esr.sprite = s;
        esr.flipX = sr.flipX;
        esr.sortingOrder = l;
    }


} //Player End
