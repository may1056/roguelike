using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour //플레이어
{
    public GameManager manager;

    public static Player player;
    //public static으로 설정한 변수는 다른 스크립트에서 맘대로 퍼갈 수 있다

    Rigidbody2D rigid;
    SpriteRenderer sr;
    Animator anim;


    public static int itemNum;
    public static int weaponNum;
    //0채찍,


    Transform bg; //배경
    Transform td; //대쉬 끝 위치
    public Transform po; //저장된 위치 표시 오브젝트
    Transform cs; //슬라이드 가능 알림 화살표


    public GameObject fadeEffect;


    public Sprite[] players = new Sprite[3]; //지금은 사용 안 하는 중!!!!!!!!
    //애니메이션 만들기 귀찮아서 임시방편으로 스프라이트 교체용
    //0: 평소 상태(멈춤), 1: 걷기(달리기), 2: 뛰기(점프)


    public int hp;
    public int maxhp;

    public int shield;
    public int maxshield;
    bool protect;


    float hurtTime = 0; //피격 시 사용할 시간 변수
    public static bool hurted;
    int inEnemies = 0;

    public Image hurtImage;

    public float dontBehaveTime = 0;


    public float speed = 0; //달리기 속도


    //bool isWalking = false;
    bool isJumping = false;
    public float jumpPower; //뛰는 힘
    float notJumpTime = 0; //가만히 있는 시간



    bool onceDashed = false; //공중에서 대쉬를 이미 했는지
    public Sprite dashSprite;
    public float dashDist; //가능한 대쉬 거리
    public LayerMask lg; //Ground


    //save position
    public Text posText;
    float posCool = 0;
    Vector2 pos = Vector2.zero;
    bool posSaved = false;
    public Sprite posSprite;
    float postime = 0;
    public static Vector2[] posP =
        { new Vector2(9999, 9999) , new Vector2(9999, 9999) };



    bool isSliding; //플랫폼 내려가는 중인지
    Vector2 slideP;
    public LayerMask lb; //Block

    public static bool getOrb; //오브 먹었는지


    SpriteRenderer bgsr; //배경 스프라이트렌더러
    float bgtime = 0;
    Color cloud = new(0.7f, 0.8f, 0.9f); //구름 약간 어두움


    public bool F;




    void Awake()
    {
        player = this; //이게 나다

        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        weaponNum = 0;


        bg = transform.GetChild(2);
        td = transform.GetChild(3);
        cs = transform.GetChild(4);


        getOrb = false;

        bgsr = bg.GetComponent<SpriteRenderer>();

    } //Awake End




    void Update()
    {
        if (GameManager.mapouterror) transform.position = Vector2.zero;

        Vector2 tp = transform.position;



        //ㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍ

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






        //ㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎ

        //방향 전환
        if (Input.GetButton("Horizontal"))
            sr.flipX = Input.GetAxisRaw("Horizontal") == -1;

        F = sr.flipX; //하도 많이 써서 정의함




        if (!isJumping) //점프 ㄴ
        {
            onceDashed = false;

            //if (isWalking) sr.sprite = players[1]; //걷고 있으면 걷는 스프라이트
            //else sr.sprite = players[0]; //멈춰 있으면 멈춘 스프라이트
        }







        //ㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅ

        //이하 내용은 대쉬 관련

        Vector2 d = F ? -1 * transform.right : transform.right; //direction

        RaycastHit2D hit;
        float[] gx = new float[5]; //감지한 Ground의 x좌표

        for (int i = -2; i <= 2; i++)
        {
            Vector2 v = new(tp.x, tp.y + i * 0.4f);
            hit = Physics2D.Raycast(v, d, dashDist, lg); //레이 맞은 것 저장

            Debug.DrawRay(v, d * dashDist, Color.red, 0.1f); //시각화

            gx[i + 2] = tp.x + (F ? -1 : 1)
                * (hit.transform != null ? hit.distance : dashDist);
        }
        float tpx = tp.x;

        float m = F ?
                Mathf.Max(gx[0], gx[1], gx[2], gx[3], gx[4]) + 0.7f :
                Mathf.Min(gx[0], gx[1], gx[2], gx[3], gx[4]) - 0.7f;


        //마우스 우클릭 대쉬
        if (!onceDashed && (Input.GetMouseButtonDown(1)
            || Input.GetKeyDown("k"))) //k는 임시 대쉬 키
        {
            onceDashed = true;

            transform.position = new Vector2(m, tp.y);
            tp = transform.position;

            tpx = tp.x - tpx;
            for (int i = 0; i < 10; i++) Instantiate(fadeEffect,
                new Vector2(tp.x - tpx * i * 0.1f, tp.y),
                Quaternion.identity);

            dontBehaveTime = 0;
        }

        //대쉬 도달 위치 표시
        td.transform.position = new Vector2(m, tp.y);







        //ㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊ

        if (posCool <= 0) //가능
        {
            //저장 위치가 없으면 바로 저장하기
            if ((Input.GetMouseButtonDown(2) || Input.GetKeyDown("l"))
                && !posSaved) SavePos();

            //위치가 이미 저장되어 있다면 길게 눌러 새로 저장
            if (Input.GetMouseButton(2) || Input.GetKey("l"))
            {
                postime += Time.deltaTime;
                if (postime >= 1 && postime <= 10) SavePos();
            }

            //빠르게 눌러 돌아가기
            if (Input.GetMouseButtonUp(2) || Input.GetKeyUp("l"))
            {
                if (posSaved && postime < 1)
                {
                    DamagePos(0, tp);
                    DamagePos(1, pos);
                    transform.position = pos;
                    posSaved = false;
                    posCool = 10;
                    dontBehaveTime = 0;
                }
                postime = 0;
            }
            else //발동 타이밍 제외 posP는 딴 데 가 있다
            {
                posP[0] = new Vector2(9999, 9999);
                posP[1] = posP[0];
            }

            //POS 온오프
            po.gameObject.SetActive(posSaved);
        }
        else //불가능
        {
            posCool -= Time.deltaTime;

            posP[0] = new Vector2(9999, 9999);
            posP[1] = posP[0];
        }

        posText.text = posCool.ToString("N1");







        //ㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹㄴㄹ

        //내려갈 수 있는가?
        Debug.DrawRay(tp, -1 * transform.up, Color.blue, 0.1f);

        RaycastHit2D B1, B2; //block
        B1 = Physics2D.Raycast(
            new Vector2(tp.x - 0.5f, tp.y), -1 * transform.up, 1, lb);
        B2 = Physics2D.Raycast(
            new Vector2(tp.x + 0.5f, tp.y), -1 * transform.up, 1, lb);

        RaycastHit2D G1, G2; //ground
        G1 = Physics2D.Raycast(
            new Vector2(tp.x - 0.5f, tp.y), -1 * transform.up, 1, lg);
        G2 = Physics2D.Raycast(
            new Vector2(tp.x + 0.5f, tp.y), -1 * transform.up, 1, lg);

        bool s = (B1.transform != null || B2.transform != null)
            && G1.transform == null && G2.transform == null;
        cs.gameObject.SetActive(s);

        //플랫폼 내려가기
        if (Input.GetKeyDown("s"))
        {
            isSliding = true;
            this.gameObject.layer = 13; //13PlayerSlide
            slideP = transform.position; //원래 위치 저장
        }

        if (slideP.y - transform.position.y > 2) SlideCheck();







        //ㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍ

        //공격당함
        if (hurted)
        {
            if (shield >= 1)
            {
                shield--;
                protect = true;
            }
            else
            {
                hp--;
                protect = false;
            }
            manager.ChangeHPMP();
            hurtTime = 1;
            hurted = false;
            dontBehaveTime = 0;
        }

        if (inEnemies == 0) hurted = false;

        if (hurtTime >= 0) Hurt(); //아플 때
        else sr.color = Color.white; //기본

        if (hp > maxhp) hp = maxhp;
        if (hp <= 0) SceneManager.LoadScene(1); //쉐이망




        //ㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷ

        //쉴드 재충전
        dontBehaveTime += Time.deltaTime;
        if (dontBehaveTime > 3 && shield < maxshield)
        {
            shield++;
            dontBehaveTime = 0;
            manager.ChangeHPMP();
        }





        //ㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂㅇㅂ

        if (getOrb)
        {
            manager.ChangeHPMP();
            getOrb = false;
        }

        //if (mp > maxmp) mp = maxmp;






        //ㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱㅂㄱ

        bg.transform.localPosition = -0.1f * tp; //배경 이동

        if (bgtime > 0)
        {
            //배경 흰색으로 갔다가 돌아온다 - 다른 배경에도 일반화하려고 복잡하게 썼음
            bgsr.color = new Color(cloud.r + bgtime * (1 - cloud.r),
                cloud.g + bgtime * (1 - cloud.g), cloud.b + bgtime * (1 - cloud.b));
            bgtime -= Time.deltaTime;
        }

    } //Update End




    void FixedUpdate()
    {
        //좌우 이동 (등속, 손 떼면 바로 멈춤)
        float h = Input.GetAxisRaw("Horizontal");
        // 속도 기본 값 10 + speed
        transform.Translate((10+speed) * Time.deltaTime * new Vector2(h, 0));

        //isWalking = h != 0;

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
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8) //8Block
            SlideCheck();
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
        //sr.color = new Color(1, 1 - hurtTime, 1 - hurtTime);

        int c = protect ? 0 : 1;
        hurtImage.color = new Color(c, c, c, 0.5f * hurtTime);
        hurtTime -= 4 * Time.deltaTime;
    }


    //******position, sprite, layer, scale******
    public void MakeEffect(Vector2 p, Sprite s, int l, float sc) //Fade 스크립트가 붙은 오브젝트 생성
    {
        GameObject effect = Instantiate(fadeEffect, p, Quaternion.identity);
        effect.transform.localScale = sc * Vector2.one;

        SpriteRenderer esr = effect.transform.GetComponent<SpriteRenderer>();
        esr.sprite = s;
        esr.flipX = sr.flipX;
        esr.sortingOrder = l;
    }



    void SavePos()
    {
        pos = transform.position;
        po.transform.position = pos;
        posSaved = true;
        postime = 99;
        MakeEffect(pos, posSprite, 8, 0.2f);
    }
    void DamagePos(int i, Vector2 v)
    {
        posP[i] = v;
        MakeEffect(v, posSprite, 10, 1);
    }


    public void ClearBG() //클리어 시 배경 색상 변동
    {
        bgsr.color = Color.white;
        bgtime = 1;
    }

} //Player End
