using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : MonoBehaviour
{
    public Player player;
    public GameManager manager;

    public static PlayerAttack playerAtk;


    public static (int, int) weaponNum = (0, 1);


    Transform atk; //공격 범위

    SpriteRenderer attacksr;

    BoxCollider2D attackbc;

    Animator attackani;

    public GameObject playerbullet;


    public static float maxAttackCooltime = 0.2f; // 쿨타임 시간
    public static float curAttackCooltime = 0; // 현재 쿨타임
    public float attackspeed = 0; // 공격 속도
    public static Vector2 attackP;
    public Sprite attackSprite;
    public static bool attackuse = false;
    Vector2 mousePosition;
    float weaponangle;


    public int mp;
    public int maxmp;
    static int savedmp = 6;

    public float cooltime = 0;

    bool skilluse; //스킬 시전하는지
    public static Vector2 skillP; //스킬 원 위치
    public Sprite skillSprite;


    //weapon skill
    float wsCool = 0;
    public static Vector2 wsP;
    bool wsAvailable = false;
    float wsgoing = 3;
    int wscount = 0;
    public Sprite ws0sprite;
    //아 모르겠다 코드 막 짜야지.. 변수만 몇 개야



    public Image skillZ, skillX;
    Text skillZcooltimeText, skillXcooltimeText;
    public Sprite swordX, gunwandX;





    public void SaveMP()
    {
        savedmp = mp;
    }


    void Start()
    {
        playerAtk = this;


        atk = transform.GetChild(1);
        attacksr = atk.GetComponent<SpriteRenderer>();
        attackbc = atk.GetComponent<BoxCollider2D>();
        attackani = atk.GetComponent<Animator>();

        //attacksr.color = new Color(1, 1, 1, 0);

        wsP = new Vector2(9999, 9999);

        skillZcooltimeText = skillZ.transform.GetChild(1).GetComponent<Text>();
        skillXcooltimeText = skillX.transform.GetChild(1).GetComponent<Text>();

        GameManager.ismeleeWeapon = GameManager.gameManager.ismelee[weaponNum.Item1];
        attackani.SetBool("IsmeleeWeapon", GameManager.ismeleeWeapon); // IsmeleeWeapon 파라미터도 GameManager.ismeleeWeapon 값따라 변경

        mp = savedmp;
        manager.ChangeHPMP();

        // 초기화
        mp = 6;
        //SaveMP();

        GetNewWeapon();

    } //Start End



    public void GetNewWeapon()
    {
        manager.WeaponInfo();
    }




    void Update()
    {
        WeaponAnimation();

        attacksr.flipX = player.F;
        //atk.transform.localPosition = new Vector2(player.F ? -2f : 2f, 0);


        //ㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱ

        maxAttackCooltime = GameManager.ismeleeWeapon ? 0.2f : 0.5f;
        if (player.selfinjury) maxAttackCooltime *= 0.4f; //자해 시 공격 빠름
        if (player.yellow) maxAttackCooltime *= 0.6f; //노란 수정 공속 증가

        if (curAttackCooltime <= maxAttackCooltime + 2)
            curAttackCooltime += Time.deltaTime;

        attackuse = (Input.GetMouseButton(0) || Input.GetKey("j"))
            && (curAttackCooltime >= maxAttackCooltime); //j는 임시 공격 키



        // 근접공격 쿨타임, 애니메이션
        if (attackuse && GameManager.ismeleeWeapon)
        {
            //Soundmanager.soundmanager.swordsounds[0].Play();
            float x = player.F ? -2 : 2;
            //attackP = new Vector2(transform.position.x + x, transform.position.y);
            //attacksr.color = new Color(1, 1, 1, 1);
            player.dontBehaveTime = 0;
        }
        else if (attackuse) //원거리 공격 쿨타임, 애니메이션?
        {
            Soundmanager.soundmanager.magicgunsounds[0].Play();
            float x = player.F ? -2 : 2;
            //attackP = new Vector2(transform.position.x + x, transform.position.y);

            GameObject pb = Instantiate(playerbullet,
                transform.position, Quaternion.Euler(0, 0, player.F ? 180 : 0));
            if (player.selfinjury)
            {
                pb.GetComponent<PlayerBullet>().bulletSpeed = 30;
                pb.transform.rotation = Quaternion.Euler(
                    0, 0, (player.F ? 180 : 0) + Random.Range(-30, 31)); //각도 분산
            }

            //attacksr.color = new Color(1, 1, 1, 1);

            curAttackCooltime = 0;
            player.dontBehaveTime = 0;
        }
        //else attacksr.color = new Color(1, 1, 1, 0);


        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown("j"))
            && curAttackCooltime >= maxAttackCooltime)
        {
            //소리

            manager.ReadOn(5, 0);
            manager.ReadOn(6, 0);
        }



        //ㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋ

        //기본 탑재 스킬
        if (cooltime > 0)
        {
            cooltime -= Time.deltaTime;
            skillZcooltimeText.text = cooltime.ToString("N1");
        }
        else skillZcooltimeText.text = null;

        skillZ.transform.GetChild(0).GetComponent<Image>().
            fillAmount = cooltime / (player.selfinjury ? 1.5f : 3.0f); //cover

        skilluse = cooltime <= 0 && Input.GetKeyDown("z") && mp >= 1
            && GameManager.prgEnd;

        if (skilluse) //약한 스킬
        {
            Soundmanager.soundmanager.basicskillsound.Play();
            if (player.selfinjury) cooltime = 1.5f;
            else
            {
                cooltime = 3;
                mp--;
                manager.ChangeHPMP();
            }
            float x = player.F ? -6 : 6;
            skillP = new Vector2(transform.position.x + x, transform.position.y);
            player.MakeEffect(skillP, skillSprite, -2, 1);
            player.dontBehaveTime = 0;
            skillZ.gameObject.SetActive(true);
            manager.ReadOn(2, 0);
        }
        else skillP = new Vector2(9999, 9999); //저 멀리


        //무기 파생 스킬
        if (wsCool > 0)
        {
            wsCool -= Time.deltaTime;
            skillXcooltimeText.text = wsCool.ToString("N1");
        }
        else skillXcooltimeText.text = null;

        skillX.transform.GetChild(0).GetComponent<Image>().
                fillAmount = wsCool / (player.selfinjury ? 5.0f : 10.0f); //Filled

        if (wsCool <= 0 && Input.GetKeyDown("x") && mp >= 2
            && GameManager.prgEnd)
        {
            wsAvailable = true;
            wsgoing = 3;
            wscount = 3;
            if (player.selfinjury) wsCool = 5; //자해 시 빠른 스킬
            else
            {
                wsCool = 10;
                mp -= 2;
                manager.ChangeHPMP();
            }
            player.dontBehaveTime = 0;
            skillX.gameObject.SetActive(true);
            manager.ReadOn(2, 1);
        }

        if (wsAvailable)
        {
            switch (weaponNum.Item1)
            {
                case 0:
                    Soundmanager.soundmanager.swordsounds[1].Play();
                    WeaponSkill0(wscount);
                    wsgoing -= 2 * Time.deltaTime;
                    break;

                case 1:
                    WeaponSkill1();
                    wsAvailable = false;
                    wsgoing = 0;
                    wscount = 0;
                    break;
            }

            wsAvailable = wsgoing > 0;
        }



        if (Input.GetKeyDown("c") && GameManager.prgEnd) Ismeleechange();


        if (mp > maxmp) mp = maxmp;

    } //Update End


    void WeaponAnimation() // 무기 위치 고정, 마우스에 따른 회전
    {
        atk.transform.localPosition = new Vector2(0, 0);
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        weaponangle = Mathf.Atan2(mousePosition.y - atk.position.y, mousePosition.x - atk.position.x) * Mathf.Rad2Deg;
        atk.transform.rotation = Quaternion.AngleAxis(weaponangle - 90, Vector3.forward); ;

        if (attackuse) attackani.SetTrigger("attacked");

        if (GameManager.ismeleeWeapon) attacksr.color = new Color(1, 1, 1, 1);
        else attacksr.color = new Color(1, 1, 1, 0);
    }

    void WeaponSkill0(int co) //채찍 전용 스킬 발현을 매개해주는 역할 //이었는데 이젠 아님
    {
        if (wsgoing <= co)
        {
            wsP = transform.position;
            wscount--;
            player.MakeEffect(transform.position, ws0sprite, 10, 1);
            //소리
        }
        else wsP = new Vector2(9999, 9999);
    }


    void WeaponSkill1()
    {
        for(int i = -2; i <= 2; i++)
        {
            Soundmanager.soundmanager.magicgunsounds[1].Play();
            GameObject ws1 = Instantiate(playerbullet,
                transform.position, Quaternion.Euler(0, 0, 20 * i + (player.F ? 180 : 0)));
            ws1.GetComponent<PlayerBullet>().pbType = 1;
            //ws1.GetComponent<SpriteRenderer>().color = colors[i+3];
        }
    }


    void WeaponChange()
    {
        //애니메이션 변경 자체는 애니메이터 파라미터에서 하는걸로 하고 여기서는 컨트롤러 값 수정
        //attackani.SetInteger("컨트롤러 이름", 변경값) 이게 변경 함수니 기억하렴 종환아 + SetFloat,SetBool,SetTrigger

        /*
        if (GameManager.ismeleeWeapon) //근접공격이면 콜라이더 활성화 아니면 비활성화
        {
            attackbc.enabled = true;

            //무기별 공격 범위 지정 필요
            //attackbc.size = new Vector2( x,y,);

        }
        else attackbc.enabled = false;
        */ //일단 주석 처리함

        if (weaponNum.Item2 != -1)
        {
            int temp = weaponNum.Item1;
            weaponNum.Item1 = weaponNum.Item2;
            weaponNum.Item2 = temp;
        }
    }

    public void Ismeleechange() // onclick
    {
        GameManager.ismeleeWeapon = !GameManager.ismeleeWeapon;
        attackani.SetBool("IsmeleeWeapon", GameManager.ismeleeWeapon); // IsmeleeWeapon 파라미터도 GameManager.ismeleeWeapon 값따라 변경

        skillX.sprite = GameManager.ismeleeWeapon ? swordX : gunwandX;

        WeaponChange();
    }


} //PlayerAttack End
