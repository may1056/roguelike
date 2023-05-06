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
        playerDamageText.text = "공격력: 공격력 변수 추가필요" /*+ Player.maxAttackCooltime.ToString()*/;
        playerMaxAttackCooltimeText.text = "공격속도: " + Player.maxAttackCooltime.ToString();
        playerSpeedText.text = "이동속도: " + Player.player.speed.ToString();
        playerJumpPowerText.text = "점프력: " + Player.player.jumpPower.ToString();
        playerDashSpeedText.text = "대시속도: " + Player.player.dashSpeed.ToString();
    }
}
