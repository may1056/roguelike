using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//SceneManagement
using UnityEngine.SceneManagement;

public class Shopout: MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Play Game"); // 전환하고자 하는 화면의 이름을 ""에 넣어준다.
    }
}
