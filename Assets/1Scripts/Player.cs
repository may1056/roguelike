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

    public Boss2 boss2;

    Rigidbody2D rigid;
    SpriteRenderer sr;
    Animator anim;


    public static (int, int) itemNum; //????이게 되네
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


    public int atkPower;


    public static float unbeatableTime; //무적 시간
    public float maxunbeatableTime;


    float hurtTime = 0; //피격 시 사용할 시간 변수
    public static bool hurted;
    int inEnemies = 0;
    public Image hurtImage;


    public float slow = 0;
    public float slowtime = 0;
    public Image frozenImage;

    public float burn = 0;
    public float burntime = 0;
    public Image explosiveImage;



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


    public int stamina; //대쉬 제한을 위해 스태미나 필요할 듯
    public int maxstamina;
    public Sprite[] St = new Sprite[6];



    //아이템 관련
    //1. 부활
    bool canRevive;
    public Image reviveImage;
    //3. 자해
    public bool selfinjury;
    //5. 버서커
    public bool berserker;
    //6. 강한 대쉬
    public bool dashdeal;
    public Sprite dashdealEff, dashupgradeEff;
    //7. ~ 12. 빨핑파초노주 수정
    public bool red, pink, blue, green, yellow, orange, purple;
    //13. 독
    public bool poison;




    void Awake()
    {
        player = this; //이게 나다

        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        weaponNum = 1; //임시


        bg = transform.GetChild(2);
        td = transform.GetChild(3);
        cs = transform.GetChild(4);

        unbeatableTime = 0;

        getOrb = false;

        bgsr = bg.GetComponent<SpriteRenderer>();

        canRevive = false;

    } //Awake End



    void Start()
    {
        GetNewItem();

        manager.ChangeHPMP();

    } //Start End



    void GetNewItem() //랜덤 아이템 얻기 - 임시
    {
        itemNum = (4, Random.Range(1, 15));
        if (itemNum.Item1 == itemNum.Item2) itemNum.Item2 = -1; //겹치면 그냥 없앰

        manager.ItemInfo();

        ItemDefault();
        ItemActive(itemNum.Item1);
        ItemActive(itemNum.Item2);
    }
    void ItemDefault() //아이템 설정 기본값으로 되돌리기
    {
        canRevive = false; //1
        transform.GetChild(6).gameObject.SetActive(false); //2
        maxhp = 6; selfinjury = false; //3
        maxshield = 0; shield = 0; //4
        berserker = false; //5
        dashdeal = false; dashDist = 8; maxstamina = 3; //6

        red = false; pink = false; blue = false; green = false; yellow = false; orange = false; purple = false;
        poison = false;
    }
    void ItemActive(int i) //아이템 설정 최신화
    {
        switch (i)
        {
            //legend
            case 0: break; //알파 수정

            //rare
            case 1: canRevive = true; break;
            case 2: transform.GetChild(6).gameObject.SetActive(true); break;
            case 3: maxhp = 1; hp = 1; selfinjury = true; break;
            case 4: maxshield = 2; shield = 2; break;
            case 5: berserker = true; break;
            case 6: dashdeal = true; dashDist = 12; maxstamina = 5; stamina = 5; break;

            //common
            case 7: red = true; break; //밸런스를 위해 딜증 대상은 무기(일반공격, 무기파생스킬)로 한정
            case 8: pink = true; break; //hp 오브 확률 증가
            case 9: blue = true; break; //mp 오브 확률 증가
            case 10: green = true; break; //이동 속도 증가
            case 11: yellow = true; break; //공격 속도 증가
            case 12: orange = true; break; //회피 활성화
            case 13: purple = true; break; //치명 활성화

            case 14: poison = true; break;
        }
    }




    void Update()
    {
        if (GameManager.mapouterror) transform.position = Vector2.zero;

        Vector2 tp = transform.position;


        if (berserker && hp < 3) atkPower = red ? 3 : 2;
        else atkPower = red ? 2 : 1;





        //ㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍ

        //점프
        if ((Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.Space))
            && !isJumping && GameManager.prgEnd)
        {
            rigid.AddForce((1 - slow) * jumpPower * Vector2.up, ForceMode2D.Impulse);
            isJumping = true;
            sr.sprite = players[2];
        }

        //연직 방향 속력이 거의 0인 상태가 0.1초 이상이면 점프 중단
        if (Mathf.Abs(rigid.velocity.y) < 0.01f) notJumpTime += Time.deltaTime;
        else notJumpTime = 0;
        isJumping = notJumpTime < 0.1f;






        //ㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎㅂㅎ

        //방향 전환
        if (Input.GetButton("Horizontal") && GameManager.prgEnd)
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
            hit = Physics2D.Raycast(v, d, dashDist * (1 - slow), lg); //레이 맞은 것 저장

            Debug.DrawRay(v, (1 - slow) * dashDist * d, Color.red, 0.1f); //시각화

            gx[i + 2] = tp.x + (F ? -1 : 1)
                * (hit.transform != null ? hit.distance : dashDist * (1 - slow));
        }
        float tpx = tp.x;

        float m = F ?
                Mathf.Max(gx[0], gx[1], gx[2], gx[3], gx[4]) + 0.7f :
                Mathf.Min(gx[0], gx[1], gx[2], gx[3], gx[4]) - 0.7f;


        //마우스 우클릭 대쉬
        if (!onceDashed && (Input.GetMouseButtonDown(1)
            || Input.GetKeyDown("k")) && slow < 1 && stamina > 0 //k는 임시 대쉬 키
             && GameManager.prgEnd)
        {
            onceDashed = true;

            transform.position = new Vector2(m, tp.y);
            tp = transform.position;

            tpx = tp.x - tpx;
            for (int i = 0; i < 10; i++)
            {
                GameObject dash = Instantiate(fadeEffect,
                    new Vector2(tp.x - tpx * i * 0.1f, tp.y), Quaternion.identity);
                if (dashdeal) //대쉬 이펙트
                    dash.GetComponent<SpriteRenderer>().sprite = dashupgradeEff;
            }

            if (dashdeal) DashDamage(tp.x, tp.x - tpx, tp.y);

            dontBehaveTime = 0;

            CancelInvoke(nameof(RecoverSt));
            stamina--;
            ChangeSt();
            Invoke(nameof(RecoverSt), dashdeal ? 2 : 3); //스태미나 충전
        }

        //대쉬 도달 위치 표시
        td.transform.position = new Vector2(m, tp.y);

        td.transform.GetComponent<SpriteRenderer>().color
            = new Color(1 - slow, 1 - slow, 1);






        //ㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊㅇㅊ
        /*
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
        */






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
        if (Input.GetKeyDown("s") && GameManager.prgEnd)
        {
            isSliding = true;
            this.gameObject.layer = 13; //13PlayerSlide
            slideP = transform.position; //원래 위치 저장
        }

        if (slideP.y - transform.position.y > 2) SlideCheck();







        //ㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍㅇㅍ

        //무적 시간: 안 아픔
        unbeatableTime -= Time.deltaTime;

        //공격당함
        if (hurted)
        {
            int r = Random.Range(0, 10);

            if (orange && r < 2)
            {
                //회피
                Debug.Log("회피");
            }
            else BeforeHurt(1);

            hurted = false;
        }

        if (inEnemies == 0) hurted = false;

        if (hurtTime >= 0) Hurt(); //아플 때
        else sr.color = Color.white; //기본

        if (hp > maxhp) hp = maxhp;




        if (hp <= 0) //쉐이망
        {
            if (canRevive) //부활 아이템
            {
                hp = maxhp;
                shield = maxshield;
                unbeatableTime = 2;
                canRevive = false;
                if (itemNum.Item1 == 1) itemNum.Item1 = -1;
                else itemNum.Item2 = -1;
                manager.ItemInfo();
                GameObject rev = Instantiate(fadeEffect, tp, Quaternion.identity);
                rev.GetComponent<SpriteRenderer>().sprite = posSprite;
                rev.GetComponent<Fade>().k = 0.5f;
                reviveImage.gameObject.SetActive(true);
                Invoke(nameof(AfterRevive), 2);
            }
            else SceneManager.LoadScene(1);
        }

        reviveImage.color = new Color(1, 1, 1, unbeatableTime * 0.5f);





        //ㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹㅅㄹ

        if (slowtime <= 0) slow = 0;
        else if (slow < 1)
        {
            frozenImage.color = new Color(1, 1, 1, slow);
            slow -= 0.2f * Time.deltaTime;
            if (slow <= 0)
            {
                slow = 0;
                slowtime = 0;
            }
        }
        else
        {
            frozenImage.color = Color.blue;
            slowtime -= 2 * Time.deltaTime;
        }

        frozenImage.gameObject.SetActive(slow > 0);


        //ㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂㅍㅂ

        if (burntime <= 0)
        {
            burn = 0;
            CancelInvoke(nameof(Explode));
        }
        else if (burn < 1)
        {
            explosiveImage.color = new Color(1, 1, 1, burn);
            burn -= 0.2f * Time.deltaTime;
            if (burn <= 0)
            {
                burn = 0;
                burntime = 0;
            }
        }
        else
        {
            explosiveImage.color = Color.yellow; //주황색으로 바꿔야 함
            burntime -= Time.deltaTime;
        }

        explosiveImage.gameObject.SetActive(burn > 0);






        //ㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷ

        //쉴드 재충전
        dontBehaveTime += Time.deltaTime;
        if (dontBehaveTime > (selfinjury ? 10 : 3) && shield < maxshield)
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

        transform.Translate(speed * (1 - slow) * (green ? 2 : 1)
            * Time.deltaTime * new Vector2(h, 0));

        //isWalking = h != 0;

        //if (onceDashed) //대쉬 후 이펙트는 계속 만들어줌
            //Instantiate(dashEffect, transform.position, Quaternion.identity);

    } //FixedUpdate End




    private void OnCollisionEnter2D(Collision2D collision)
    {
        int l = collision.gameObject.layer;

        //플랫폼 닿으면 점프 상태 냅다 해제 (잘 안 먹힘..)
        if (collision.gameObject.CompareTag("Platform")) isJumping = false;

        if (collision.gameObject.CompareTag("Enemy")) inEnemies++;

        if (l == 17)
        {
            hp--;
            manager.ChangeHPMP();
        }


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





    public void BeforeHurt(int d)
    {
        unbeatableTime = maxunbeatableTime;
        if (shield >= d)
        {
            shield -= d;
            protect = true;
        }
        else
        {
            hp -= d - shield;
            shield = 0;
            protect = false;
        }
        manager.ChangeHPMP();
        hurtTime = 1;
        dontBehaveTime = 0;
    }

    void Hurt() //잠깐 붉은색 되었다가 서서히 회복
    {
        //sr.color = new Color(1, 1 - hurtTime, 1 - hurtTime);

        int c = protect ? 0 : 1;
        hurtImage.color = new Color(c, c, c, 0.5f * hurtTime);
        hurtTime -= 4 * Time.deltaTime;
    }



    public void RepeatEx()
    {
        InvokeRepeating(nameof(Explode), 0, 0.98f);
    }
    void Explode() //폭발성 데미지 - 지속 딜 Invoke용
    {
        //hurtImage는 주황색~
        hurted = true;
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


    void RecoverSt()
    {
        CancelInvoke(nameof(RecoverSt));
        if (stamina < maxstamina) stamina++;
        ChangeSt();

        Invoke(nameof(RecoverSt), selfinjury ? (dashdeal ? 4.5f : 9) : (dashdeal ? 1.5f : 3));
    }
    void ChangeSt()
    {
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = St[stamina];
    }



    //아이템 관련 함수들

    void AfterRevive() //1. 부활
    {
        reviveImage.gameObject.SetActive(false);
        manager.ChangeHPMP();
    }

    void DashDamage(float X1, float X2, float Y) //6. 강한 대쉬
    {
        if (manager.transform.childCount == 1)
        {
            Transform c0 = manager.transform.GetChild(0);

            if (c0.childCount > 2)
            {
                for (int i = 2; i < c0.childCount; i++)
                {
                    Transform c0i = c0.GetChild(i);

                    for (int j = 0; j < c0i.childCount; j++)
                    {
                        Transform c0ij = c0i.GetChild(j);

                        Vector2 mv = c0ij.position;

                        if (((mv.x > X1 && mv.x < X2) || (mv.x < X1 && mv.x > X2))
                            && mv.y > Y - 2 && mv.y < Y + 2)
                        {
                            Monster c0ijm = c0ij.GetComponent<Monster>();
                            c0ijm.Apa(Color.red);
                            c0ijm.hp--;
                            if (purple)
                            {
                                int r = Random.Range(0, 10);
                                if (r < 2)
                                {
                                    c0ijm.hp--;
                                    Debug.Log("치명");
                                }
                            }
                            if (poison) c0ijm.RepeatAD();

                            GameObject dd = Instantiate(fadeEffect, mv, Quaternion.identity);
                            SpriteRenderer ddsr = dd.GetComponent<SpriteRenderer>();
                            ddsr.sprite = dashdealEff;
                            ddsr.sortingOrder = 8;
                        }

                    }
                }
            }
        }

        if (boss2.gameObject.activeSelf)
        {
            Vector2 b2p = boss2.transform.position;

            if (((b2p.x > X1 && b2p.x < X2) || (b2p.x < X1 && b2p.x > X2))
                && b2p.y > Y - 1 && b2p.y < Y + 1)
            {
                boss2.hp--;
                if (purple)
                {
                    int r = Random.Range(0, 10);
                    if (r < 2)
                    {
                        boss2.hp--;
                        Debug.Log("치명");
                    }
                }
                if (poison) boss2.RepeatAD();

                GameObject dd = Instantiate(fadeEffect, b2p, Quaternion.identity);
                SpriteRenderer ddsr = dd.GetComponent<SpriteRenderer>();
                ddsr.sprite = dashdealEff;
                ddsr.sortingOrder = 8;

                if (boss2.hide) boss2.PlayerKnows();
            }
        }

    } //DashDamage End





    public void ClearBG() //클리어 시 배경 색상 변동
    {
        bgsr.color = Color.white;
        bgtime = 1;
    }

} //Player End
