using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour //���� �Ѱ�
{
    public Player player;
    public Image[] hps = new Image[6]; //hp ������

    public int enemies; //���� ��

    public static int killed; //ų ��
    public Text killText;

    void Start()
    {
        killed = 0;
        ChangeHP();
    }


    void Update()
    {
        //ų �� ǥ��
        killText.text = killed.ToString() + " / " + enemies.ToString();

        if (killed == enemies) Debug.LogWarning("Ŭ����");

        //���� �����
        if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(0);
    }


    public void ChangeHP() //hp ���� �ֽ�ȭ
    {
        for(int i = 0; i < 6; i++)
        {
            bool v = i < player.hp;
            hps[i].gameObject.SetActive(v);
        }
    }

} //GameManager End
