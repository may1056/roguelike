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
    readonly string[] weapon =
        { "채찍", "검", "창", "활", "총지팡이",
        "열라짱짱 쎈 킹왕짱 울트라 슈퍼 매지컬 치즈스틱 롱치즈 이거 ㄹㅇ실화냐...", "방패" };
    //무기별 확률
    readonly float[] weapon_p = { 9.9f, 25, 20, 15, 15, 0.1f, 15 };



















    public Player player;
    public Image[] hps = new Image[6]; //hp 구슬들
    public Image[] mps = new Image[6]; //mp 구슬들

    public Text[] stat = new Text[5];

    public int enemies; //적들 수

    public static int killed; //킬 수
    public Text killText;

    public Text coolText; //쿨타임
    //public Text atkcoolText; //일반공격 쿨타임

    public Text respawnText;


    void Start()
    {
        killed = 0;
        ChangeHPMP();
    }


    void Update()
    {
        stat[0].text = "공격력: 공격력 변수 추가필요" /*+ Player.maxAttackCooltime.ToString()*/;
        stat[1].text = "공격속도: " + Player.maxAttackCooltime.ToString();
        stat[2].text = "이동속도: " + Player.player.speed.ToString();
        stat[3].text = "점프력: " + Player.player.jumpPower.ToString();
        stat[4].text = "대시속도: " + Player.player.dashSpeed.ToString();


        coolText.text = "쿨타임: " + player.cooltime.ToString("N0") + "초";

        //킬 수 표시
        killText.text = killed.ToString() + " / " + enemies.ToString();

        //if (killed == enemies) Debug.LogWarning("클리어");

        //빠른 재시작
        if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(0);

        //원위치 - 버그 나면 억울하니까 임시방편으로
        if (player.respawnCool > 0) respawnText.text =
                "리스폰 활성화까지 " + player.respawnCool.ToString("N0") + "초";
        else respawnText.text = "리스폰 가능 (r키)";

    } //Update End


    public void ChangeHPMP() //hp, mp 구슬 최신화
    {
        for(int i = 0; i < 6; i++)
        {
            hps[i].gameObject.SetActive(i < player.hp);
            mps[i].gameObject.SetActive(i < player.mp);
        }
    }

} //GameManager End
