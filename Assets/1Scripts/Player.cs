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

    public Sprite[] players = new Sprite[3];
    //애니메이션 만들기 귀찮아서 임시방편으로 스프라이트 교체용
    //0: 평소 상태(멈춤), 1: 걷기(달리기), 2: 뛰기(점프)

    public int hp;
    float hurtTime = 0; //피격 시 사용할 시간 변수

    public float speed = 10; //달리기 속도

    public float attackCooltime = 0.2f; // 쿨타임
    public float attackSpeed = 0; // 추가 공격속도
    public static Vector2 attackP;
    public Sprite attackSprite;
    bool attackuse;


    bool isWalking = false;
    bool isJumping = false;
    public float jumpPower; //뛰는 힘
    float notJumpTime = 0; //가만히 있는 시간


    bool isDashing = false;
    bool onceDashed = false; //공중에서 대쉬를 이미 했는지
    public float maxDash; //최대 대쉬 가능 시간
    public float dashSpeed; //대쉬 빠르기
    float dashTime = 0; //대쉬하는 시간
    public GameObject dashEffect;
    public Sprite dashSprite;

    bool isAttacking = false;
    SpriteRenderer attacksr;
    //공격 오브젝트의 스프라이트렌더러, flipX 때문에 필요할 듯

    //무기는 획득 시 플레이어의 자손 목록 맨 첫 번째에 들어가도록 합시다!!!

    public int mp;
    public float cooltime = 0;

    bool skilluse; //스킬 시전하는지
    public static Vector2 skillP; //스킬 원 위치
    public Sprite skillSprite;


    void Awake()
    {
        player = this; //이게 나다

        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //플레이어의 0번째 자손인 Attack의 스프라이트렌더러를 끌고 온다.
        //허접한 근접 공격을 만들기 위함.
        attacksr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        attacksr.color = new Color(1, 1, 1, 0);
    } //Awake End


    void Update()
    {
        //점프
        if (Input.GetKeyDown("w") && !isJumping)
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

        //근?접 공격 오브젝트도 뒤집고 위치 변경. 이건 좀 많이 손봐야 한다.
        attacksr.flipX = sr.flipX;
        transform.GetChild(0).transform.localPosition
            = new Vector2(sr.flipX ? -2f : 2f, 0);


        if (!isJumping) //점프 ㄴ
        {
            onceDashed = false;

            if (isWalking) sr.sprite = players[1]; //걷고 있으면 걷는 스프라이트
            else sr.sprite = players[0]; //멈춰 있으면 멈춘 스프라이트
        }


        //마우스 우클릭 대쉬
        if (isJumping && !onceDashed && (Input.GetMouseButtonDown(1)
            || Input.GetKeyDown("k"))) //k는 임시 대쉬 키
        {
            isDashing = true;
            onceDashed = true;
            gameObject.layer = 12; //12PlayerDash
        }


        //공격 (임시로 스킬에서 코드 가져온거라 페이드 들어감, 
        if (attackCooltime > 0) attackCooltime -= Time.deltaTime;
        isAttacking = (Input.GetMouseButton(0) || Input.GetKey("j")) && attackCooltime <= 0; //j는 임시 공격 키
        if (isAttacking) 
        {
            attackCooltime = 0.2f-attackSpeed; // 기본값 소수점 단위에 추가 공격속도 값만큼 빼니 음수로 안가게 주의 
            float x = sr.flipX ? -2 : 2;
            attackP = new Vector2(transform.position.x + x, transform.position.y);
            MakeEffect(attackP, true, attackSprite, -2);
        }
        else attackP = new Vector2(9999, 9999); //저 멀리

        //스킬
        if (cooltime > 0) cooltime -= Time.deltaTime;
        skilluse = cooltime <= 0 && Input.GetKeyDown("s") && mp >= 1;
        if (skilluse) //약한 스킬
        {
            cooltime = 3;
            float x = sr.flipX ? -3 : 3;
            skillP = new Vector2(transform.position.x + x, transform.position.y);
            MakeEffect(skillP, true, skillSprite, -2);
            mp--;
            manager.ChangeHPMP();
        }
        else skillP = new Vector2(9999, 9999); //저 멀리


        if (hurtTime >= 0) Hurt(); //아플 때
        else sr.color = Color.white; //기본

        if (hp <= 0) SceneManager.LoadScene(0); //쉐이망
    } //Update End


    void FixedUpdate()
    {
        //좌우 이동 (등속, 손 떼면 바로 멈춤)
        float h = Input.GetAxisRaw("Horizontal");
        transform.Translate((10+speed) * Time.deltaTime * new Vector2(h, 0)); // 속도 기본 값 10 + speed

        isWalking = h != 0;


        if (isDashing) //대쉬 중이다
        {
            dashTime += Time.deltaTime;
            rigid.AddForce(dashSpeed * (sr.flipX ? Vector2.left : Vector2.right),
                ForceMode2D.Impulse);
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            MakeEffect(transform.position, false, dashSprite, -1);
        }
        if (dashTime >= maxDash) //대쉬 시간
        {
            isDashing = false;
            rigid.velocity = new Vector2(0, 0);
            gameObject.layer = 11; //11Player
            dashTime = 0;
        }

    } //FixedUpdate End


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //플랫폼 닿으면 점프 상태 냅다 해제 (잘 안 먹힘..)
        if (collision.gameObject.CompareTag("Platform")) isJumping = false;

        //아픔
        if (gameObject.layer == 11 && collision.gameObject.CompareTag("Enemy"))
        {
            hp--;
            manager.ChangeHPMP();
            hurtTime = 1;
        }
    }

    //왜인지는 모르겠으나 콜라이더 관련 함수들이 죄다 이상하게 작동한다. 슬프다


    void Hurt() //잠깐 붉은색 되었다가 서서히 회복
    {
        sr.color = new Color(1, 1 - hurtTime, 1 - hurtTime);
        hurtTime -= 4 * Time.deltaTime;
    }


    void MakeEffect(Vector2 p, bool d, Sprite s, int l) //Fade 스크립트가 붙은 오브젝트 생성
    {
        GameObject effect = Instantiate(dashEffect, p, Quaternion.identity);
        if (d) effect.tag = "SkillCircle";
        SpriteRenderer esr = effect.transform.GetComponent<SpriteRenderer>();
        esr.sprite = s;
        esr.flipX = sr.flipX;
        esr.sortingOrder = l;
    }

} //Player End
