using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour //게임 총괄
{
    public static GameManager gameManager;

    public GameObject ShopSet;//상점 열고 닫/
    public Image Progress; //처음에 게임 진행 상황 알리기 위해
    float progressTime = 0; //4초까지 보여줄 거야
    public static bool prgEnd; //알려주는 거 끝났는지

    static int floor = 3; //몇 층
    static int stage = 4; //몇 스테이지

    Transform p_l; //point_line
    int dot; //반짝거릴 점 번호


    //ㅋㅋㅋㅋㅋㅋㅋㅋ
    static float 게임실행시간 = 0;
    public Text 게임실행시간텍스트;



    //아이템
    readonly string[] Items = { "알파 수정",
        "부활", "자동 공격", "자해", "쉴드", "버서커", "대쉬 강화",
        "붉은 수정", "분홍 수정", "푸른 수정", "초록 수정", "노란 수정", "주황 수정", "보라 수정", "독", };

    readonly int[] Items_legendary = { 2,
        1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0, }; //2: legend, 1: rare, 0: common

    //아이템별 확률
    //readonly float[] Items_p = { };

    //Items_legendary에 상수 곱해서 확률 만들기로 정해짐!!! 기억할 것


    public Text itemText;
    public GameObject itemCube;
    public Sprite[] itemSprites;
    int icnum; //아이템 큐브 번호, 즉 새로 들어온 아이템을 뜻함
    public GameObject itemChangeScreen;

    readonly string[] Items_explaination =
    {
        "모든 수정의 능력치를 갖습니다.", //알파 수정

        "사망 시 체력과 마나가 모두 충전된 상태로 부활합니다. (아이템 소멸)", //부활
        "적을 타겟팅하는 공격자를 소환합니다. 약하지만, 적을 감속시킵니다.", //자동 공격
        "크큭.. 왼손의 흑염룡이 미쳐 날뛰려 하는군.. 흑마법의 힘으로 모두 파.괴.해주겠어", //자해
        "피격 시 체력 대신 소모되는 방어막을 하나 더 갖습니다. 시간이 지나면 회복됩니다.", //쉴드
        "체력이 2 이하일 때 공격력이 1 증가합니다.", //버서커
        "대쉬 폼 미쳤다 ㄷㄷ", //대쉬 강화

        "무기의 공격력이 1 증가합니다.", //붉은 수정
        "체력 오브가 나타날 확률이 증가합니다.", //분홍 수정
        "마나 오브가 나타날 확률이 증가합니다.", //푸른 수정
        "이동 속도가 증가합니다.", //초록 수정
        "공격 속도가 증가합니다.", //노란 수정
        "낮은 확률로 공격을 회피합니다.", //주황 수정
        "낮은 확률로 치명타를 입힙니다.", //보라 수정
        "타격 시 랜덤 시간 이후 추가 데미지가 발생합니다.", //독
    };



    //무기


    readonly string[] meleeWeapon = // 근접 무기
        { "채찍", "검", "창",
        "열라짱짱 쎈 킹왕짱 울트라 슈퍼 매지컬 치즈스틱 롱치즈 이거 ㄹㅇ실화냐...", "방패" };

    readonly string[] rangedWeapon = // 원거리 무기
        { "활", "총지팡이"};

    //무기별 확률
    readonly float[] meleeWeapon_p = { 9.9f, 25, 20, 0.1f, 15 };
    readonly float[] rangedWeapon_p = { 15, 15 };

    // 현재 무기 코드
    public static int meleeWeaponIndex;

    public static int rangedWeaponIndex;

    public static bool ismeleeWeapon;
    public Text isMWText;


    public static int mapNum = 2; //맵 번호
    GameObject map; //맵이 들어가는 공간

    public GameObject[] maps; //맵 프리팹
    public GameObject[] mons; //맵 내 몬스터 집합 프리팹

    //맵별 페이즈 수
    readonly int[] phases = { 1, 1, 3, 3, 1, 1, 1, 1, };

    //페이즈별 잡아야 할 몬스터 수, -1: 안 잡아도 된다
    readonly int[,] enemies
        = { { 23, 0, 0, }, { -1, 0, 0, }, { 14, 15, 25, }, { 21, 25, 24, }, { 32, 0, 0, },
        { 14, 0, 0, }, { 19, 0, 0, }, { 22, 0, 0, }};

    public bool making; //진행 중인지
    int nowPhase; //현재 페이즈
    float phaseTime; //페이즈 진행 시간
    bool appeared; //적들 등장했는지



    public static bool mapouterror; //맵뚫 오류가 발생한 것 같다!
    float errortime;






    public GameObject Portal1; //쉬움, 보통
    public GameObject Portal2; //어려움, 기믹

    readonly float[,,] portal_position //맵별 포탈 위치
        = { { { 0, 0 }, { 999, 999 } }, { { 0, 5 }, { 999, 999 } }, { { -2, 1 }, { 2, 1 } }, { { 20, 1 }, { 26, 1 } }, { { -2, 0 }, { 2, 0 } },
        { { 4.5f, -15 }, { 13.5f, -15 } }, { { 2.5f, -0.5f }, { 7.5f, -0.5f } }, { { 31.5f, 1.5f }, { 36.5f, 1.5f } } };

    readonly int[,] portal_mapNum //포탈별 다음 맵 번호, -1은 포탈 X
        = { { 0, -1 }, { 2, -1 }, { 5, 3 }, { 6, 4 }, { 5, 6 },
        { 2, 3 }, { 5, 4 }, { 5, 2 } };







    public Player player;
    public PlayerAttack playerAtk;
    public Image[] hps = new Image[8]; //hp 구슬들
    public Image[] mps = new Image[6]; //mp 구슬들

    public Text[] stat = new Text[5];

    public GameObject menuSet;

    public static int killed = 0; //킬 수
    public Text killText;
    public static int realkilled; //실속 있는 킬 수

    public Text coolText; //쿨타임
    //public Text atkcoolText; //일반공격 쿨타임


    public static int coins = 0;
    public Text coinText;


    public Boss2 boss2;
    public Image bossHpLine;
    public Image Boss2WowWonderfulShit;
    public GameObject boss2map;





    void Awake()
    {
        gameManager = this;

        //아래는 Canvas의 PROGRESS 오브젝트 관련

        prgEnd = false; //안 끝났어
        Time.timeScale = 0; //멈춰
        Progress.gameObject.SetActive(true); //나타내

        //floor-stage는 텍스트메쉬프로를 사용해볼 것임. 예쁘기 때문
        Progress.rectTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text
            = floor.ToString() + " - " + stage.ToString(); //층 - 스테이지

        //point_line은 점과 선 집합이며, 각 점에는 같은 번호에 해당하는 선이 종속되어 있다.
        p_l = Progress.rectTransform.GetChild(1);

        dot = 0; //초기화

        for (int f = 1; f <= 3; f++) //floor
        {
            for (int s = 1; s <= (f == 1 ? 2 : 4); s++) //stage
            {
                //하하 각주 대체 뭐라 써야 되지. 아무튼 f와 s 가지고 0123456789 만들어낸다
                int n = f * (f - 1) - 1 + s;

                //진행 상황에 맞는 건 살리고, 아닌 건 죽인다
                if (f < floor)
                {
                    p_l.GetChild(n).GetComponent<Image>().color = Color.gray;
                    p_l.GetChild(n).gameObject.SetActive(true);
                    dot = n; //반응할 점 번호
                }
                else if (f == floor && s <= stage)
                {
                    p_l.GetChild(n).GetComponent<Image>().color = Color.white;
                    p_l.GetChild(n).gameObject.SetActive(true);
                    dot = n; //반응할 점 번호
                }
                else p_l.GetChild(n).gameObject.SetActive(false);

            }
        }

        player.transform.GetChild(0).GetComponent<Camera>().orthographicSize = 8;

    } //Awake End



    void Start()
    {
        if (transform.childCount != 0)
            Destroy(transform.GetChild(0).gameObject); //맵 남아있으면 삭제

        realkilled = 0;

        //맵 불러오기
        if (stage == 4) map = Instantiate(boss2map);
        else map = Instantiate(maps[mapNum]); //맵을 생성한다

        map.transform.SetParent(gameObject.transform); //게임매니저가 맵의 부모가 됨

        making = true;
        nowPhase = 0;
        phaseTime = 0;

        bossHpLine.gameObject.SetActive(stage == 4);

    } //Start End





    void EndProgress() //진행 화면 이제 그만 띄워라
    {
        prgEnd = true;
        Time.timeScale = 1;
        Progress.gameObject.SetActive(false);
    }


    void Update()
    {
        if (Input.GetKeyDown("o")) {
            if (ShopSet.activeSelf)
            {
                ShopSet.SetActive(false);
                Time.timeScale = 1f;
            }
              else
            {
                ShopSet.SetActive(true);
                Time.timeScale = 0;
            }
        }
        //상점 열고닫기


        if (progressTime > (stage == 4 ? 8 : 4) && !prgEnd) EndProgress();
        else progressTime += Time.unscaledDeltaTime; //TimeScale에 구애받지 않음


        ChangeHPMP();


        게임실행시간 += Time.deltaTime;
        게임실행시간텍스트.text = 게임실행시간.ToString();


        stat[0].text = "공격력: 공격력 변수 추가필요" /*+ Player.maxAttackCooltime.ToString()*/;
        stat[1].text = "공격속도: " + PlayerAttack.maxAttackCooltime.ToString();
        stat[2].text = "이동속도: " + Player.player.speed.ToString();
        stat[3].text = "점프력: " + Player.player.jumpPower.ToString();


        coolText.text = "쿨타임: " + playerAtk.cooltime.ToString("N0") + "초";

        //메뉴창 표시
        if (Input.GetButtonDown("Cancel") && progressTime > 4)
        {
            if (menuSet.activeSelf)
            {
                menuSet.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                menuSet.SetActive(true);
                Time.timeScale = 0;
            }
        }


        //킬 수 표시
        killText.text = killed.ToString();

        //획득 동전 수 표시
        coinText.text = coins.ToString();

        //빠른 재시작
        if (Input.GetKeyDown(KeyCode.Backspace) && progressTime > 4) SceneManager.LoadScene(1);

        if (ismeleeWeapon) isMWText.text = "원거리로 바꾸기";
        else isMWText.text = "근거리로 바꾸기";




        //적 불러오기
        if (making && progressTime > 4)
        {
            if (floor == 2 && stage == 4) //보스1
            {
            }
            else if (floor == 3 && stage == 4) //보스2
            {
                if (progressTime > 8)
                {
                    Boss2WowWonderfulShit.gameObject.SetActive(false);
                    boss2.gameObject.SetActive(true);
                }
                else Boss2WowWonderfulShit.gameObject.SetActive(true);

            }
            else //일반 스테이지
            {
                MakeEnemy(nowPhase);
                phaseTime += Time.deltaTime;

                if (phaseTime > 0.5f && !appeared)
                {
                    Transform set = map.transform.GetChild(nowPhase + 2);
                    for (int i = 0; i < set.childCount; i++)
                        set.GetChild(i).gameObject.SetActive(true);
                    appeared = true;
                }
            }
        }


        //맵 아웃 에러
        if (mapouterror && errortime > 0.02f) mapouterror = false;
        else errortime += Time.deltaTime;





        //전투가 끝났거나 다 잡을 필요 없으면 포탈들 정위치 후 보이기
        if (!making || enemies[mapNum, 0] == -1)
        {
            Portal1.transform.position = new Vector2(
                portal_position[mapNum, 0, 0], portal_position[mapNum, 0, 1]);
            Portal1.SetActive(true);

            Portal2.transform.position = new Vector2(
                portal_position[mapNum, 1, 0], portal_position[mapNum, 1, 1]);
            Portal2.SetActive(true);
        }

        GameObject P1S = Portal1.transform.GetChild(0).gameObject; //포탈1 S 텍스트
        GameObject P2S = Portal2.transform.GetChild(0).gameObject; //포탈2 S 텍스트

        //포탈1과 가까우면
        if (!making && Vector2.Distance(player.transform.position, Portal1.transform.position) < 2)
        {
            P1S.SetActive(true);
            P2S.SetActive(false);
            if (Input.GetKeyDown("s")) //포탈 타기
            {
                mapNum = portal_mapNum[mapNum, 0];
                NextStage();
            }
        }
        //포탈2와 가까우면
        else if (!making && Vector2.Distance(player.transform.position, Portal2.transform.position) < 2)
        {
            P1S.SetActive(false);
            P2S.SetActive(true);
            if (Input.GetKeyDown("s")) //포탈 타기
            {
                mapNum = portal_mapNum[mapNum, 1];
                NextStage();
            }
        }
        else //멀리 있다면
        {
            P1S.SetActive(false);
            P2S.SetActive(false);
        }



    } //Update End


    void NextStage()
    {
        player.SaveHP();
        playerAtk.SaveMP();

        stage++;

        if (floor == 1 && stage == 3) //1-3은 없으니 2-1로 가라
        {
            floor = 2;
            stage = 1;
        }
        else if (floor == 2 && stage == 5) //2-5는 없으니 3-1로 가라
        {
            floor = 3;
            stage = 1;
        }
        else if (floor == 3 && stage == 5) //3-5는 없... 막보를 죽였군! 잘했다
        {
            //끝
        }

        SceneManager.LoadScene(1); //PlayGame 재시작
    }



    public void ChangeHPMP() //hp, mp 구슬 최신화
    {
        for (int i = 0; i < 8; i++)
        {
            //HP
            if (i < player.hp)
            {
                hps[i].color = new Color(1, 0.7f, 0.9f);
                hps[i].gameObject.SetActive(true);
            }
            //SHIELD
            else if (i < player.hp + player.shield)
            {
                hps[i].color = Color.gray;
                hps[i].gameObject.SetActive(true);
            }
            else hps[i].gameObject.SetActive(false);
        }

        //MP
        for(int i = 0; i < playerAtk.maxmp; i++)
        {
            mps[i].color = Player.itemNum.Item1 == 3 || Player.itemNum.Item2 == 3 ?
                Color.black : new Color(0.3f, 0.3f, 0.8f); //자해 - 검정, 기본 - 마나 색
            mps[i].gameObject.SetActive(i < playerAtk.mp);
        }
    }


    void MakeEnemy(int ph) //적 프리팹 활성화
    {
        //지금 페이즈에 해당하는 몬스터 집합은 몇 번?
        int sum = 0;
        for (int i = 0; i < mapNum; i++) sum += phases[i];

        //전투 시작
        if (ph == 0) EnemySet(sum);

        //전투 종료
        else if (ph >= phases[mapNum] && realkilled == enemies[mapNum, ph - 1])
        {
            for (int i = 0; i < phases[mapNum]; i++)
            {
                Transform set = map.transform.GetChild(i + 3);
                Destroy(set.gameObject);
            }
            making = false;
            player.ClearBG();

        newItem: icnum = Random.Range(0, 15);
            if (icnum == Player.itemNum.Item1 || icnum == Player.itemNum.Item2)
                goto newItem; //goto 쓰지 말라고 했던 것 같긴 한데 아무튼, 겹치면 다시 뽑음

            GameObject ic = Instantiate(itemCube, Vector2.zero, Quaternion.identity);
            ic.GetComponent<Itemcube>().cubeNum = icnum;
            ic.GetComponent<SpriteRenderer>().sprite = itemSprites[icnum];
        }

        //다음 페이즈
        else if (realkilled == enemies[mapNum, ph - 1]) EnemySet(sum + ph);
    }

    void EnemySet(int dex)
    {
        GameObject mon = Instantiate(mons[dex]);
        mon.transform.SetParent(map.transform);
        nowPhase++;
        realkilled = 0;
        phaseTime = 0;
        appeared = false;
    }


    public void GameContinuetimeScale() // 계속하기 - 온 클릭용 함수
    {
        Time.timeScale = 1f;
    }

    public void SsipBug()
    {
        mapouterror = true;
        errortime = 0;
    }



    public void ItemInfo() //현재 무슨 아이템을 가지고 있나
    {
        int i1 = Player.itemNum.Item1, i2 = Player.itemNum.Item2;

        itemText.text = "1. " + (i1 == -1 ? "없음" : Items[i1]) +
            " 2. " + (i2 == -1 ? "없음" : Items[i2]);
    }

    public void ItemChangeHaseyo() //인벤토리 꽉 참! 템 버려!
    {
        Transform[] bg = new Transform[3]; //background
        Image[] it = new Image[3]; //item
        Text[] na = new Text[3]; //name
        Text[] ex = new Text[3]; //explaination

        for (int i = 0; i < 3; i++)
        {
            bg[i] = itemChangeScreen.transform.GetChild(i);
            it[i] = bg[i].GetChild(0).GetComponent<Image>();
            na[i] = bg[i].GetChild(0).GetChild(0).GetComponent<Text>();
            ex[i] = bg[i].GetChild(1).GetComponent<Text>();
        }

        it[0].sprite = itemSprites[Player.itemNum.Item1];
        na[0].text = Items[Player.itemNum.Item1];
        ex[0].text = Items_explaination[Player.itemNum.Item1];

        it[1].sprite = itemSprites[Player.itemNum.Item2];
        na[1].text = Items[Player.itemNum.Item2];
        ex[1].text = Items_explaination[Player.itemNum.Item2];

        it[2].sprite = itemSprites[icnum];
        na[2].text = Items[icnum];
        ex[2].text = Items_explaination[icnum];

        itemChangeScreen.SetActive(true);
    }

    public void ItemThrow(int code) //1: item1 버리기, 2: item2 버리기, 3: 새로운 아이템 안 먹기
    {
        int deleteNum = icnum; //버리는 템 번호 (할당하래서 아무거나 넣음;)

        switch (code)
        {
            case 1:
                deleteNum = Player.itemNum.Item1;
                Player.itemNum.Item1 = icnum;
                break;

            case 2:
                deleteNum = Player.itemNum.Item2;
                Player.itemNum.Item2 = icnum;
                break;

            case 3: deleteNum = icnum; break;
        }

        GameObject ic = Instantiate(itemCube,
            Player.player.transform.position, Quaternion.identity);
        ic.GetComponent<Itemcube>().cubeNum = deleteNum;
        ic.GetComponent<SpriteRenderer>().sprite = itemSprites[deleteNum];

        icnum = deleteNum;
        itemChangeScreen.SetActive(false);
    }






    public void GameExit() // 게임 종료 버튼 - 에디터에선 실행안됨
    {
        SceneManager.LoadScene(0);
    }

} //GameManager End
