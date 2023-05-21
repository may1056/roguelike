using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//SceneManagement
using UnityEngine.SceneManagement;

public class Shopin : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("shop"); // 전환하고자 하는 화면의 이름을 ""에 넣어준다.
    }
}
