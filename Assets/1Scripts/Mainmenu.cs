using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Mainmenu : MonoBehaviour // 게임매니저 사용하려했는데 씬 달라서 오브젝트 없다고 오류띄우길래 만듬
{
    public GameObject Option;
    AudioSource menusound;

    public static bool nevertutored = true;
    public Button nButton;

    public static bool cleared = false;
    public TextMeshProUGUI clearText;


    void Awake()
    {
        menusound = transform.GetChild(0).GetComponent<AudioSource>();
    }

    void Start()
    {
        //clearText.gameObject.SetActive(cleared); //버그 왜 나는지 모르겠네
    }

    public void GameExit() // 게임 종료 버튼
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    public void GameStart(bool tutorial) // 게임 시작 버튼
    {
        if (nevertutored && !tutorial) //튜토리얼 안 해봤는데 X 누름
        {
            nButton.gameObject.SetActive(false);
        }
        else
        {
            GameManager.shouldplaytutorial = tutorial;
            GameManager.killed = 0;
            GameManager.coins = 0;
            GameManager.atFirst = true;
            GameManager.게임실행시간 = 0;
            Player.itemNum = (-1, -1);
            PlayerAttack.weaponNum = (0, 1);
            Story.isEnding = false;

            SceneManager.LoadScene(4);
        }
    }

    public void MenuSound()
    {
        menusound.Play();
    }


} //Mainmenu End
