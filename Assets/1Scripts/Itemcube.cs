using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itemcube : MonoBehaviour
{
    public int cubeNum;
    float dist;



    void Update()
    {
        dist = Vector2.Distance(transform.position, Player.player.transform.position);

        transform.GetChild(0).gameObject.SetActive(dist < 1); //E

        if (dist < 1 && Input.GetKeyDown("e"))
        {
            if (Player.itemNum.Item1 == -1) //아이템1에 넣음
            {
                Player.itemNum.Item1 = cubeNum;
                GameManager.gameManager.ItemInfo();
            }
            else if (Player.itemNum.Item2 == -1) //아이템2에 넣음
            {
                Player.itemNum.Item2 = cubeNum;
                GameManager.gameManager.ItemInfo();
            }
            else GameManager.gameManager.ItemChangeHaseyo();

            Destroy(gameObject);
        }

    } //Update End

} //Itemcube End
