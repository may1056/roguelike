using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Boss1 : MonoBehaviour{
    //첫번째 보스 체스 퀸
    public static Boss1 boss1;

    Rigidbody2D rigid;
    BoxCollider2D col;
    SpriteRenderer sr;
    public float noticeDist;
    float t; //시간
    float dist;
    public int hp;
    public Text hpText; //임시 체력 텍스트
    public Image hpBAR;
    public Image hpCASE;
    float H;//이동상
    Player player;


    bool moving = false;
    bool spons = true;
    bool leezzang = true;
    public bool phase2 = false;


    //pattern0

    public GameObject hpOrb, mpOrb;

    //pattern1


    //pattern2


    //pattern3
    public GameObject fadeEffect;



    public Sprite Empty;
    public Sprite doubleCircle;
    bool inAttackArea;
    bool withPlayer = false;
    float withPlayerTime = 0;
    Vector2 firstP;


    Vector2 tp;
    //플레이어의 공격 범위 내에 있는지
    public float pollution = 0; //오염 정도
    public bool polluted = false; //오염되었는지

    public GameObject pon, knight, bishop, look, king;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        int l = collision.gameObject.layer;
        if (l == 11 || l == 13) withPlayer = true;

        if (l == 15) transform.position = firstP;
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


    void ModifyHp() //hp bar 최신화
    {
        hpText.text = hp.ToString(); //임시

        if (hp > 0) hpBAR.rectTransform.sizeDelta = new Vector2(hp * 4, 70);
        else hpBAR.gameObject.SetActive(false);

        //if (t > 100 && t < 120)
        //    hpCASE.rectTransform.sizeDelta = new Vector2(16 * (120 - t), 70);
        //else if (t >= 120) hpCASE.gameObject.SetActive(false);
    }

    public void Apa(Color c)
    {
        if (hp > 0)
        {
            GameObject hurt = Instantiate(fadeEffect, transform.position,
                Quaternion.identity);

            SpriteRenderer hsr = hurt.transform.GetComponent<SpriteRenderer>();
            //hsr.flipX = sr.flipX;
            hsr.sortingOrder = 21;
            hsr.sprite = Empty;
            hsr.color = c;

            hurt.GetComponent<Fade>().k = 5;


        }
    }
    private void Ponspon()
    {
        if (leezzang)
        {
            for (int i = 0; i < 2; i++)
            {
                Instantiate(pon, transform.position, Quaternion.identity);

                transform.position = new Vector2(-10 + i * 3, 0);


            }
            transform.position = new Vector2(0, 0);


        }

    }
    void Awake()
    {
        boss1 = this;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        player = Player.player;
        sr = GetComponent<SpriteRenderer>();
        InvokeRepeating(nameof(Ponspon), 3, 13); //ponspon end

        firstP = transform.position;
    }
    private void Update()
    {
        tp = transform.position;
        Vector2 pp = player.transform.position;
        dist = Vector2.Distance(tp, pp);

        H = tp.x > pp.x ? -3 : 3;


        Targeting();

        if (hp <= 80 && hp >60)
        {

            if (spons)
            {
                GameObject knightsss = Instantiate(knight, transform.position, Quaternion.identity);
                transform.position = new Vector2(0,0);
                spons = false;
            }


        }
        else if(hp <= 60 && hp > 40)
        {
            if (!spons)
            {
                GameObject bishopsss = Instantiate(bishop, transform.position, Quaternion.identity);
                transform.position = new Vector2(0, 0);
                spons = true;
            }

        }
        else if (hp <= 40 && hp > 20)
        {
            if (spons)
            {
                GameObject looksss = Instantiate(look, transform.position, Quaternion.identity);
                transform.position = new Vector2(0, 0);
                spons = false;
            }

        }

        if (hp <= 0)
        {
            //Destroy(gameObject);
            GameManager.gameManager.NextStage();
        }

        if (withPlayer) withPlayerTime += Time.deltaTime;
        else withPlayerTime = 0;

        if (withPlayerTime > 0.29f)
        {
            withPlayerTime = -0.5f;
            if (Player.unbeatableTime <= 0) Player.hurted = true;
        }

        ModifyHp();
        //피 닳는 시스템
        if (inAttackArea && (Input.GetMouseButtonDown(0)
            || Input.GetKeyDown("j")) && //내가 마우스가 없어서 임시로 설정한 키
            PlayerAttack.curAttackCooltime >= PlayerAttack.maxAttackCooltime
             && GameManager.prgEnd)
        {
            Apa(Color.red);
            hp -= Player.player.atkPower;
            if (Player.player.purple) //보라 수정: 치명타
            {
                int r = Random.Range(0, 10);
                if (r < 2)
                {
                    hp--;
                    Debug.Log("치명");
                }
            }
            PlayerAttack.curAttackCooltime = 0;
        }
        //스킬 범위 내에 있음
        if (Mathf.Abs(PlayerAttack.skillP.y) < 100 &&
            Vector2.Distance(tp, PlayerAttack.skillP) < 5.5f)
        {
            Apa(Color.red);
            hp--;
            if (Player.player.purple) //보라 수정: 치명타
            {
                int r = Random.Range(0, 10);
                if (r < 2)
                {
                    hp--;
                    Debug.Log("치명");
                }
            }
        }
        //무기 파생 스킬 범위 내에 있음
        Vector2 wsp = PlayerAttack.wsP;
        if (Mathf.Abs(wsp.y) < 100)
        {
            switch (PlayerAttack.weaponNum.Item1)
            {
                case 0:
                    bool inX = Mathf.Abs(wsp.x - tp.x) < 7.5f
                        && Mathf.Abs(wsp.y - tp.y) < 1;
                    bool inY = Mathf.Abs(wsp.y - tp.y) < 7.5f
                        && Mathf.Abs(wsp.x - tp.x) < 1;
                    if (inX || inY)
                    {
                        Apa(Color.red);
                        hp -= 2 * Player.player.atkPower;
                        if (Player.player.purple) //보라 수정: 치명타
                        {
                            int r = Random.Range(0, 10);
                            if (r < 2)
                            {
                                hp -= 2;
                                Debug.Log("치명");
                            }
                        }
                        if (Player.player.poison)
                            Invoke(nameof(AfterDamage), Random.Range(1, 20));
                    }
                    break;
            }
        }
        //자동 공격 오염
        polluted = pollution == 1;
        if (polluted) Invoke(nameof(RemovePollution), 1);
        else if (pollution > 0 && !polluted) pollution -= 0.3f * Time.deltaTime;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color
            = new Color(1, 1, 1, pollution);
        transform.GetChild(0).gameObject.SetActive(pollution > 0);
    }//update end


    public void RepeatAD() //AfterDamage() 반복
    {
        Invoke(nameof(AfterDamage), Random.Range(1, 20));
    }
    void AfterDamage() //poison 아이템 - Invoke용
    {
        if (hp > 0)
        {
            Apa(Color.green);
            hp--;

            if (Player.player.purple) //보라 수정: 치명타
            {
                int r = Random.Range(0, 10);
                if (r < 2)
                {
                    hp--;
                    Debug.Log("치명");
                }
            }
        }
    }

    public void RemovePollution() //오염 제거
    {
        if (polluted) pollution = 0;
        CancelInvoke(nameof(RemovePollution));
    }
    void Targeting() //타겟팅형 몬스터를 위해
    {
        sr.flipX = tp.x < Player.player.transform.position.x - 0.3f;

        //가까우면 이동 시작
        if (dist < noticeDist)
        {

            moving = true;
        }
    }
    private void FixedUpdate()
    {
        if (moving)
        {
            transform.Translate(H * (1 - pollution) *
                Time.deltaTime * Vector2.right);





        }

    }

} //Boss1 End
