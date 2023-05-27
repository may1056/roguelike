using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss2 : MonoBehaviour
{
    //두 번째 보스 - 자연의 섭리

    SpriteRenderer sr;

    float T; //주기
    float t; //시간

    public int hp;
    public Text hpText; //임시 체력 텍스트

    Player player;
    public GameObject cam;

    //pattern1
    public GameObject rain;
    float[] rainR = { 0.3f, 0.4f, 0.4f, 0.4f, 0.5f, 0.5f, 0.6f, 0.6f, 0.7f, 0.8f },
        rainG = { 0.6f, 0.6f, 0.7f, 0.8f, 0.7f, 0.8f, 0.7f, 0.8f, 0.8f, 0.9f };

    //pattern2
    public GameObject pxb3; //3 pixel bullet

    //pattern3
    public GameObject fadeEffect;
    public Sprite bigpx;
    int[] x = new int[6];
    int[] y = new int[6];
    int thenumberofsquares = 6;

    public Sprite Empty;




    Vector2 tp;
    bool inAttackArea; //플레이어의 공격 범위 내에 있는지
    public float pollution = 0; //오염 정도
    public bool polluted = false; //오염되었는지



    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        T = 15; //임시
        t = 0;

        player = Player.player;
        cam.GetComponent<Camera>().orthographicSize = 11;

        InvokeRepeating(nameof(Rain), 5, T);
        InvokeRepeating(nameof(LetBullet), 8, T);
        InvokeRepeating(nameof(Square), 12, T);
        InvokeRepeating(nameof(Square), 14, T);

        hp = 100; //임시
    }




    void Update()
    {
        cam.transform.position = -10 * Vector3.forward;

        t += 0.5f * Time.deltaTime;
        transform.position = 7.5f * new Vector2(Mathf.Cos(t), Mathf.Sin(t));






        //이하 Monster.cs에서 가져옴

        tp = transform.position;
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
            switch (Player.weaponNum)
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
                            Invoke(nameof(AfterDamage), Random.Range(1, 30));
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






    void Rain() //패턴1. 1px 비가 내린다
    {
        for (int i = 0; i < 20; i++)
            Invoke(nameof(RainMaker), i * 0.1f);
    }
    void RainMaker()
    {
        GameObject bi = Instantiate(rain, new Vector2(
            Random.Range(-189, 190) * 0.1f, 8.9f), Quaternion.identity);
        int n = Random.Range(0, 10);
        bi.GetComponent<SpriteRenderer>().color
            = new Color(rainR[n], rainG[n], 1); //푸른색
    }



    void LetBullet() //패턴2. 3px 탄막이 2px 탄막을 뿌리고 2px 탄막이 1px 탄막을 뿌림
    {
        for (int i = 0; i < 4; i++) Instantiate(pxb3,
                transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
    }



    void Square() //패턴3. 배경을 18분할하여 그중 6개 랜덤 선정 데미지
    {
        //x좌표는 -15, -9, -3, 3, 9, 15 중 하나, y좌표는 -6, 0, 6 중 하나
        for (int i = 0; i < thenumberofsquares; i++)
        {
            bool equal = true;
            while (equal)
            {
                equal = false;
                x[i] = Random.Range(0, 6) * 6 - 15;
                y[i] = Random.Range(-1, 2) * 6;
                for (int j = 0; j < i; j++)
                {
                    if (x[i] == x[j] && y[i] == y[j]) equal = true;
                }
            }
        }

        MakeSquare(0.5f, true, 1);
        Invoke(nameof(DeathSquare), 1);
    }
    void DeathSquare()
    {
        MakeSquare(1, false, 2);

        float px = player.transform.position.x, py = player.transform.position.y;
        bool area = false;
        for (int i = 0; i < thenumberofsquares; i++)
        {
            if (x[i] - 3 < px && px < x[i] + 3 && y[i] - 3 < py && py < y[i] + 3)
                area = true;
        }
        if (area && Player.unbeatableTime <= 0) Player.hurted = true;
    }
    void MakeSquare(float red, bool ud, int k)
    {
        for (int i = 0; i < thenumberofsquares; i++)
        {
            GameObject sq = Instantiate(fadeEffect,
                new Vector2(x[i], y[i]), Quaternion.identity);
            Fade sqf = sq.GetComponent<Fade>();
            sqf.up_down = ud;
            sqf.k = k;
            SpriteRenderer sqsr = sq.GetComponent<SpriteRenderer>();
            sqsr.sprite = bigpx;
            sqsr.color = new Color(red, 0, 0);
        }
    }






    public void RemovePollution() //오염 제거
    {
        if (polluted) pollution = 0;
        CancelInvoke(nameof(RemovePollution));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            inAttackArea = true; //들어간다
            Debug.Log("들어가");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            inAttackArea = false; //빠져나온다
    }
    public void Apa(Color c)
    {
        GameObject hurt = Instantiate(fadeEffect, transform.position,
            Quaternion.identity);

        SpriteRenderer hsr = hurt.transform.GetComponent<SpriteRenderer>();
        //hsr.flipX = sr.flipX;
        hsr.sortingOrder = 5;
        hsr.sprite = Empty;
        hsr.color = c;

        hurt.GetComponent<Fade>().k = 5;
    }
    void ModifyHp() //hp circle 최신화
    {
        hpText.text = hp.ToString();
    }
    public void RepeatAD() //AfterDamage() 반복
    {
        Invoke(nameof(AfterDamage), Random.Range(1, 30));
    }
    void AfterDamage() //poison 아이템 - Invoke용
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



} //Boss2 End
