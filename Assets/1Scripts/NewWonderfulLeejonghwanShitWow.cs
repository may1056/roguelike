using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewWonderfulLeejonghwanShitWow : MonoBehaviour //메인메뉴 보조 (스토리씬에도 Mainmenu.cs가 있어서, 작동 오류 나는 건 여기로)
{
    Mainmenu mainmenu;

    public static int savedcoin = 0;
    public Text coinText, itemText;

    public static bool[] locked = { false, true, true, true, true, true, true, true, true, true };
    public Button[] stageButtons;

    public static int selectedFloor = 1, selectedStage = 1;



    private void Start()
    {
        mainmenu = GetComponent<Mainmenu>();
        mainmenu.Cleared________________________________________();
        mainmenu.Option_Tutorial(); mainmenu.Option_Tutorial(); //nevertutored 변수가 스위치 상태랑 따로 노는 것을 방지

        //아이템 완전 초기화
        Player.itemNum = (-1, -1);
        GameManager.savedItem = (-1, -1);

        //스테이지 바로가기 버튼들의 하위 개체 관리
        for (int i = 1; i < 10; i++)
        {
            stageButtons[i].transform.GetChild(1).gameObject.SetActive(locked[i]); //자물쇠 보이기 여부
            stageButtons[i].transform.GetChild(2).gameObject.SetActive(!locked[i]); //비용 보이기 여부
        }
    }


    void Update()
    {
        coinText.text = savedcoin.ToString();

        if (GameManager.savedItem.Item2 != -1) itemText.text = "아이템1, 아이템2 장착됨";
        else if (GameManager.savedItem.Item1 != -1) itemText.text = "아이템1 장착됨";
        else itemText.text = null;
    }



    public void BuyRandomItem()
    {
        if (savedcoin >= 50)
        {
            if (GameManager.savedItem.Item1 == -1)
            {
                GameManager.savedItem.Item1 = Random.Range(0, 15);
                savedcoin -= 50;
            }
            else if (GameManager.savedItem.Item2 == -1)
            {
                do GameManager.savedItem.Item2 = Random.Range(0, 15);
                while (GameManager.savedItem.Item1 == GameManager.savedItem.Item2);
                savedcoin -= 50;
            }
        }
    }


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


} //NewWonderfulLeejonghwanShitWow End
