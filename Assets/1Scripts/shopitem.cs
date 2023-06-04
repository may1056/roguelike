using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopitem : MonoBehaviour{

    public GameObject ShopSet;//상점 열고 닫/
    public void shopbuyhp() //버튼 클릭 이벤트에 대한 함수를 만들어 준다.체력회복
    {
        if(GameManager.coins >= 10) {
            Player.player.hp += 6;
            GameManager.coins -= 10;
        }

    }
    public void shopbuymp() //버튼 클릭 이벤트에 대한 함수를 만들어 준다.마나회복
    {
        if (GameManager.coins >= 10)
        {
            PlayerAttack.playerAtk.mp += 6;
            GameManager.coins -= 10;
        }
       
    }
    public void shopinout()//샵버튼 작동하게 만든는 거
    {
        if (ShopSet.activeSelf)
        {
            ShopSet.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            ShopSet.SetActive(true);
            Time.timeScale = 0;
        }
    }
}