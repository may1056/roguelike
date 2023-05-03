using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour //게임 총괄
{
    public Player player;
    public Image[] hps = new Image[6]; //hp 구슬들

    public static int killed;
    public Text killText;

    void Start()
    {
        killed = 0;
    }


    void Update()
    {
        killText.text = killed.ToString() + " / 6";

        if (killed == 6) Debug.LogError("클리어");

        //빠른 재시작
        if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(0);
    }


    public void ChangeHP() //hp 구슬 최신화
    {
        for(int i = 0; i < 6; i++)
        {
            bool v = i < player.hp;
            hps[i].gameObject.SetActive(v);
        }
    }

} //GameManager End
