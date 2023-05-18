using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour //게임 총괄
{
    //아이템
    readonly string[] legendItems = { "알파 수정" };
    readonly string[] rareItems =
        { "부활 아이템", "자동 공격", "자해", "버서커", "강한 대쉬", "극진공수도 비급",};
    readonly string[] commonItems =
        { "붉은 수정", "hp색 수정", "초록 수정", "노란 수정", "푸른 수정", "주황 수정", "독" };

    //아이템별 확률
    readonly float[] legendItems_p = { 0.1f };
    readonly float[] rareItems_p = { 3, 3, 3, 3, 3, 3 };
    readonly float[] commonItems_p = { 10, 10, 10, 10, 10, 10, 10, };

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


    public static int mapNum; //맵 번호
    GameObject map; //맵이 들어가는 공간

    public GameObject[] maps; //맵 프리팹
    public GameObject[] mons; //맵 내 몬스터 집합 프리팹

    //맵별 페이즈 수
    readonly int[] phases = { 1, 1, 3, 3, };

    //페이즈별 잡아야 할 몬스터 수, -1: 안 잡아도 된다
    readonly int[,] enemies = { { 23, 0, 0, }, { -1, 0, 0, }, { 14, 15, 25, }, { 21, 25, 24, } };

    public bool making; //진행 중인지
    int nowPhase; //현재 페이즈
    float phaseTime; //페이즈 진행 시간
    bool appeared; //적들 등장했는지



    public static bool mapouterror; //맵뚫 오류가 발생한 것 같다!
    float errortime;






    public GameObject Portal1; //쉬움, 보통
    public GameObject Portal2; //어려움, 기믹

    readonly Vector2[,] portal_position //맵별 포탈 위치
        = { { new Vector2(0, 0), new Vector2(999, 999) }, { new Vector2(0, 5), new Vector2(999, 999) }, { new Vector2(-2, 0), new Vector2(2, 0) }, { new Vector2(20, 1), new Vector2(26, 1) } };

    readonly int[,] portal_mapNum //포탈별 다음 맵 번호, -1은 포탈 X
        = { { 0, -1 }, { 2, -1 }, { 3, 1 }, { 2, 1 } };







    public Player player;
    public PlayerAttack playerAtk;
    public Image[] hps = new Image[8]; //hp 구슬들
    public Image[] mps = new Image[6]; //mp 구슬들

    public Text[] stat = new Text[5];

    public GameObject menuSet;

    public static int killed; //킬 수
    public Text killText;
    public static int realkilled; //실속 있는 킬 수

    public Text coolText; //쿨타임
    //public Text atkcoolText; //일반공격 쿨타임


    public static int coins = 0;
    public Text coinText;




    void Start()
    {
        if (transform.childCount != 0)
            Destroy(transform.GetChild(0).gameObject); //맵 남아있으면 삭제

        killed = 0;

        coins = 0;

        mapNum = 3; //임시

        realkilled = 0;

        //맵 불러오기
        map = Instantiate(maps[mapNum]); //맵을 생성한다
        map.transform.SetParent(gameObject.transform); //게임매니저가 맵의 부모가 됨

        making = true;
        nowPhase = 0;
        phaseTime = 0;

    } //Start End


    void Update()
    {
        stat[0].text = "공격력: 공격력 변수 추가필요" /*+ Player.maxAttackCooltime.ToString()*/;
        stat[1].text = "공격속도: " + PlayerAttack.maxAttackCooltime.ToString();
        stat[2].text = "이동속도: " + Player.player.speed.ToString();
        stat[3].text = "점프력: " + Player.player.jumpPower.ToString();


        coolText.text = "쿨타임: " + playerAtk.cooltime.ToString("N0") + "초";

        //메뉴창 표시
        if (Input.GetButtonDown("Cancel"))
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
        if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(1);

        if (ismeleeWeapon) isMWText.text = "원거리로 바꾸기";
        else isMWText.text = "근거리로 바꾸기";




        //적 불러오기
        if (making)
        {
            MakeEnemy(nowPhase);
            phaseTime += Time.deltaTime;

            if (phaseTime > 0.5f && !appeared)
            {
                Transform set = map.transform.GetChild(nowPhase + 1);
                for (int i = 0; i < set.childCount; i++)
                    set.GetChild(i).gameObject.SetActive(true);
                appeared = true;
            }
        }


        //맵 아웃 에러
        if (mapouterror && errortime > 0.02f) mapouterror = false;
        else errortime += Time.deltaTime;





    } //Update End



    public void ChangeHPMP() //hp, mp 구슬 최신화
    {
        for (int i = 0; i < 8; i++)
        {
            //HP
            if (i < player.hp)
            {
                hps[i].color = Color.white;
                hps[i].gameObject.SetActive(true);
            }
            //SHIELD
            else if (i < player.hp + player.shield)
            {
                hps[i].color = Color.black;
                hps[i].gameObject.SetActive(true);
            }
            else hps[i].gameObject.SetActive(false);
        }

        //MP
        for(int i = 0; i < playerAtk.maxmp; i++)
            mps[i].gameObject.SetActive(i < playerAtk.mp);
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
                Transform set = map.transform.GetChild(i + 2);
                Destroy(set.gameObject);
            }
            making = false;
            player.ClearBG();
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

    public void GameExit() // 게임 종료 버튼 - 에디터에선 실행안됨
    {
        SceneManager.LoadScene(0);
    }

} //GameManager End
