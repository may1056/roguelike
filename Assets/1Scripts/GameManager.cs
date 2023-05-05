using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour //���� �Ѱ�
{
    public Player player;
    public Image[] hps = new Image[6]; //hp ������
    public Image[] mps = new Image[6]; //mp ������

    public int enemies; //���� ��

    public static int killed; //ų ��
    public Text killText;

    public Text coolText; //��Ÿ��
    public Text atkcoolText; //�Ϲݰ��� ��Ÿ��


    void Start()
    {
        killed = 0;
        ChangeHPMP();
    }


    void Update()
    {
        atkcoolText.text = "��Ÿ��: " + Player.attackCooltime.ToString("N0") + "��";
        coolText.text = "��Ÿ��: " + player.cooltime.ToString("N0") + "��";

        //ų �� ǥ��
        killText.text = killed.ToString() + " / " + enemies.ToString();

        if (killed == enemies) Debug.LogWarning("Ŭ����");

        //���� �����
        if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(0);
    }


    public void ChangeHPMP() //hp, mp ���� �ֽ�ȭ
    {
        for(int i = 0; i < 6; i++)
        {
            hps[i].gameObject.SetActive(i < player.hp);
            mps[i].gameObject.SetActive(i < player.mp);
        }
    }

} //GameManager End
