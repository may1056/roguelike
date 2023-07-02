using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Mainmenu : MonoBehaviour // 게임매니저 사용하려했는데 씬 달라서 오브젝트 없다고 오류띄우길래 만듬
{
    public GameObject Option;
    public Button[] Switches;
    public Sprite switchOn, switchOff;

    AudioSource menusound;

    public static bool
        nevertutored = true, //옵션0 - 튜토리얼 보는가?
        viewstory = true; //옵션1 - 스토리 보는가?

    public static bool cleared = false;
    public TextMeshProUGUI clearText;


    void Awake()
    {
        menusound = transform.GetChild(0).GetComponent<AudioSource>();
    }

    void Start()
    {
        Time.timeScale = 1;
    }


    public void GameExit() // 게임 종료 버튼
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    public void GameStart() // 게임 시작 버튼
    {
        GameManager.shouldplaytutorial = nevertutored;
        GameManager.killed = 0;
        GameManager.coins = 0;
        GameManager.atFirst = true;
        GameManager.게임실행시간 = 0;
        Player.itemNum = (-1, -1);
        PlayerAttack.weaponNum = (0, 1);
        Story.isEnding = false;

        SceneManager.LoadScene(viewstory ? 4 : 1);
    }

    public void MenuSound()
    {
        menusound.Play();
    }



    public void Cleared________________________________________()
    {
        clearText.gameObject.SetActive(cleared);
    }



    //옵션

    public void Option_Tutorial()
    {
        nevertutored = !nevertutored;
        Switches[0].image.sprite = nevertutored ? switchOn : switchOff;
    }

    public void Option_Story()
    {
        viewstory = !viewstory;
        Switches[1].image.sprite = viewstory ? switchOn : switchOff;
    }


} //Mainmenu End
