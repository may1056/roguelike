using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour // 게임매니저 사용하려했는데 씬 달라서 오브젝트 없다고 오류띄우길래 만듬
{
    public GameObject Option;

    public void GameExit() // 게임 종료 버튼 - 에디터에선 실행안됨
    {
        Application.Quit();
    }

    public void GameStart() // 게임 시작 버튼
    {
        SceneManager.LoadScene("SampleScene");
    }
}
