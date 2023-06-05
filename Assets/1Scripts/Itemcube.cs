using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Itemcube : MonoBehaviour
{
    public int cubeNum;
    float dist;

    public bool IsPickaxe = false; // 보스전 곡괭이



    void Update()
    {
        dist = Vector2.Distance(transform.position, Player.player.transform.position);

        transform.GetChild(0).gameObject.SetActive(dist < 1); // [E]

        if (!IsPickaxe) {
            if (dist < 1 && Input.GetKeyDown("e"))
            {
                GameManager.gameManager.ItemGettodaje();
                Destroy(gameObject);
            }
        }
        else
        {
            Player.Pickaxe = true;
        }

    } //Update End


} //Itemcube End
