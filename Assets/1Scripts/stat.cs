using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stat : MonoBehaviour
{

    public Text playerDamageText;
    public Text playerMaxAttackCooltimeText;
    public Text playerSpeedText;
    public Text playerJumpPowerText;
    public Text playerDashSpeedText;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerDamageText.text = "���ݷ�: ���ݷ� ���� �߰��ʿ�" /*+ Player.maxAttackCooltime.ToString()*/;
        playerMaxAttackCooltimeText.text = "���ݼӵ�: " + Player.maxAttackCooltime.ToString();
        playerSpeedText.text = "�̵��ӵ�: " + Player.player.speed.ToString();
        playerJumpPowerText.text = "������: " + Player.player.jumpPower.ToString();
        playerDashSpeedText.text = "��üӵ�: " + Player.player.dashSpeed.ToString();
    }
}
