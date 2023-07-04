using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewWonderfulLeejonghwanShitWow : MonoBehaviour //메인메뉴 보조 (스토리씬에도 Mainmenu.cs가 있어서, 작동 오류 나는 건 여기로)
{
    Mainmenu mainmenu;

    public static int savedcoin;
    public Text coinText, itemText;

    public static bool[] locked = new bool[10]; //스테이지 잠겼는가? (진입해본 적 없는가?)
    public static int maxReachStage;
    public Button[] stageButtons;
    public static int selectedFloor = 1, selectedStage = 1;

    //아이템 미리 얻기
    public static bool[] itemOpen = new bool[15]; //아이템 해금되었는가? (얻어본 적 있는가?)
    public static int itemOpen_BinaryToDecimal; //itemOpen 배열을 이진법으로 생각하여 그것을 십진법으로 바꾼 수 (저장 용이하게 하기 위함)
    readonly int[] itemAmount = { 8, 6, 1 }; //등급별 아이템 양 - 일반, 희귀, 전설
    static int[] itemIndex = { 0, 0, 0 }; //진열된 동급 아이템 중 현재 번호 - 일반, 희귀, 전설
    public Image[] itemSet;
    public Sprite[] itemSprites;
    public Image owningItem1, owningItem2;

    public GameObject start;

    public static (int, int) shortestTime; //최단 시간 (분, 초)
    public static int clearCount, maxKill, maxCoin; //클리어 횟수, 최다 킬수, 코인 최대 획득

    public TextMeshProUGUI clearText;
    public Image recordWindow;



    void Start()
    {
        //메인메뉴와 상호작용을 위해 끌고 오기
        mainmenu = GetComponent<Mainmenu>();

        //클리어 시 인증 띄우기
        clearText.gameObject.SetActive(clearCount >= 1);

        //기록 불러오기
        LoadData();

        //스테이지 선택 화면 일단 끄기
        start.SetActive(false);

    } //Start End



    void Update()
    {
        coinText.text = savedcoin.ToString();


        if (GameManager.savedItem.Item1 == -1) owningItem1.gameObject.SetActive(false);
        else
        {
            owningItem1.GetComponent<Image>().sprite = itemSprites[GameManager.savedItem.Item1];
            owningItem1.gameObject.SetActive(true);
        }

        if (GameManager.savedItem.Item2 == -1) owningItem2.gameObject.SetActive(false);
        else
        {
            owningItem2.GetComponent<Image>().sprite = itemSprites[GameManager.savedItem.Item2];
            owningItem2.gameObject.SetActive(true);
        }

    } //Update End




    public void JumpToStage(int num) //num은 0~9 중 하나
    {
        if (savedcoin >= 10 * num && !locked[num])
        {
            //num으로 floor 계산
            selectedFloor = (num + 2) / 4 + 1;
            //num으로 stage 계산
            selectedStage = (num < 2 ? num : (num - 2) % 4) + 1;

            savedcoin -= 10 * num;
            mainmenu.GameStart();
        }
        else Debug.Log("안 열린 스테이지거나 돈 없음");
    }


    void ItemModify() //아이템 진열 상태 최신화
    {
        //아이템 이미지
        itemSet[0].transform.GetChild(0).GetComponent<Image>().sprite
            = itemSprites[itemIndex[0] + itemAmount[1] + itemAmount[2]];
        itemSet[1].transform.GetChild(0).GetComponent<Image>().sprite
            = itemSprites[itemIndex[1] + itemAmount[2]];
        itemSet[2].transform.GetChild(0).GetComponent<Image>().sprite
            = itemSprites[itemIndex[2]];

        //아이템 번호
        for (int i = 0; i < 3; i++)
        {
            itemSet[i].transform.GetChild(1).GetComponent<Text>().text
                = (itemIndex[i] + 1).ToString() + " / " + itemAmount[i].ToString();
        }

        //아이템 잠김 여부
        itemSet[0].transform.GetChild(2).gameObject.SetActive
            (!itemOpen[itemIndex[0] + itemAmount[1] + itemAmount[2]]);
        itemSet[1].transform.GetChild(2).gameObject.SetActive
            (!itemOpen[itemIndex[1] + itemAmount[2]]);
        itemSet[2].transform.GetChild(2).gameObject.SetActive
            (!itemOpen[itemIndex[2]]);
    }
    public void ItemListPrev(int legendary) //진열된 아이템을 이전 것으로 변경
    {
        if (itemIndex[legendary] > 0)
        {
            itemIndex[legendary]--;
            ItemModify();
        }
    }
    public void ItemListNext(int legendary) //진열된 아이템을 다음 것으로 변경
    {
        if (itemIndex[legendary] < itemAmount[legendary] - 1)
        {
            itemIndex[legendary]++;
            ItemModify();
        }
    }
    public void ItemBuy(int legendary) //아이템 구매
    {
        int num = 999;
        switch (legendary)
        {
            case 0: num = itemIndex[0] + itemAmount[1] + itemAmount[2]; break;
            case 1: num = itemIndex[1] + itemAmount[2]; break;
            case 2: num = itemIndex[2]; break;
        }

        if (savedcoin >= 10 * (legendary + 1) * (legendary + 1) //돈이 있는지
            && itemOpen[num] //아이템 잠금 열었는지
            && GameManager.savedItem.Item1 != num //아이템 안 겹치는지
            && GameManager.savedItem.Item2 == -1) //공간이 더 있는지
        {
            savedcoin -= 10 * (legendary + 1) * (legendary + 1);
            if (GameManager.savedItem.Item1 == -1) GameManager.savedItem.Item1 = num;
            else GameManager.savedItem.Item2 = num;
        }
    }



    public static void SaveWhenGameEnds()
    {
        PlayerPrefs.SetInt("SavedCoin", savedcoin);

        PlayerPrefs.SetInt("MaxReachStage", maxReachStage);

        itemOpen_BinaryToDecimal = 0;
        for (int i = itemOpen.Length - 1; i >= 0; i--)
        {
            if (itemOpen[i]) itemOpen_BinaryToDecimal += (int)Mathf.Pow(2, i);
        }
        PlayerPrefs.SetInt("ItemOpen_btd", itemOpen_BinaryToDecimal);
    }

    public void LoadData()
    {
        //기록 불러오기
        savedcoin = PlayerPrefs.GetInt("SavedCoin", 0);

        clearCount = PlayerPrefs.GetInt("ClearCount", 0);
        shortestTime = (PlayerPrefs.GetInt("ShortestTimeMinute", 9999), PlayerPrefs.GetInt("ShortestTimeSecond", 59));
        maxKill = PlayerPrefs.GetInt("MaxKill", -1);
        maxCoin = PlayerPrefs.GetInt("MaxCoin", -1);

        maxReachStage = PlayerPrefs.GetInt("MaxReachStage", 0);
        for (int i = 9; i >= 0; i--) locked[i] = maxReachStage < i;

        itemOpen_BinaryToDecimal = PlayerPrefs.GetInt("ItemOpen_btd", 0);
        int iO = itemOpen_BinaryToDecimal;
        for (int i = itemOpen.Length - 1; i >= 0; i--)
        {
            if (iO >= Mathf.Pow(2, i))
            {
                iO -= (int)Mathf.Pow(2, i);
                itemOpen[i] = true;
            }
            else itemOpen[i] = false;
        }
        Debug.Log(itemOpen_BinaryToDecimal);

        Mainmenu.nevertutored = PlayerPrefs.GetInt("int_NeverTutored", 1) == 1;
        Mainmenu.viewstory = PlayerPrefs.GetInt("int_ViewStory", 1) == 1;


        //스테이지 바로가기 버튼들의 하위 개체 관리
        for (int i = 1; i < 10; i++)
        {
            stageButtons[i].transform.GetChild(1).gameObject.SetActive(locked[i]); //자물쇠 보이기 여부
            stageButtons[i].transform.GetChild(2).gameObject.SetActive(!locked[i]); //비용 보이기 여부
        }


        //아이템 완전 초기화
        Player.itemNum = (-1, -1);
        GameManager.savedItem = (-1, -1);
        ItemModify();


        //옵션 변수가 스위치 상태랑 따로 노는 것을 방지
        for (int i = 0; i < 2; i++)
        {
            mainmenu.Option_Tutorial();
            mainmenu.Option_Story();
        }

    } //LoadData End

    public void ResetData()
    {
        itemIndex[0] = 0; itemIndex[1] = 0; itemIndex[2] = 0;

        PlayerPrefs.DeleteAll();
        LoadData();
    }



    public void ClearRecordWindowOnOff() //클리어 기록 보기 창
    {
        recordWindow.gameObject.SetActive(!recordWindow.gameObject.activeSelf);

        recordWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = clearCount.ToString();
        recordWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text
            = shortestTime.Item1 == 9999 & shortestTime.Item2 == 59 ? "-- : --" : shortestTime.Item1.ToString() + " : " + shortestTime.Item2.ToString();
        recordWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = maxKill.ToString();
    }


} //NewWonderfulLeejonghwanShitWow End
