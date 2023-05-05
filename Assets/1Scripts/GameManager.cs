using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour //게임 총괄
{
    public Player player;
    public Image[] hps = new Image[6]; //hp 구슬들
    public Image[] mps = new Image[6]; //mp 구슬들

    public int enemies; //적들 수

    public static int killed; //킬 수
    public Text killText;

    public Text coolText; //쿨타임
    public Text atkcoolText; //일반공격 쿨타임


    void Start()
    {
        killed = 0;
        ChangeHPMP();
    }


    void Update()
    {
        atkcoolText.text = "쿨타임: " + Player.attackCooltime.ToString("N0") + "초";
        coolText.text = "쿨타임: " + player.cooltime.ToString("N0") + "초";

        //킬 수 표시
        killText.text = killed.ToString() + " / " + enemies.ToString();

        if (killed == enemies) Debug.LogWarning("클리어");

        //빠른 재시작
        if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(0);
    }


    public void ChangeHPMP() //hp, mp 구슬 최신화
    {
        for(int i = 0; i < 6; i++)
        {
            hps[i].gameObject.SetActive(i < player.hp);
            mps[i].gameObject.SetActive(i < player.mp);
        }
    }

} //GameManager End
