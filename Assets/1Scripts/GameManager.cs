using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour //게임 총괄
{
    public static GameManager gameManager;

    public static bool hardmode;

    public static bool shouldplaytutorial = false;
    public static bool atFirst = true;

    public GameObject ShopSet;//상점 열고 닫/
    public Image Progress; //처음에 게임 진행 상황 알리기 위해
    float progressTime = 0; //4초까지 보여줄 거야
    public static bool prgEnd; //알려주는 거 끝났는지

    static int floor = 1; //몇 층
    static int stage = 1; //몇 스테이지

    Transform p_l; //point_line
    int dot; //반짝거릴 점 번호
    Transform nowOn;


    //ㅋㅋㅋㅋㅋㅋㅋㅋ
    public static float 게임실행시간 = 0;
    public Text 게임실행시간텍스트;

    public bool onTabPage;
    public Image tabPage;


    public Image death;



    //아이템
    readonly string[] Items = { "알파 수정",
        "부활", "자동 공격", "자해", "추가 쉴드", "버서커", "대쉬 강화",
        "붉은 수정", "분홍 수정", "푸른 수정", "초록 수정", "노란 수정", "주황 수정", "보라 수정", "독", };

    readonly int[] Items_legendary = { 2,
        1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 0, 0, 0, }; //2: legend, 1: rare, 0: common

    //아이템별 확률
    //readonly float[] Items_p = { };

    //Items_legendary에 상수 곱해서 확률 만들기로 정해짐!!! 기억할 것


    GameObject nowCube;

    public Text itemText;
    public GameObject itemCube;
    public Sprite[] itemSprites;
    int icnum; //아이템 큐브 번호, 즉 새로 들어온 아이템을 뜻함
    public GameObject itemChangeScreen;
    public Image itemGet;
    bool getto = false;
    public Image item1Image, item2Image;
    public Image item1tabImage, item2tabImage;
    public Button item1tabButton, item2tabButton;
    public Image item1Info, item2Info;
    public Text item1destroyCoin, item2destroyCoin;

    readonly string[] Items_explaination =
    {
        "모든 수정의 능력치를 갖습니다. 단, 다른 수정의 효과는 무시합니다.", //알파 수정

        "사망 시 체력과 마나가 모두 충전된 상태로 부활합니다. (아이템 소멸)", //부활
        "적을 타겟팅하는 공격자를 소환합니다. 약하지만, 적을 감속시킵니다.", //자동 공격
        "크큭.. 왼손의 흑염룡이 미쳐 날뛰려 하는군.. 흑마법의 힘으로 모두 파.괴.해주겠어 체력 1 되고 엄청 강해짐, 적 처치 시 체력 오브 안 나옴", //자해
        "피격 시 체력 대신 소모되는 방어막을 하나 더 갖습니다. 그리고 무적 시간이 길어집니다.", //쉴드
        "체력이 2 이하일 때 공격력이 2 증가합니다. 적 처치 시 체력 오브가 등장하지 않습니다.", //버서커
        "대쉬 폼 미쳤다 ㄷㄷ", //대쉬 강화

        "무기의 공격력이 1 증가합니다.", //붉은 수정
        "적 처치 시 체력 오브가 나타날 확률이 증가합니다.", //분홍 수정
        "적 처치 시 마나 오브가 나타날 확률이 증가합니다.", //푸른 수정
        "이동 속도가 증가합니다.", //초록 수정
        "공격 속도가 증가합니다.", //노란 수정
        "낮은 확률로 공격을 회피합니다.", //주황 수정
        "낮은 확률로 치명타를 입힙니다.", //보라 수정
        "타격 시 랜덤 시간 이후 추가 데미지가 발생합니다.", //독
    };



    //무기


    //readonly string[] meleeWeapon = // 근접 무기
    //    { "채찍", "검", "창",
    //    "열라짱짱 쎈 킹왕짱 울트라 슈퍼 매지컬 치즈스틱 롱치즈 이거 ㄹㅇ실화냐...", "방패" };

    //readonly string[] rangedWeapon = // 원거리 무기
    //    { "활", "총지팡이"};

    readonly string[] Weapons = // 무기 통합
        { "검", "불안정한\n총지팡이", "창", "활",
        "열라짱짱 쎈 킹왕짱 울트라 슈퍼 매지컬 치즈스틱 롱치즈 이거 ㄹㅇ실화냐..." };


    public GameObject weaponCube;
    public Sprite[] weaponSprites;
    int wcnum;
    public GameObject weaponChangeScreen;
    public Image weaponGet;
    bool getto_w = false;
    public Image nowWeaponImage, subWeaponImage;
    public Image nowWeaponTabImage, subWeaponTabImage;
    public Button subWeaponTabButton;
    public Image nowWeaponInfo, subWeaponInfo;

    readonly string[] Weapons_explaination =
    {
        "짧습니다.",
        "제어하기 힘듭니다.",
        "찌릅니다.",
        "쏩니다.",
        "ㅇㅁㄴ류ㅓ팧ㅊㅍㄴㅁㄴㅇ치테코윺닏ㄹ먼ㄼㅈㄷ로변ㅇㅎㅁㅅㄴㅇㅊㄱ머나히ㅑㅡㅐ도갸ㅕㄹㄹ나외며ㅑㅈ돞비ㅓ어ㅐ렘ㄴ"
    };

    //무기별 확률
    readonly float[] meleeWeapon_p = { 9.9f, 25, 20, 0.1f, 15 };
    readonly float[] rangedWeapon_p = { 15, 15 };

    // 현재 무기 코드
    public static int meleeWeaponIndex;

    public static int rangedWeaponIndex;

    public readonly bool[] ismelee = { true, false, true, false, true };
    public static bool ismeleeWeapon;
    public Text isMWText;




    public static int mapNum; //맵 번호
    GameObject map; //맵이 들어가는 공간

    public GameObject[] maps; //맵 프리팹
    public GameObject[] mons; //맵 내 몬스터 집합 프리팹

    //맵별 페이즈 수
    public int[] phases;

    //페이즈별 잡아야 할 몬스터 수, -1: 안 잡아도 된다
    readonly int[,] enemies
        = { { 14, 0, 0, }, { 19, 0, 0, }, { 11, 0, 0, },{ 16, 0, 0, },{ 10, 4, 0, },
        { 14, 15, 25, }, { 21, 25, 14, },{ 22, 0, 0, },{ 19, 0, 0, },{ 11, 0, 0, },
        { 15, 0, 0, }, { 10, 23, 0, },{ 8, 0, 0, },{ 3, 0, 0, },{ 16, 12, 0, },{ 21, 0, 0, }};

    public bool making; //진행 중인지
    int nowPhase; //현재 페이즈
    float phaseTime; //페이즈 진행 시간
    bool appeared; //적들 등장했는지


    static int[] exceptionMaps = new int[8]; //이미 나온 맵 번호
    static int exceptionCount = 0;


    public static bool mapouterror; //맵뚫 오류가 발생한 것 같다!
    float errortime;






    public GameObject Portal1; //쉬움, 보통
    public GameObject Portal2; //어려움, 기믹

    //readonly float[,,] portal_position //맵별 포탈 위치
    //    = { { { 0, 0 }, { 999, 999 } }, { { 0, 5 }, { 999, 999 } }, { { -2, 1 }, { 2, 1 } }, { { 20, 1 }, { 26, 1 } }, { { -2, 0 }, { 2, 0 } },
    //    { { 4.5f, -15 }, { 13.5f, -15 } }, { { 2.5f, -0.5f }, { 7.5f, -0.5f } }, { { 31.5f, 1.5f }, { 36.5f, 1.5f } } };

    //readonly int[,] portal_mapNum //포탈별 다음 맵 번호, -1은 포탈 X
    //    = { { 0, -1 }, { 2, -1 }, { 5, 3 }, { 6, 4 }, { 5, 6 },
    //    { 2, 3 }, { 5, 4 }, { 5, 2 } };

    public Sprite[] portalSprites; //0: Easy, 1: Hard, 2: Boss, 3: Shop

    public ParticleSystem nearbyplayerps;
    public ParticleSystem.MainModule nearbypsmain;




    public Player player;
    public PlayerAttack playerAtk;
    public Image[] hps = new Image[8]; //hp 구슬들
    public Image[] mps = new Image[6]; //mp 구슬들

    public ParticleSystem[] hps_ps, mps_ps; //원래 잘못 만든 건데 그냥 하트들 위치 확인용으로 남겨둠
    public ParticleSystem hp_ps, sh_ps, mp_ps, mp_ps_si; //이게 진짜임
    public ParticleSystemRenderer hp_ps_renderer, sh_ps_renderer, mp_ps_renderer;

    public Text[] stat = new Text[5];

    public GameObject menuSet;

    public static int killed = 0; //킬 수
    public TextMeshProUGUI killText;
    public static int realkilled; //실속 있는 킬 수
    public ParticleSystem killps;


    public static int coins = 0;
    public TextMeshProUGUI coinText;
    public ParticleSystem coinps;


    public Image bossHpLine;

    public Image Boss1WowWonderfulShit;
    public Boss1 boss1;
    public GameObject boss1map;

    public Image Boss2WowWonderfulShit;
    public Boss2 boss2;
    public GameObject boss2map;



    public Camera uicam;


    public Image hpPotion, mpPotion;

    public float pstimerhp;//hp postion
    public float pstimermp;//mp postion

    public ParticleSystem usable1ps, use1ps, usable2ps, use2ps;



    public AudioSource recover;


    public GameObject TutorialMap;
    public Image Read;
    public Image skillZ, skillX;

    public Image nowWeapon;

    public static (int, int) savedItem;





    readonly string[] tips =
    {
        "분홍 하트는 체력, 푸른 하트는 마나입니다.",
        "회색 하트는 쉴드입니다. 안 맞고 안 때리면 찹니다.",
        "플레이어 위의 흰 점들은 스태미나입니다. 대쉬할 때마다 소모됩니다.",
        "설정에서 튜토리얼 또는 스토리를 건너뛰도록 할 수 있습니다.",
        "아이템 파괴 시 코인을 일정량 획득합니다.",
        "무기의 공격 범위는 마우스가 향하는 쪽으로 넓습니다.",
        "몇몇 스킬은 적이 발사한 탄막을 제거합니다.",
        "적을 처치하면 체력 오브, 마나 오브, 코인이 확률적으로 등장합니다.",
    };
    public Text tipText;


    public Image[] keys;





    void Awake()
    {
        gameManager = this;

        if (atFirst)
        {
            floor = NewWonderfulLeejonghwanShitWow.selectedFloor;
            stage = NewWonderfulLeejonghwanShitWow.selectedStage;
            Player.itemNum = savedItem;
            exceptionCount = 0;
            killed = 0;
            coins = 0;
        }

        player.transform.GetChild(0).GetComponent<Camera>().orthographicSize = 8;
        uicam.orthographicSize = 8;

        onTabPage = false;


        //아래는 Canvas의 PROGRESS 오브젝트 관련

        prgEnd = false; //안 끝났어
        Time.timeScale = 0; //멈춰
        Progress.gameObject.SetActive(true); //나타내

        //floor-stage는 텍스트메쉬프로를 사용해볼 것임. 예쁘기 때문
        Progress.rectTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text
            = floor.ToString() + " - " + stage.ToString(); //층 - 스테이지

        //point_line은 점과 선 집합이며, 각 점에는 같은 번호에 해당하는 선이 종속되어 있다.
        p_l = Progress.rectTransform.GetChild(2);

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
                    p_l.GetChild(n).GetComponent<Image>().color
                        = new Color(0.2f, 0.2f, 0.2f);
                    if (n > 0) p_l.GetChild(n).GetChild(0).GetComponent<Image>().color
                            = new Color(0.2f, 0.2f, 0.2f);
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

        nowOn = Progress.rectTransform.GetChild(1);
        nowOn.transform.position = p_l.GetChild(dot).position;

        RectTransform nort = nowOn.GetComponent<RectTransform>();
        if (stage == 4) nort.sizeDelta = new Vector2(50, 50);
        else nort.sizeDelta = new Vector2(35, 35);

        tipText.text = tips[Random.Range(0, tips.Length)]; //팁도 같이 띄움

    } //Awake End



    void Start()
    {
        if (atFirst)
        {
            atFirst = false;
            player.hp = 6;
            playerAtk.mp = 6;
        }

        if (transform.childCount != 0)
            Destroy(transform.GetChild(0).gameObject); //맵 남아있으면 삭제

        realkilled = 0;

        making = true;
        nowPhase = 0;
        phaseTime = 0;


        bool mn = true;
        while (mn)
        {
            switch (floor)
            {
                case 1:
                    mapNum = Random.Range(0, 5); // 0~4
                    break;
                case 2:
                    mapNum = Random.Range(5, 10); // 5~9
                    break;
                case 3:
                    mapNum = Random.Range(10, 16); // 10~15
                    break;
            }
            if (exceptionCount == 0) mn = false;
            else
            {
                bool ex = false;
                for (int i = 0; i < exceptionCount; i++)
                {
                    if (exceptionMaps[i] == mapNum) ex = true;
                }
                mn = ex;
            }
        }
        Debug.Log(mapNum);
        //mapNum = Random.Range(0, 10);
        //mapNum += 1;


        //맵 불러오기
        if (stage == 4) map = Instantiate(floor == 2 ? boss1map : boss2map);
        else if (shouldplaytutorial)
        {
            skillZ.gameObject.SetActive(false);
            skillX.gameObject.SetActive(false);
            map = Instantiate(TutorialMap);
            making = false;
            hpPotion.gameObject.SetActive(false);
            mpPotion.gameObject.SetActive(false);
            //nowWeapon.transform.GetChild(1).gameObject.SetActive(false);
            //nowWeapon.transform.GetChild(2).gameObject.SetActive(false);
            progressTime = 4.01f;
        }
        else
        {
            map = Instantiate(maps[mapNum]);
            exceptionMaps[exceptionCount] = mapNum;
            exceptionCount++;
        }

        map.transform.SetParent(gameObject.transform); //게임매니저가 맵의 부모가 됨


        bossHpLine.gameObject.SetActive(stage == 4);


        //배경 설정
        switch (floor * (floor - 1) - 1 + stage)
        {
            case 0: case 1: player.StartBG(1); break; //배경1 : 1-1, 1-2
            case 2: case 3: case 4: player.StartBG(2); break; //배경2 : 2-1, 2-2, 2-3
            case 6: case 7: case 8: player.StartBG(3); break; //배경3 : 3-1, 3-2, 3-3
            case 5: player.StartBG(4); break; //배경4 : 보스1 (2-4)
            case 9: player.StartBG(5); break; //배경5 : 보스2 (3-4)
        }
        if (shouldplaytutorial) player.StartBG(0); //배경0 : 튜토리얼


        //진입해본 스테이지는 잠금해제
        if (NewWonderfulLeejonghwanShitWow.maxReachStage < floor * (floor - 1) - 1 + stage)
            NewWonderfulLeejonghwanShitWow.maxReachStage = floor * (floor - 1) - 1 + stage;


        //키 관련
        for (int i = 0; i < keys.Length; i++) keys[i].gameObject.SetActive(Mainmenu.markkey);
        InvokeRepeating(nameof(KeyHighlight), 0, 0.3f);


        nearbypsmain = nearbyplayerps.GetComponent<ParticleSystem>().main;

    } //Start End


    public void OnEnable()
    {
        SizeUI(usable1ps); SizeUI(use1ps); SizeUI(usable2ps); SizeUI(use2ps);
        SizeUI(killps); SizeUI(coinps);
        SizeUI(hp_ps); SizeUI(sh_ps); SizeUI(mp_ps); SizeUI(mp_ps_si);
    }
    void SizeUI(ParticleSystem ps) //UI 파티클시스템 크기가 카메라 사이즈에 따라 들쑥날쑥이라 비율 맞춤
    {
        Vector2 uipsv = new(uicam.orthographicSize / 12, uicam.orthographicSize / 12);
        ps.GetComponent<RectTransform>().localScale = uipsv;
    }


    void EndProgress() //진행 화면 이제 그만 띄워라
    {
        prgEnd = true;
        Time.timeScale = 1;
        Progress.gameObject.SetActive(false);
    }


    void Update()
    {
        if (progressTime > (stage == 4 ? 8 : 4) && !prgEnd) EndProgress();
        else progressTime += Time.unscaledDeltaTime; //TimeScale에 구애받지 않음

        nowOn.GetComponent<Image>().color = new Color(1, 0, 0, progressTime % 1);


        //개발자 핵 - "종"
        if ((Input.GetKey("z") || Input.GetKey("j"))
            && Input.GetKey("o") && Input.GetKey("n") && Input.GetKeyDown("g")

            && !Input.GetKey("p") && !Input.GetKey("r") && !Input.GetKey("k") && !Input.GetKey("i")
            && !Input.GetKey("c") && !Input.GetKey("y") && !Input.GetKey("u"))
        {
            coins += 50;
            NextStage();
        }


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

        pstimerhp += Time.deltaTime;
        if (15 <= pstimerhp)
        {
            if (Input.GetKeyDown("1")
                && player.hp < player.maxhp && coins >= 10)
            {
                player.hp += 3;
                coins -= 10;
                pstimerhp = 0;
                ReadOn(4, 0);
                use1ps.Play();
            }
        }
        hpPotion.transform.GetChild(0).GetComponent<Image>().fillAmount
            = (15 - pstimerhp) / 15;
        hpPotion.transform.GetChild(1).GetComponent<Text>().text
            = pstimerhp < 15 ? (15 - pstimerhp).ToString("N1") : null;
        usable1ps.gameObject.SetActive(15 <= pstimerhp && coins >= 10 && (
            !shouldplaytutorial || hpPotion.gameObject.activeSelf));

        pstimermp += Time.deltaTime;
        if (15 <= pstimermp)
        {
            if (Input.GetKeyDown("2")
                && playerAtk.mp < playerAtk.maxmp && coins >= 10)
            {
                playerAtk.mp += 3;
                coins -= 10;
                pstimermp = 0;
                ReadOn(4, 1);
                use2ps.Play();
            }
        }
        mpPotion.transform.GetChild(0).GetComponent<Image>().fillAmount
            = (15 - pstimermp) / 15;
        mpPotion.transform.GetChild(1).GetComponent<Text>().text
            = pstimermp < 15 ? (15 - pstimermp).ToString("N1") : null;
        usable2ps.gameObject.SetActive(15 <= pstimermp && coins >= 10 && (
            !shouldplaytutorial || mpPotion.gameObject.activeSelf));




        ChangeHPMP();


        if (!boss2.worldEnder.activeSelf) 게임실행시간 += Time.deltaTime;
        게임실행시간텍스트.text = ((int)(게임실행시간 / 60)).ToString() + ":" + ((int)(게임실행시간 % 60)).ToString("D2");

        if (Input.GetKeyDown(KeyCode.Tab) && progressTime > 4)
        {
            onTabPage = !onTabPage;
            ItemInfo();
            WeaponInfo();
            ReadOn(3, 1);
        }

        tabPage.gameObject.SetActive(onTabPage);


        stat[0].text = "공격력: 공격력 변수 추가필요" /*+ Player.maxAttackCooltime.ToString()*/;
        stat[1].text = "공격속도: " + PlayerAttack.maxAttackCooltime.ToString();
        stat[2].text = "이동속도: " + player.speed.ToString();
        stat[3].text = "점프력: " + player.jumpPower.ToString();


        //메뉴창 표시
        if (Input.GetButtonDown("Cancel") && progressTime > (stage == 4 ? 8 : 4)
            && !death.gameObject.activeSelf && !boss2.worldEnder.activeSelf)
        {
            if (menuSet.activeSelf)
            {
                menuSet.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                menuSet.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text
                    = shouldplaytutorial ? "Tutorial" : floor.ToString() + " - " + stage.ToString();
                menuSet.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text
                    = hardmode ? "HARD" : "EASY";
                menuSet.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color
                    = hardmode ? new Color(1, 0.6f, 0.6f) : new Color(0.6f, 0.7f, 1);
                menuSet.SetActive(true);
                Time.timeScale = 0;
            }

            ReadOn(3, 0);
        }

        if (menuSet.activeSelf)
        {
            if (Input.GetKeyDown("r"))
            {
                SsipBug();
                menuSet.SetActive(false);
                Time.timeScale = 1f;
            }

            if (Input.GetKeyDown("q")) GameExit();
        }


        //킬 수 표시
        killText.text = killed.ToString();

        //획득 동전 수 표시
        coinText.text = coins.ToString();

        //빠른 재시작
        //if (Input.GetKeyDown(KeyCode.Backspace) && progressTime > 4) SceneManager.LoadScene(1);

        if (ismeleeWeapon) isMWText.text = "원거리로 바꾸기";
        else isMWText.text = "근거리로 바꾸기";




        //적 불러오기
        if (making && progressTime > 4)
        {
            if (floor == 2 && stage == 4) //보스1
            {
                if (progressTime > 8)
                {
                    Boss1WowWonderfulShit.gameObject.SetActive(false);
                    boss1.gameObject.SetActive(true);
                }
                else Boss1WowWonderfulShit.gameObject.SetActive(true);

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
            SpriteRenderer portalsr = Portal1.GetComponent<SpriteRenderer>();

            if (shouldplaytutorial) //1-1이 아니라 튜토리얼로 처리하기로 했음!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            {
                Portal1.transform.position = new Vector2(115.5f, 1);
                portalsr.sprite = portalSprites[0];
                Portal1.SetActive(true);
            }
            else
            {
                Portal1.transform.position = Vector2.up;
                portalsr.sprite = portalSprites[stage == 3 ? 2 : (hardmode ? 1 : 0)];
                Portal1.SetActive(true);

                //Portal2.transform.position = new Vector2(
                //    portal_position[mapNum, 1, 0], portal_position[mapNum, 1, 1]);
                //Portal2.SetActive(true);
            }

            Color portalpsColor = stage == 3 ? new Color(0.7f, 0, 0) : (hardmode ? new Color(1, 0.6f, 0.6f) : new Color(0.6f, 0.7f, 1));

            for (int i = 1; i <= 2; i++)
            {
                ParticleSystem.MainModule portalmain = Portal1.transform.GetChild(i).GetComponent<ParticleSystem>().main;
                portalmain.startColor = portalpsColor;
            }

            nearbyplayerps.gameObject.SetActive(true);
            nearbypsmain.startColor = new Color(portalpsColor.r, portalpsColor.g, portalpsColor.b, 0.3f);

            //ParticleSystem.MainModule nearbyaroundpsmain = nearbyplayerps.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            //nearbyaroundpsmain.startColor = portalpsColor;
        }

        GameObject P1S = Portal1.transform.GetChild(0).gameObject; //포탈1 S 텍스트
        //GameObject P2S = Portal2.transform.GetChild(0).gameObject; //포탈2 S 텍스트

        //포탈1과 가까우면
        if (!making && Vector2.Distance(player.transform.position, Portal1.transform.position) < 2)
        {
            P1S.SetActive(Mainmenu.markkey);
            //P2S.SetActive(false);

            if (Input.GetKeyDown("s")) //포탈 타기
            {
                if (shouldplaytutorial)
                {
                    Read.gameObject.SetActive(true);
                    hpPotion.gameObject.SetActive(true);
                    mpPotion.gameObject.SetActive(true);
                    nowWeapon.transform.GetChild(1).gameObject.SetActive(true);
                    nowWeapon.transform.GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    //mapNum = portal_mapNum[mapNum, 0];
                    NextStage();
                }
            }

            //포탈 근접 파티클 효과
            Portal1.transform.GetChild(2).gameObject.SetActive(true);
            nearbypsmain.loop = true;
            nearbyplayerps.Play();
            //nearbyplayerps.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            P1S.SetActive(false);

            Portal1.transform.GetChild(2).gameObject.SetActive(false);
            nearbypsmain.loop = false;
            //nearbyplayerps.transform.GetChild(0).gameObject.SetActive(false);
        }

        //포탈2와 가까우면
        //else if (!making && Vector2.Distance(player.transform.position, Portal2.transform.position) < 2)
        //{
        //    P1S.SetActive(false);
        //    P2S.SetActive(true);
        //    if (Input.GetKeyDown("s")) //포탈 타기
        //    {
        //        mapNum = portal_mapNum[mapNum, 1];
        //        NextStage();
        //    }
        //}
        //else //멀리 있다면
        //{
        //    P1S.SetActive(false);
        //    P2S.SetActive(false);
        //}


        if (Input.GetKeyDown("e"))
        {
            if (getto) Equip();
            else if (getto_w) Equip_w();
        }
        if (Input.GetKeyDown("f"))
        {
            if (getto) Throw();
            else if (getto_w) Throw_w();
        }


        //아이템이 3개면 파괴 좀 하라고 TAB키 하이라이트
        keys[5].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0, 0,
            Player.itemNum.Item1 != -1 && Player.itemNum.Item2 != -1 && nowCube != null ? 1 : 0);

    } //Update End



    public void KillPlus()
    {
        realkilled++;
        killed++;
        killps.Play();
    }
    public void CoinPlus(bool orb)
    {
        if (orb) coins++;
        Player.player.pickupcoin.Play();
        coinps.Play();
    }


    public void NextStage()
    {
        player.SaveHP();
        playerAtk.SaveMP();

        stage++;

        if (shouldplaytutorial)
        {
            floor = NewWonderfulLeejonghwanShitWow.selectedFloor;
            stage = NewWonderfulLeejonghwanShitWow.selectedStage;
            shouldplaytutorial = false;
            //Mainmenu.nevertutored = false;
            SceneManager.LoadScene(1); //PlayGame 재시작
        }
        else if (floor == 1 && stage == 3) //1-3은 없으니 2-1로 가라
        {
            floor = 2;
            stage = 1;
            SceneManager.LoadScene(1); //PlayGame 재시작
        }
        else if (floor == 2 && stage == 5) //2-5는 없으니 3-1로 가라
        {
            floor = 3;
            stage = 1;
            SceneManager.LoadScene(1); //PlayGame 재시작
        }
        else if (floor == 3 && stage == 5) //3-5는 없... 막보를 죽였군! 잘했다
        {
            Story.isEnding = true;
            SceneManager.LoadScene(Mainmenu.viewstory ? 4 : 3);
        }
        else SceneManager.LoadScene(1);
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

        if (player.hp > 0)
        {
            ParticleSystem.MainModule hppsmain = hp_ps.main;
            hppsmain.startLifetime = 0.7f + 0.3f * (player.hp - 1);
            hppsmain.startSpeed = 0.2f + 0.6f * (player.hp - 1);
            hp_ps_renderer.lengthScale = 3.0f + 1.2f * (player.hp - 1);

            ParticleSystem.MainModule shpsmain = sh_ps.main;

            if (player.shield <= 0) shpsmain.startLifetime = 0;
            else
            {
                sh_ps.transform.position = hps_ps[player.hp].transform.position;

                shpsmain.startLifetime = 1.0f + 0.5f * player.shield;
                shpsmain.startSpeed = -0.6f + 0.8f * player.shield;
                sh_ps_renderer.lengthScale = 1.0f + 2.0f * player.shield;
            }
        }


        //MP
        for(int i = 0; i < playerAtk.maxmp; i++)
        {
            mps[i].color = Player.itemNum.Item1 == 3 || Player.itemNum.Item2 == 3 ?
                Color.black : new Color(0.5f, 0.5f, 1); //자해 - 검정, 기본 - 마나 색
            mps[i].gameObject.SetActive(i < playerAtk.mp);
        }

        ParticleSystem.MainModule mppsmain = mp_ps.main;
        ParticleSystem.MainModule mppssimain = mp_ps_si.main;

        if (Player.itemNum.Item1 == 3 || Player.itemNum.Item2 == 3) //자해
        {
            mppsmain.startLifetime = 0;
            mppssimain.startLifetime = 2.2f;
        }
        else if (playerAtk.mp > 0)
        {
            mppsmain.startLifetime = 0.7f + 0.3f * (playerAtk.mp - 1);
            mppsmain.startSpeed = 0.2f + 0.6f * (playerAtk.mp - 1);
            mp_ps_renderer.lengthScale = 3.0f + 1.2f * (playerAtk.mp - 1);
            mppssimain.startLifetime = 0;
        }
        else
        {
            mppsmain.startLifetime = 0;
            mppssimain.startLifetime = 0;
        }

    } //ChangeHPMP End




    void MakeEnemy(int ph) //적 프리팹 활성화
    {
        //지금 페이즈에 해당하는 몬스터 집합은 몇 번?
        int sum = 0;
        for (int i = 0; i < mapNum; i++) sum += phases[i];

        //전투 시작
        if (ph == 0) EnemySet(sum);

        //전투 종료
        else if (ph >= phases[mapNum] && realkilled >= enemies[mapNum, ph - 1])
        {
            for (int i = 0; i < phases[mapNum]; i++)
            {
                Transform set = map.transform.GetChild(i + 3);
                Destroy(set.gameObject);
            }
            making = false;
            player.ClearBG();

            if (Random.Range(0, 3) >= 0) //3분의 2 확률로 아이템큐브 //일 생각이었는데 무기 제작이 제대로 안 돼서 일단 아이템만
            {
            newItem: icnum = Random.Range(0, 15);
                if (icnum == Player.itemNum.Item1 || icnum == Player.itemNum.Item2)
                    goto newItem; //겹치면 다시 뽑음

                nowCube = Instantiate(itemCube, 2.5f * Vector2.right, Quaternion.identity);
                nowCube.GetComponent<Itemcube>().cubeNum = icnum;
                nowCube.GetComponent<SpriteRenderer>().sprite = itemSprites[icnum];
            }
            else //3분의 1 확률로 무기큐브
            {
            newWeapon: wcnum = Random.Range(0, 4);
                if (wcnum == PlayerAttack.weaponNum.Item1 || wcnum == PlayerAttack.weaponNum.Item2)
                    goto newWeapon; //겹치면 다시 뽑음

                nowCube = Instantiate(weaponCube, 2.5f * Vector2.right, Quaternion.identity);
                nowCube.GetComponent<Weaponcube>().cubeNum = wcnum;
                nowCube.GetComponent<SpriteRenderer>().sprite = weaponSprites[wcnum];
            }
        }

        //다음 페이즈
        else if (realkilled >= enemies[mapNum, ph - 1]) EnemySet(sum + ph);

    } //MakeEnemy End

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


    public void ReadOn(int set, int key)
    {
        Image read = Read.transform.GetChild(set).GetChild(key).GetComponent<Image>();
        read.color = new Color(1, 1, 1, 0.4f);
        read.transform.GetChild(0).GetComponent<Text>().color = new Color(0, 0, 0, 0.4f);
    }


    public void ItemInfo() //현재 무슨 아이템을 가지고 있나
    {
        int i1 = Player.itemNum.Item1, i2 = Player.itemNum.Item2;

        //itemText.text = "1. " + (i1 == -1 ? "없음" : Items[i1]) +
        //    " 2. " + (i2 == -1 ? "없음" : Items[i2]);

        if (i1 == -1)
        {
            item1Image.gameObject.SetActive(false);

            item1tabImage.gameObject.SetActive(false);
            item1tabButton.gameObject.SetActive(false);
        }
        else
        {
            item1Image.sprite = itemSprites[i1];
            item1Image.gameObject.SetActive(true);

            item1tabImage.sprite = itemSprites[i1];
            item1tabImage.gameObject.SetActive(true);
            item1tabButton.gameObject.SetActive(true);

            item1destroyCoin.text = "+" + ((Items_legendary[i1] + 1) * (Items_legendary[i1] + 1) * 5).ToString();
        }

        if (i2 == -1)
        {
            item2Image.gameObject.SetActive(false);

            item2tabImage.gameObject.SetActive(false);
            item2tabButton.gameObject.SetActive(false);
        }
        else
        {
            item2Image.sprite = itemSprites[i2];
            item2Image.gameObject.SetActive(true);

            item2tabImage.sprite = itemSprites[i2];
            item2tabImage.gameObject.SetActive(true);
            item2tabButton.gameObject.SetActive(true);

            item2destroyCoin.text = "+" + ((Items_legendary[i2] + 1) * (Items_legendary[i2] + 1) * 5).ToString();
        }
    }
    public void WeaponInfo() //현재 무슨 무기를 가지고 있나
    {
        int w1 = PlayerAttack.weaponNum.Item1, w2 = PlayerAttack.weaponNum.Item2;

        nowWeaponImage.sprite = weaponSprites[w1];
        nowWeaponTabImage.sprite = weaponSprites[w1];

        if (w2 == -1)
        {
            subWeaponImage.gameObject.SetActive(false);

            subWeaponTabImage.gameObject.SetActive(false);
            //subWeaponTabButton.gameObject.SetActive(false);
        }
        else
        {
            subWeaponImage.sprite = weaponSprites[w2];
            subWeaponImage.gameObject.SetActive(true);

            subWeaponTabImage.sprite = weaponSprites[w2];
            subWeaponTabImage.gameObject.SetActive(true);
            //subWeaponTabButton.gameObject.SetActive(true);
        }
    }


    public void ItemChangeHaseyo() //인벤토리 꽉 참! 템 버려!
    {
        Transform[] bg = new Transform[3]; //background
        Image[] it = new Image[3]; //item
        Text[] na = new Text[3]; //name
        Text[] ex = new Text[3]; //explaination
        Text[] le = new Text[3]; //legendary

        for (int i = 0; i < 3; i++)
        {
            bg[i] = itemChangeScreen.transform.GetChild(i);
            it[i] = bg[i].GetChild(0).GetComponent<Image>();
            na[i] = bg[i].GetChild(0).GetChild(0).GetComponent<Text>();
            ex[i] = bg[i].GetChild(1).GetComponent<Text>();
            le[i] = bg[i].GetChild(2).GetComponent<Text>();
        }

        int i1 = Player.itemNum.Item1;
        it[0].sprite = itemSprites[i1];
        na[0].text = Items[i1];
        ex[0].text = Items_explaination[i1];
        le[0].text = Items_legendary[i1] == 2 ? "전설" : (Items_legendary[i1] == 1 ? "희귀" : "일반");
        le[0].color = Items_legendary[i1] == 2 ? new Color(1, 0.7f, 0) : (Items_legendary[i1] == 1 ? new Color(0.5f, 0.4f, 1) : new Color(0.8f, 0.3f, 0));

        int i2 = Player.itemNum.Item2;
        it[1].sprite = itemSprites[i2];
        na[1].text = Items[i2];
        ex[1].text = Items_explaination[i2];
        le[1].text = Items_legendary[i2] == 2 ? "전설" : (Items_legendary[i2] == 1 ? "희귀" : "일반");
        le[1].color = Items_legendary[i2] == 2 ? new Color(1, 0.7f, 0) : (Items_legendary[i2] == 1 ? new Color(0.5f, 0.4f, 1) : new Color(0.8f, 0.3f, 0));

        it[2].sprite = itemSprites[icnum];
        na[2].text = Items[icnum];
        ex[2].text = Items_explaination[icnum];
        le[2].text = Items_legendary[icnum] == 2 ? "전설" : (Items_legendary[icnum] == 1 ? "희귀" : "일반");
        le[2].color = Items_legendary[icnum] == 2 ? new Color(1, 0.7f, 0) : (Items_legendary[icnum] == 1 ? new Color(0.5f, 0.4f, 1) : new Color(0.8f, 0.3f, 0));

        itemChangeScreen.SetActive(true);
    }

    public void WeaponChangeHaseyo() //인벤토리 꽉 참! 무기 버려!
    {
        Transform[] bg = new Transform[3]; //background
        Image[] we = new Image[3]; //weapon
        Text[] na = new Text[3]; //name
        Text[] ex = new Text[3]; //explaination

        for (int i = 0; i < 3; i++)
        {
            bg[i] = weaponChangeScreen.transform.GetChild(i);
            we[i] = bg[i].GetChild(0).GetComponent<Image>();
            na[i] = bg[i].GetChild(0).GetChild(0).GetComponent<Text>();
            ex[i] = bg[i].GetChild(1).GetComponent<Text>();
        }

        we[0].sprite = weaponSprites[PlayerAttack.weaponNum.Item1];
        na[0].text = Weapons[PlayerAttack.weaponNum.Item1];
        ex[0].text = Weapons_explaination[PlayerAttack.weaponNum.Item1];

        we[1].sprite = weaponSprites[PlayerAttack.weaponNum.Item2];
        na[1].text = Weapons[PlayerAttack.weaponNum.Item2];
        ex[1].text = Weapons_explaination[PlayerAttack.weaponNum.Item2];

        we[2].sprite = weaponSprites[wcnum];
        na[2].text = Weapons[wcnum];
        ex[2].text = Weapons_explaination[wcnum];

        weaponChangeScreen.SetActive(true);
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

        if (deleteNum != -1)
        {
            nowCube = Instantiate(itemCube, Player.player.transform.position, Quaternion.identity);
            nowCube.GetComponent<Itemcube>().cubeNum = deleteNum;
            nowCube.GetComponent<SpriteRenderer>().sprite = itemSprites[deleteNum];

            icnum = deleteNum;
        }

        itemChangeScreen.SetActive(false);

        ItemInfo();

        getto = false;

        player.GetNewItem();
    }

    public void WeaponThrow(int code)
    {
        int deleteNum = wcnum;

        switch (code)
        {
            case 1:
                deleteNum = PlayerAttack.weaponNum.Item1;
                PlayerAttack.weaponNum.Item1 = wcnum;
                break;

            case 2:
                deleteNum = PlayerAttack.weaponNum.Item2;
                PlayerAttack.weaponNum.Item2 = wcnum;
                break;

            case 3: deleteNum = wcnum; break;
        }

        GameObject wc = Instantiate(weaponCube,
            Player.player.transform.position, Quaternion.identity);
        wc.GetComponent<Weaponcube>().cubeNum = deleteNum;
        wc.GetComponent<SpriteRenderer>().sprite = weaponSprites[deleteNum];

        wcnum = deleteNum;
        weaponChangeScreen.SetActive(false);

        WeaponInfo();

        getto_w = false;
    }


    public void ItemGettodaje()
    {
        Image it; //item
        Text na; //name
        Text ex; //explaination
        Text le; //legendary

        it = itemGet.transform.GetChild(0).GetComponent<Image>();
        na = it.transform.GetChild(0).GetComponent<Text>();
        ex = itemGet.transform.GetChild(1).GetComponent<Text>();
        le = itemGet.transform.GetChild(2).GetComponent<Text>();

        it.sprite = itemSprites[icnum];
        na.text = Items[icnum];
        ex.text = Items_explaination[icnum];
        le.text = Items_legendary[icnum] == 2 ? "전설" : (Items_legendary[icnum] == 1 ? "희귀" : "일반");
        le.color = Items_legendary[icnum] == 2 ? new Color(1, 0.7f, 0) : (Items_legendary[icnum] == 1 ? new Color(0.5f, 0.4f, 1) : new Color(0.8f, 0.3f, 0));

        itemGet.gameObject.SetActive(true);
        getto = true;

        player.GetNewItem();
        ReadOn(1, 0);
    }

    public void WeaponGettodaje()
    {
        Image we; //weapon
        Text na; //name
        Text ex; //explaination

        we = weaponGet.transform.GetChild(0).GetComponent<Image>();
        na = we.transform.GetChild(0).GetComponent<Text>();
        ex = weaponGet.transform.GetChild(1).GetComponent<Text>();

        we.sprite = weaponSprites[wcnum];
        na.text = Weapons[wcnum];
        ex.text = Weapons_explaination[wcnum];

        weaponGet.gameObject.SetActive(true);
        getto_w = true;

        playerAtk.GetNewWeapon();
        ReadOn(1, 0);
    }


    public void Throw()
    {
        itemGet.gameObject.SetActive(false);

        ItemThrow(3);
    }
    public void Equip()
    {
        itemGet.gameObject.SetActive(false);
        getto = false;

        if (Player.itemNum.Item1 == -1) //아이템1에 넣음
        {
            Player.itemNum.Item1 = icnum;
            ItemInfo();
        }
        else if (Player.itemNum.Item2 == -1) //아이템2에 넣음
        {
            Player.itemNum.Item2 = icnum;
            ItemInfo();
        }
        else ItemChangeHaseyo();

        player.GetNewItem();
        ReadOn(1, 0);
    }

    public void Throw_w()
    {
        weaponGet.gameObject.SetActive(false);

        WeaponThrow(3);
    }
    public void Equip_w()
    {
        weaponGet.gameObject.SetActive(false);
        getto_w = false;

        if (PlayerAttack.weaponNum.Item2 == -1)
        {
            PlayerAttack.weaponNum.Item2 = wcnum;
            WeaponInfo();
        }
        else WeaponChangeHaseyo();

        playerAtk.GetNewWeapon();
        ReadOn(1, 0);
    }


    public void ItemWeaponSell(int n) //파괴
    {
        switch (n)
        {
            case 1:
                coins += (int)Mathf.Pow(Items_legendary[Player.itemNum.Item1] + 1, 2) * 5;
                Player.itemNum.Item1 = -1;
                player.GetNewItem();
                break;

            case 2:
                coins += (int)Mathf.Pow(Items_legendary[Player.itemNum.Item2] + 1, 2) * 5;
                Player.itemNum.Item2 = -1;
                player.GetNewItem();
                break;

            case 3:
                coins += 40;
                PlayerAttack.weaponNum.Item2 = -1;
                playerAtk.GetNewWeapon();
                break;
        }

        CoinPlus(false);
    }
    public void ItemWeaponInfo() // ( i )
    {
        int i1 = Player.itemNum.Item1,
            i2 = Player.itemNum.Item2,
            w1 = PlayerAttack.weaponNum.Item1,
            w2 = PlayerAttack.weaponNum.Item2;

        if (i1 != -1)
        {
            item1Info.transform.GetChild(0).GetComponent<Image>().sprite
                = itemSprites[i1];
            item1Info.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
                = Items[i1];
            item1Info.transform.GetChild(1).GetComponent<Text>().text
                = Items_explaination[i1];
            item1Info.transform.GetChild(2).GetComponent<Text>().text
                = Items_legendary[i1] == 2 ? "전설" : (Items_legendary[i1] == 1 ? "희귀" : "일반");
            item1Info.transform.GetChild(2).GetComponent<Text>().color
                = Items_legendary[i1] == 2 ? new Color(1, 0.7f, 0) : (Items_legendary[i1] == 1 ? new Color(0.5f, 0.4f, 1) : new Color(0.8f, 0.3f, 0));
        }

        if (i2 != -1)
        {
            item2Info.transform.GetChild(0).GetComponent<Image>().sprite
                = itemSprites[i2];
            item2Info.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
                = Items[i2];
            item2Info.transform.GetChild(1).GetComponent<Text>().text
                = Items_explaination[i2];
            item2Info.transform.GetChild(2).GetComponent<Text>().text
                = Items_legendary[i2] == 2 ? "전설" : (Items_legendary[i2] == 1 ? "희귀" : "일반");
            item2Info.transform.GetChild(2).GetComponent<Text>().color
                = Items_legendary[i2] == 2 ? new Color(1, 0.7f, 0) : (Items_legendary[i2] == 1 ? new Color(0.5f, 0.4f, 1) : new Color(0.8f, 0.3f, 0));
        }

        nowWeaponInfo.transform.GetChild(0).GetComponent<Image>().sprite
            = weaponSprites[w1];
        nowWeaponInfo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
            = Weapons[w1];
        nowWeaponInfo.transform.GetChild(1).GetComponent<Text>().text
            = Weapons_explaination[w1];

        if (w2 != -1)
        {
            subWeaponInfo.transform.GetChild(0).GetComponent<Image>().sprite
                = weaponSprites[w2];
            subWeaponInfo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text
                = Weapons[w2];
            subWeaponInfo.transform.GetChild(1).GetComponent<Text>().text
                = Weapons_explaination[w2];
        }
    }



    void KeyHighlight()
    {
        keys[5].transform.GetChild(0).gameObject.SetActive(!keys[5].transform.GetChild(0).gameObject.activeSelf);
    }



    public void GameExit() // 게임 종료 버튼
    {
        NewWonderfulLeejonghwanShitWow.SaveWhenGameEnds();
        SceneManager.LoadScene(0);
    }

} //GameManager End
