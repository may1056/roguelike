using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Boss1 : MonoBehaviour
{
    //첫번째 보스 체스 퀸
    public static Boss1 boss1;

    public GameObject chessEmptySpace; //체스말들 다 여기 종속시킬 거임

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
    float gauge;

    float H; //이동 상수


    Player player;
    public Camera cam, uicam;


    bool spons = true;
    bool leezzang = true;

    int pieceCount = 0;


    //pattern0

    //public GameObject hpOrb, mpOrb;

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

    public GameObject crown_ps;


    bool hm;




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
        if (hp > 0) hpBAR.rectTransform.sizeDelta = new Vector2(hp, 70);
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


    void BeforeSpawn()
    {
        GameObject cr = Instantiate(crown_ps, new(tp.x, tp.y + 4.9f), Quaternion.identity);
        Destroy(cr, 3);
    }
    private void Spawn()
    {
        //if (leezzang)
        //{
        //    for (int i = 0; i < 2; i++)
        //    {
        //        GameObject PON = Instantiate(pon, transform.position, Quaternion.identity);
        //        PON.transform.SetParent(chessEmptySpace.transform);

        //        transform.position = new Vector2(-10 + i * 3, 0);
        //    }
        //    transform.position = new Vector2(0, 0);
        //}

        //4.9

        GameObject[] pieces = new GameObject[8];
        Vector2 v = new(tp.x, tp.y + 4.9f);

        switch (pieceCount)
        {
            case 0: //8폰
                for (int i = 0; i < 8; i++) pieces[i] = Instantiate(pon, v, Quaternion.identity);
                break;

            case 1: //6폰 2말
                for (int i = 0; i < 8; i++) pieces[i] = Instantiate(i < 6 ? pon : knight, v, Quaternion.identity);
                break;

            case 2: //4폰 2말 2비숍
                for (int i = 0; i < 4; i++) pieces[i] = Instantiate(pon, v, Quaternion.identity);
                for (int i = 4; i < 6; i++)
                {
                    pieces[i] = Instantiate(knight, v, Quaternion.identity);
                    pieces[i + 2] = Instantiate(bishop, v, Quaternion.identity);
                }
                break;

            case 3: //2폰 2말 2비숍 2룩
                for (int i = 0; i < 2; i++)
                {
                    pieces[i] = Instantiate(pon, v, Quaternion.identity);
                    pieces[i + 2] = Instantiate(knight, v, Quaternion.identity);
                    pieces[i + 4] = Instantiate(bishop, v, Quaternion.identity);
                    pieces[i + 6] = Instantiate(look, v, Quaternion.identity);
                }
                break;
        }


        int[] r8n = Random8Numbers();
        for (int n = 0; n < 8; n++)
        {
            pieces[n].transform.SetParent(chessEmptySpace.transform);

            pieces[n].GetComponent<Monster>().rigid = pieces[n].GetComponent<Rigidbody2D>();
            pieces[n].GetComponent<Monster>().rigid.AddForce(200 * new Vector2(2 * Mathf.Cos(45 * r8n[n]), Mathf.Sin(45 * r8n[n])));
        }

    }
    int[] Random8Numbers() //대체 왜 되는 거지
    {
        int[] r8n = new int[8];

        for (int i = 0; i < 8; i++)
        {
            bool equal;
            do
            {
                equal = false;
                r8n[i] = Random.Range(0, 8);

                for (int j = 0; j < i; j++)
                {
                    if (r8n[j] == r8n[i]) equal = true;
                }
            }
            while (equal);
        }

        Debug.Log($"{r8n[0]},{r8n[1]},{r8n[2]},{r8n[3]},{r8n[4]},{r8n[5]},{r8n[6]},{r8n[7]}");
        return r8n;
    }



    void Awake()
    {
        boss1 = this;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        player = Player.player;
        //cam.orthographicSize = 12;
        //uicam.orthographicSize = 12;

        sr = GetComponent<SpriteRenderer>();

        hm = GameManager.hardmode;

        InvokeRepeating(nameof(BeforeSpawn), 4, hm ? 10 : 20);
        InvokeRepeating(nameof(Spawn), 5, hm ? 10 : 20);

        gauge = hm ? 160 : 80;
        for (int i = 2; i <= 4; i++)
            hpCASE.transform.GetChild(i).GetComponent<RectTransform>().localPosition = new(80 * (i - 3), -45);

        firstP = transform.position;

        Soundmanager.soundmanager.bossbgm[0].Play();

        GameManager.gameManager.OnEnable();
        player.OnEnable();
        PlayerAttack.playerAtk.OnEnable();
    }
    private void Update()
    {
        tp = transform.position;
        Vector2 pp = player.transform.position;
        dist = Vector2.Distance(tp, pp);

        H = gauge < (hm ? 32 : 16) || gauge > 320 - (hm ? 32 : 16) ? 0 : (tp.x > pp.x ? -pieceCount / 2.0f - 1 : pieceCount / 2.0f + 1);


        gauge -= (hm ? 32 : 16) * Time.deltaTime;
        if (gauge < 0) gauge = 320;

        hpCASE.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(gauge, 3);


        if (Mathf.Abs(player.transform.position.x - tp.x) < 0.5f && Mathf.Abs(player.transform.position.y - tp.y) < 3.5f) //끼임
            player.transform.position = new(tp.x, tp.y + 5);


        if (hp <= 240 && hp > 160)
        {
            //if (spons)
            //{
            //    GameObject knightsss = Instantiate(knight, transform.position, Quaternion.identity);
            //    knightsss.transform.SetParent(chessEmptySpace.transform);
            //    transform.position = new Vector2(0,0);
            //    spons = false;
            //}

            pieceCount = 1;
        }
        else if (hp <= 160 && hp > 80)
        {
            //if (!spons)
            //{
            //    GameObject bishopsss = Instantiate(bishop, transform.position, Quaternion.identity);
            //    bishopsss.transform.SetParent(chessEmptySpace.transform);
            //    transform.position = new Vector2(0, 0);
            //    spons = true;
            //}

            pieceCount = 2;
        }
        else if (hp <= 80 && hp > 0)
        {
            //if (spons)
            //{
            //    GameObject looksss = Instantiate(look, transform.position, Quaternion.identity);
            //    looksss.transform.SetParent(chessEmptySpace.transform);
            //    transform.position = new Vector2(0, 0);
            //    spons = false;
            //}

            pieceCount = 3;
        }
        if (hp <= 0)
        {
            //Destroy(gameObject);
            //GameManager.gameManager.NextStage();

            GameManager.gameManager.making = false;
            CancelInvoke(nameof(BeforeSpawn));
            CancelInvoke(nameof(Spawn));
            Destroy(chessEmptySpace);
            gameObject.SetActive(false);
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

            int r = Random.Range(0, 5);
            if (r < Player.player.purple)
            {
                hp--;
                Player.player.MakeEffect(new Vector2(tp.x, tp.y + 9), Player.player.critical, 5, 1);
            }
            if (Player.player.poison) RepeatAD();

            PlayerAttack.curAttackCooltime = 0;
        }
        //스킬 범위 내에 있음
        if (Mathf.Abs(PlayerAttack.skillP.y) < 200 &&
            Mathf.Abs(PlayerAttack.skillP.x - tp.x) < 5 && Mathf.Abs(PlayerAttack.skillP.y - tp.y) < 8)
        {
            Apa(Color.red);
            hp -= player.skillPower;

            int r = Random.Range(0, 5);
            if (r < Player.player.purple)
            {
                hp--;
                Player.player.MakeEffect(new Vector2(tp.x, tp.y + 9), Player.player.critical, 5, 1);
            }
            if (Player.player.poison) RepeatAD();
        }
        //무기 파생 스킬 범위 내에 있음
        Vector2 wsp = PlayerAttack.wsP;
        if (Mathf.Abs(wsp.y) < 200)
        {
            switch (PlayerAttack.weaponNum.Item1)
            {
                case 0:
                    bool inX = Mathf.Abs(wsp.x - tp.x) < 10f
                        && Mathf.Abs(wsp.y - tp.y) < 7;
                    bool inY = Mathf.Abs(wsp.y - tp.y) < 13.5f
                        && Mathf.Abs(wsp.x - tp.x) < 3.5f;
                    if (inX || inY)
                    {
                        Apa(Color.red);
                        hp -= Player.player.skillPower + 1;

                        int r = Random.Range(0, 5);
                        if (r < Player.player.purple)
                        {
                            hp--;
                            Player.player.MakeEffect(new Vector2(tp.x, tp.y + 9), Player.player.critical, 5, 1);
                        }
                        if (Player.player.poison) RepeatAD();
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

    } //Update End


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

            int r = Random.Range(0, 5);
            if (r < Player.player.purple)
            {
                hp--;
                Player.player.MakeEffect(new Vector2(tp.x, tp.y + 9), Player.player.critical, 5, 1);
            }
        }
    }

    public void RemovePollution() //오염 제거
    {
        if (polluted) pollution = 0;
        CancelInvoke(nameof(RemovePollution));
    }
    private void FixedUpdate()
    {
        transform.Translate(H * (hm ? 1.5f : 1) * (2 - pollution) * Time.deltaTime * Vector2.right);

    }

} //Boss1 End
