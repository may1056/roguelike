using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour //���� �Ѱ�
{
    public static int killed;
    public Text killText;

    void Start()
    {
        killed = 0;
    }


    void Update()
    {
        killText.text = killed.ToString() + " / 6";

        if (killed == 6) Debug.LogError("Ŭ����");
    }
} //GameManager End
