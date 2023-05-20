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
    bool attackuse = false;
    Vector2 mousePosition;
    float weaponangle;


    public int mp;
    public int maxmp;
    public float cooltime = 0;

    bool skilluse; //스킬 시전하는지
    public static Vector2 skillP; //스킬 원 위치
    public Sprite skillSprite;


    //weapon skill
    public Text wsText;
    float wsCool = 0;
    public static Vector2 wsP;
    bool wsAvailable = false;
    float wsgoing = 3;
    int wscount = 0;
    public Sprite ws0sprite;
    //아 모르겠다 코드 막 짜야지.. 변수만 몇 개야


    readonly Color[] colors =
        { new Color(1, 0.6f, 1), Color.red, new Color(1, 0.6f, 0), Color.yellow,
        new Color(0, 0.7f, 1), Color.blue, new Color(0.6f, 0.3f, 1),};





    void Start()
    {
        playerAtk = this;


        atk = transform.GetChild(1);
        attacksr = atk.GetComponent<SpriteRenderer>();
        attackbc = atk.GetComponent<BoxCollider2D>();
        attackani = atk.GetComponent<Animator>();

        //attacksr.color = new Color(1, 1, 1, 0);

        wsP = new Vector2(9999, 9999);

        GameManager.ismeleeWeapon = false;
        attackani.SetBool("IsmeleeWeapon", GameManager.ismeleeWeapon); // IsmeleeWeapon 파라미터도 GameManager.ismeleeWeapon 값따라 변경
    }



    void Update()
    {
        WeaponAnimation();

        attacksr.flipX = player.F;
        //atk.transform.localPosition = new Vector2(player.F ? -2f : 2f, 0);


        //ㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱㄱ

        maxAttackCooltime = GameManager.ismeleeWeapon ? 0.2f : 0.5f;
        if (player.selfinjury) maxAttackCooltime *= 0.2f; //자해 시 공격 빠름

        if (curAttackCooltime <= maxAttackCooltime + 2)
            curAttackCooltime += Time.deltaTime;

        attackuse = (Input.GetMouseButton(0) || Input.GetKey("j"))
            && (curAttackCooltime >= maxAttackCooltime); //j는 임시 공격 키


        // 근접공격 쿨타임, 애니메이션
        if (attackuse && GameManager.ismeleeWeapon)
        {
            float x = player.F ? -2 : 2;
            //attackP = new Vector2(transform.position.x + x, transform.position.y);
            //attacksr.color = new Color(1, 1, 1, 1);
            player.dontBehaveTime = 0;
        }
        else if (attackuse) //원거리 공격 쿨타임, 애니메이션?
        {
            float x = player.F ? -2 : 2;
            //attackP = new Vector2(transform.position.x + x, transform.position.y);

            GameObject pb = Instantiate(playerbullet,
                transform.position, Quaternion.Euler(0, 0, player.F ? 180 : 0));
            if (player.selfinjury)
            {
                pb.GetComponent<PlayerBullet>().bulletSpeed = 40;
                pb.transform.rotation = Quaternion.Euler(
                    0, 0, (player.F ? 180 : 0) + Random.Range(-30, 31)); //각도 분산
            }

            //attacksr.color = new Color(1, 1, 1, 1);

            curAttackCooltime = 0;
            player.dontBehaveTime = 0;
        }
        //else attacksr.color = new Color(1, 1, 1, 0);






        //ㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋㅅㅋ

        //기본 탑재 스킬
        if (cooltime > 0) cooltime -= Time.deltaTime;

        skilluse = cooltime <= 0 && Input.GetKeyDown("z") && mp >= 1;

        if (skilluse) //약한 스킬
        {
            if (player.selfinjury) cooltime = 1;
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
        }
        else skillP = new Vector2(9999, 9999); //저 멀리


        //무기 파생 스킬
        if (wsCool > 0)
        {
            wsCool -= Time.deltaTime;
            wsText.text = wsCool.ToString("N1");
        }
        else wsText.text = "0";

        if (wsCool <= 0 && Input.GetKeyDown("x") && mp >= 2)
        {
            wsAvailable = true;
            wsgoing = 3;
            wscount = 3;
            if (player.selfinjury) wsCool = 2; //자해 시 빠른 스킬
            else
            {
                wsCool = 20;
                mp -= 2;
                manager.ChangeHPMP();
            }
            player.dontBehaveTime = 0;
        }

        if (wsAvailable)
        {
            switch (Player.weaponNum)
            {
                case 0:
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



        if (Input.GetKeyDown("p")) Ismeleechange();


        if (mp > maxmp) mp = maxmp;

    } //Update End


    void WeaponAnimation() // 무기 위치 고정, 마우스에 따른 회전
    {
        atk.transform.localPosition = new Vector2(0, 0);
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        weaponangle = Mathf.Atan2(mousePosition.y - atk.position.y, mousePosition.x - atk.position.x) * Mathf.Rad2Deg;
        atk.transform.rotation = Quaternion.AngleAxis(weaponangle - 90, Vector3.forward); ;

        if (attackuse) attackani.SetTrigger("attacked");
    }

    void WeaponSkill0(int co) //채찍 전용 스킬 발현을 매개해주는 역할 //이었는데 이젠 아님
    {
        if (wsgoing <= co)
        {
            wsP = transform.position;
            wscount--;
            player.MakeEffect(transform.position, ws0sprite, 10, 1);
        }
        else wsP = new Vector2(9999, 9999);
    }


    void WeaponSkill1()
    {
        for(int i = -2; i <= 2; i++)
        {
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

        if (GameManager.ismeleeWeapon) //근접공격이면 콜라이더 활성화 아니면 비활성화
        {
            attackbc.enabled = true;

            //무기별 공격 범위 지정 필요
            //attackbc.size = new Vector2( x,y,);

        }
        else attackbc.enabled = false;

    }

    public void Ismeleechange() // onclick
    {
        GameManager.ismeleeWeapon = !GameManager.ismeleeWeapon;
        attackani.SetBool("IsmeleeWeapon", GameManager.ismeleeWeapon); // IsmeleeWeapon 파라미터도 GameManager.ismeleeWeapon 값따라 변경


        if (Player.weaponNum == 0) Player.weaponNum = 1;
        else Player.weaponNum = 0;
    }

} //PlayerAttack End
