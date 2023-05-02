using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour //플레이어
{
    public static Player player;
    //public static으로 설정한 변수는 다른 스크립트에서 맘대로 퍼갈 수 있다

    Rigidbody2D rigid;
    SpriteRenderer sr;
    Animator anim;

    public Sprite[] players = new Sprite[3];
    //애니메이션 만들기 귀찮아서 임시방편으로 스프라이트 교체용
    //0: 평소 상태(멈춤), 1: 걷기(달리기), 2: 뛰기(점프)

    bool isWalking = false;
    bool isJumping = false;
    public float jumpPower;
    float jumpTime = 0;

    bool isAttacking = false;
    SpriteRenderer attacksr;
    //공격 오브젝트의 스프라이트렌더러긴 한데 나중에 개편할 때 삭제하자


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
    }


    void Update()
    {
        //점프
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isJumping = true;
            sr.sprite = players[2];
        }

        //연직 방향 속력이 0인 상태가 0.1초 이상이면 점프 중단
        //왜인지는 모르겠으나 콜라이더 충돌이 잘 안 먹혀서 임시로 만들었음
        if (rigid.velocity.y == 0) jumpTime += Time.deltaTime;
        else jumpTime = 0;
        isJumping = jumpTime < 0.1f;

        //방향 전환
        if (Input.GetButton("Horizontal"))
            sr.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //근?접 공격 오브젝트도 뒤집고 위치 변경. 이건 좀 많이 손봐야 한다.
        attacksr.flipX = sr.flipX;
        float X = sr.flipX ? -2f : 2f;
        transform.GetChild(0).transform.localPosition = new Vector2(X, 0);

        if (!isJumping) //점프 ㄴ
        {
            if (isWalking) sr.sprite = players[1]; //걷고 있으면 걷는 스프라이트
            else sr.sprite = players[0]; //멈춰 있으면 멈춘 스프라이트
        }

        //아래 화살표 눌러 공격인데 필히 키 교체가 필요
        isAttacking = Input.GetKey(KeyCode.DownArrow);
        if (isAttacking) attacksr.color = new Color(1, 1, 1, 1); //불투명함
        else attacksr.color = new Color(1, 1, 1, 0); //투명함
    }


    void FixedUpdate()
    {
        //좌우 이동 (등속, 손 떼면 바로 멈춤)
        float h = Input.GetAxisRaw("Horizontal");
        transform.Translate(10 * Time.deltaTime * new Vector2(h, 0));

        isWalking = h != 0;

        /*
        //플랫폼 착지
        if (rigid.velocity.y < 0)
        {
            //에디터상에서만 Ray를 그려주는 함수
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            //Ray에 닿은 오브젝트
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down,
                2, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 2.5f)
                {
                    isJumping = false;
                    sr.sprite = players[0];
                }
            }
        }
        */ //레이캐스트 저리 가
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform")) isJumping = false;
        //플랫폼 닿으면 점프 상태 냅다 해제

        if (collision.gameObject.CompareTag("Enemy")) SceneManager.LoadScene(0);
        //쉐이망
    }

} //Player End
