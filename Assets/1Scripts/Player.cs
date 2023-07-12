using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour //플레이어
{
    public GameManager manager;

    public static Player player;
    //public static으로 설정한 변수는 다른 스크립트에서 맘대로 퍼갈 수 있다

    public Boss1 boss1;
    public Boss2 boss2;

    Rigidbody2D rigid;
    SpriteRenderer sr;
    Animator anim;




    Transform bg; //배경
    SpriteRenderer bgsr; //배경 스프라이트렌더러
    public Sprite[] bgArtworks; //배경 이미지


    Transform td; //대쉬 끝 위치
    public Transform po; //저장된 위치 표시 오브젝트
    Transform cs; //슬라이드 가능 알림 화살표


    public GameObject fadeEffect;


    public Sprite[] players = new Sprite[3]; //지금은 사용 안 하는 중!!!!!!!!
    //애니메이션 만들기 귀찮아서 임시방편으로 스프라이트 교체용
    //0: 평소 상태(멈춤), 1: 걷기(달리기), 2: 뛰기(점프)


    public int hp;
    public int maxhp;
    static int savedhp = 6;

    public int shield;
    public int maxshield;
    bool protect;


    public GameObject death;


    public int atkPower; //일반 공격 딜
    public int skillPower; //스킬 딜


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

    public float dark = 0;
    public float darktime = 0;
    public Image anboyeoImage;



    public float dontBehaveTime = 0;


    public float speed = 0; //달리기 속도


    //bool isWalking = false;
    bool isJumping = false;
    public float jumpPower; //뛰는 힘
    float notJumpTime = 0; //가만히 있는 시간



    bool onceDashed = false; //공중에서 대쉬를 이미 했는지
    public Sprite dashSpriteIdle, dashSpriteWalk;
    public float dashDist; //가능한 대쉬 거리
    public LayerMask lg; //Ground
    Animator animm;

    //save position
    public Text posText;
    //float posCool = 0;
    Vector2 pos = Vector2.zero;
    //bool posSaved = false;
    public Sprite posSprite;
    //float postime = 0;
    public static Vector2[] posP =
        { new Vector2(9999, 9999) , new Vector2(9999, 9999) };



    bool isSliding; //플랫폼 내려가는 중인지
    Vector2 slideP;
    public LayerMask lb; //Block


    public bool F;


    public int stamina; //대쉬 제한을 위해 스태미나 필요할 듯
    public int maxstamina;
    public Sprite[] St = new Sprite[6];



    //아이템 관련
    public static (int, int) itemNum = (-1, -1); //????이게 되네

    //1. 부활
    bool canRevive;
    public Image reviveImage;
    //3. 자해
    public bool selfinjury;
    //5. 버서커
    public bool berserker;
    //6. 강한 대쉬
    public bool dashdeal;
    public Sprite dashdealEff, dashupgradeEffIdle, dashupgradeEffWalk;
    //7. ~ 13. 빨핑파초노주보 수정
    public int red, pink, blue, green, yellow, orange, purple;
    public Sprite avoid, critical;
    //14. 독
    public bool poison;

    public ParticleSystem item1ps, item2ps, subps, nowps;



    // 보스전 전용 무기
    public static bool Pickaxe = false;



    public Sprite Empty;


    public AudioSource dodge;
    public AudioSource jump;
    public AudioSource recover;
    public AudioSource ouch;
    public AudioSource pickupcoin;
    public AudioSource pickupitem;



    public void SaveHP()
    {
        savedhp = hp;
    }


    void Awake()
    {
        player = this; //이게 나다

        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        animm = GetComponent<Animator>();


        bg = transform.GetChild(2);
        bgsr = bg.GetComponent<SpriteRenderer>();

        td = transform.GetChild(3);

        cs = transform.GetChild(4);
        cs.GetChild(0).gameObject.SetActive(Mainmenu.markkey);

        unbeatableTime = 0;

        hp = savedhp;

    } //Awake End



    void Start()
    {
        GetNewItem();

        manager.ChangeHPMP();

        ChangeSt();

    } //Start End



    public void GetNewItem() //아이템 구성에 변화가 생겼겠다 싶으면 실행
    {
        //itemNum = (Random.Range(0, 15), Random.Range(0, 15));
        //if (itemNum.Item1 == itemNum.Item2) itemNum.Item2 = -1; //겹치면 그냥 없앰

        manager.ItemInfo();

        ItemDefault();
        ItemActive(itemNum.Item1);
        ItemActive(itemNum.Item2);
    }
    void ItemDefault() //아이템 설정 기본값으로 되돌리기
    {
        canRevive = false; //1
        transform.GetChild(6).gameObject.SetActive(false); transform.GetChild(6).transform.GetComponent<AutoAttack>().CancelInvoke(); //2
        maxhp = 6; selfinjury = false; //3
        maxshield = 1; shield = 1; //4
        berserker = false; //5
        dashdeal = false; dashDist = 8; maxstamina = 3; //6

        red = 0; pink = 0; blue = 0; green = 0; yellow = 0; orange = 0; purple = 0;
        poison = false;
    }
    void ItemActive(int i) //아이템 설정 최신화
    {
        switch (i)
        {
            //legend
            case 0: red++; pink++; blue++; green++; yellow++; orange++; purple++; break; //알파 수정

            //rare
            case 1: canRevive = true; break;
            case 2: transform.GetChild(6).gameObject.SetActive(true); break;
            case 3: maxhp = 1; hp = 1; selfinjury = true; PlayerAttack.playerAtk.mp = 6; break;
            case 4: maxshield = 2; shield = 2; maxunbeatableTime = 0.6f; break;
            case 5: berserker = true; break;
            case 6: dashdeal = true; dashDist = 12; maxstamina = 5; stamina = 5; break;

            //common
            case 7: red++; break; //밸런스를 위해 딜증 대상은 무기(일반공격, 무기파생스킬)로 한정
            case 8: pink++; break; //hp 오브 확률 증가
            case 9: blue++; break; //mp 오브 확률 증가
            case 10: green++; break; //이동 속도 증가
            case 11: yellow++; break; //공격 속도 증가
            case 12: orange++; break; //회피 활성화
            case 13: purple++; break; //치명 활성화

            case 14: poison = true; break;
        }
    }



    public void OnEnable()
    {
        //UI 파티클시스템 크기가 카메라 사이즈에 따라 들쑥날쑥이라 비율 맞춤
        Camera cam = transform.GetChild(0).GetComponent<Camera>();
        Vector2 uipsv = new(cam.orthographicSize / 8, cam.orthographicSize / 8); //아 반대로 작업했다..
        item1ps.GetComponent<RectTransform>().localScale = uipsv;
        item2ps.GetComponent<RectTransform>().localScale = uipsv;
        subps.GetComponent<RectTransform>().localScale = uipsv;
        nowps.GetComponent<RectTransform>().localScale = uipsv;
    }



    void Update()
    {
        if (GameManager.mapouterror) transform.position = Vector2.zero;

        Vector2 tp = transform.position;


        skillPower = berserker && hp < 3 ? 3 : 1;
        atkPower = skillPower + red;

        //animation player
        if (Input.GetButton("Horizontal"))
            animm.SetBool("iswalking2", true);
        else
            animm.SetBool("iswalking2", false);

        //if ()
        //    animm.SetBool("isjumping2", false);
        //else
        //    animm.SetBool("isjumping2", true);



        //ㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍㅈㅍ

        //점프
        if ((Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.Space))
            && !isJumping && GameManager.prgEnd)
        {
            rigid.AddForce((1 - slow) * jumpPower * (1 + Mathf.Pow(green, 0.6f) * 0.2f) * Vector2.up, ForceMode2D.Impulse);
            jump.Play();
            isJumping = true;
            sr.sprite = players[2];
        }

        //연직 방향 속력이 거의 0인 상태가 0.1초 이상이면 점프 중단
        if (Mathf.Abs(rigid.velocity.y) < 0.01f) notJumpTime += Time.deltaTime;
        else notJumpTime = 0;
        isJumping = notJumpTime < 0.02f;

        animm.SetBool("isjumping2", isJumping);





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
            Soundmanager.soundmanager.dashsound.Play();
            transform.position = new Vector2(m, tp.y);
            tp = transform.position;

            tpx = tp.x - tpx;
            for (int i = 0; i < 10; i++)
            {
                GameObject dash = Instantiate(fadeEffect,
                    new Vector2(tp.x - tpx * i * 0.1f, tp.y), Quaternion.identity);

                SpriteRenderer dashsr = dash.GetComponent<SpriteRenderer>();
                dashsr.flipX = F;

                if (Input.GetButton("Horizontal") && !isJumping)
                    dashsr.sprite = dashdeal ? dashupgradeEffWalk : dashSpriteWalk;
                else dashsr.sprite = dashdeal ? dashupgradeEffIdle : dashSpriteIdle;
            }

            if (dashdeal) DashDamage(tp.x, tp.x - tpx, tp.y);

            dontBehaveTime = 0;

            CancelInvoke(nameof(RecoverSt));
            stamina--;
            ChangeSt();
            Invoke(nameof(RecoverSt), dashdeal ? 2 : 3); //스태미나 충전

            manager.ReadOn(5, 1);
            manager.ReadOn(6, 1);
        }

        //대쉬 도달 위치 표시
        td.transform.position = new Vector2(m, tp.y);

        td.transform.GetComponent<SpriteRenderer>().color
            = new Color(1 - slow, 1 - slow, 1, stamina > 0 ? 1 : 0.5f);

        var tdmain = td.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        tdmain.startColor = new Color(1 - slow, 1 - slow, 1);

        td.transform.GetChild(0).gameObject.SetActive(stamina > 0);






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
            int r = Random.Range(0, 5);

            if (r < orange)
            {
                //회피
                dodge.Play();
                MakeEffect(new Vector2(tp.x, tp.y + 2), avoid, 5, 1);
            }
            else
            {
                ouch.Play();
                BeforeHurt(1);
            }

            hurted = false;
        }

        if (inEnemies == 0) hurted = false;

        if (hurtTime > 0) Hurt(); //아플 때
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
            else
            {
                death.SetActive(true);
                death.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text
                    = "+" + GameManager.coins.ToString();
                Time.timeScale = 0;
            }
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
            explosiveImage.color = new Color(1, 0.8f, 0, burn);
            burn -= 0.2f * Time.deltaTime;
            if (burn <= 0)
            {
                burn = 0;
                burntime = 0;
            }
        }
        else
        {
            explosiveImage.color = new Color(1, 0.5f, 0);
            burntime -= Time.deltaTime;
        }

        explosiveImage.gameObject.SetActive(burn > 0);


        //ㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁㅅㅁ

        if (darktime <= 0) dark = 0;
        else if (dark < 1)
        {
            anboyeoImage.color = new Color(0, 0, 0, dark);
            dark -= 0.2f * Time.deltaTime;
            if (dark <= 0)
            {
                dark = 0;
                darktime = 0;
            }
        }
        else
        {
            anboyeoImage.color = Color.black;
            darktime -= Time.deltaTime;
        }

        anboyeoImage.gameObject.SetActive(dark > 0);
        for (int i = 0; i < 5; i++)
        {
            Image blind = anboyeoImage.transform.GetChild(i).GetComponent<Image>();
            blind.color = new Color(blind.color.r, blind.color.g, blind.color.b, dark);
        }






        //ㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷㅅㄷ

        //쉴드 재충전
        dontBehaveTime += Time.deltaTime;
        if (dontBehaveTime > (selfinjury ? 2 : 1) * (4 - maxshield) && shield < maxshield)
        {
            recover.Play();
            shield++;
            dontBehaveTime = 0;
            manager.ChangeHPMP();
        }







        bg.transform.localPosition = -0.1f * tp; //배경 이동



        if (itemNum.Item1 != -1)
        {
            NewWonderfulLeejonghwanShitWow.itemOpen[itemNum.Item1] = true;

            //ParticleSystem.ShapeModule i1psshape = item1ps.shape;
            //i1psshape.texture = GameManager.gameManager.itemSprites[itemNum.Item1].texture;
        }
        if (itemNum.Item2 != -1)
        {
            NewWonderfulLeejonghwanShitWow.itemOpen[itemNum.Item2] = true;

            //ParticleSystem.ShapeModule i2psshape = item2ps.shape;
            //i2psshape.texture = GameManager.gameManager.itemSprites[itemNum.Item2].texture;
        }

        item1ps.gameObject.SetActive(itemNum.Item1 != -1);
        item2ps.gameObject.SetActive(itemNum.Item2 != -1);

    } //Update End




    void FixedUpdate()
    {
        //좌우 이동 (등속, 손 떼면 바로 멈춤)
        float h = Input.GetAxisRaw("Horizontal");

        transform.Translate(speed * (1 - slow) * (1 + 0.5f * green)
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

        if (l == 15)
        {
            transform.position = new Vector2(0, 0); // 맵탈출시 원상복귀
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

        GameObject aya = Instantiate(fadeEffect, transform.position, Quaternion.identity);
        SpriteRenderer ayasr = aya.transform.GetComponent<SpriteRenderer>();
        //ayasr.sprite = Empty;
        ayasr.color = protect ? Color.gray : Color.red;
        ayasr.flipX = F;
        ayasr.sortingOrder = 8;
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
        if (s == avoid || s == critical) esr.flipX = false;
        else esr.flipX = sr.flipX;
        esr.sortingOrder = l;
    }



    //void SavePos()
    //{
    //    pos = transform.position;
    //    po.transform.position = pos;
    //    posSaved = true;
    //    postime = 99;
    //    MakeEffect(pos, posSprite, 8, 0.2f);
    //}
    //void DamagePos(int i, Vector2 v)
    //{
    //    posP[i] = v;
    //    MakeEffect(v, posSprite, 10, 1);
    //}


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

        ParticleSystem.MainModule stpsmain = transform.GetChild(5).GetChild(0).GetComponent<ParticleSystem>().main;
        stpsmain.startLifetime = stamina;
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
                            c0ijm.hp -= skillPower;

                            int r = Random.Range(0, 5);
                            if (r < purple)
                            {
                                c0ijm.hp--;
                                MakeEffect(new Vector2(mv.x, mv.y + 2), critical, 5, 1);
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


        if (boss1.gameObject.activeSelf)
        {
            Vector2 b1p = boss1.transform.position;

            if (((b1p.x > X1 && b1p.x < X2) || (b1p.x < X1 && b1p.x > X2))
                && b1p.y > Y - 7 && b1p.y < Y + 7)
            {
                boss1.hp -= skillPower;

                int r = Random.Range(0, 5);
                if (r < purple)
                {
                    boss1.hp--;
                    MakeEffect(new Vector2(b1p.x, b1p.y + 9), critical, 5, 1);
                }
                if (poison) boss2.RepeatAD();

                GameObject dd = Instantiate(fadeEffect, b1p, Quaternion.identity);
                SpriteRenderer ddsr = dd.GetComponent<SpriteRenderer>();
                ddsr.sprite = dashdealEff;
                ddsr.sortingOrder = 8;
            }


            for (int j = 0; j < boss1.chessEmptySpace.transform.childCount; j++)
            {
                Transform cj = boss1.chessEmptySpace.transform.GetChild(j);

                Vector2 mv = cj.position;

                if (((mv.x > X1 && mv.x < X2) || (mv.x < X1 && mv.x > X2))
                    && mv.y > Y - 3 && mv.y < Y + 3)
                {
                    Monster cjm = cj.GetComponent<Monster>();
                    cjm.Apa(Color.red);
                    cjm.hp -= skillPower;

                    int r = Random.Range(0, 5);
                    if (r < purple)
                    {
                        cjm.hp--;
                        MakeEffect(new Vector2(mv.x, mv.y + 4), critical, 5, 1);
                    }
                    if (poison) cjm.RepeatAD();

                    GameObject dd = Instantiate(fadeEffect, mv, Quaternion.identity);
                    SpriteRenderer ddsr = dd.GetComponent<SpriteRenderer>();
                    ddsr.sprite = dashdealEff;
                    ddsr.sortingOrder = 8;
                }

            }
        }


        if (boss2.gameObject.activeSelf)
        {
            Vector2 b2p = boss2.transform.position;

            if (((b2p.x > X1 && b2p.x < X2) || (b2p.x < X1 && b2p.x > X2))
                && b2p.y > Y - 1 && b2p.y < Y + 1)
            {
                boss2.hp -= skillPower;

                int r = Random.Range(0, 5);
                if (r < purple)
                {
                    boss2.hp--;
                    MakeEffect(new Vector2(b2p.x, b2p.y + 2), critical, 5, 1);
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




    public void StartBG(int num) //시작 시 배경 선정
    {
        bgsr.sprite = bgArtworks[num];
        bgsr.color = new Color(0.7f, 0.7f, 0.7f);
    }
    public void ClearBG() //클리어 시 배경 색상 변동
    {
        bgsr.color = Color.white;
    }


    public void Die()
    {
        NewWonderfulLeejonghwanShitWow.savedcoin += GameManager.coins;
        PlayerPrefs.SetInt("SavedCoin", NewWonderfulLeejonghwanShitWow.savedcoin);
        NewWonderfulLeejonghwanShitWow.SaveWhenGameEnds();
        SceneManager.LoadScene(0);
    }

} //Player End
