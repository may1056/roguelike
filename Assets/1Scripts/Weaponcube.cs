using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weaponcube : MonoBehaviour
{
    public int cubeNum;
    float dist;


    void Update()
    {
        dist = Vector2.Distance(transform.position, Player.player.transform.position);

        transform.GetChild(0).gameObject.SetActive(dist < 1); // [E]

        if (dist < 1 && Input.GetKeyDown(KeyCode.E))
        {
            Player.player.pickupitem.Play();
            GameManager.gameManager.WeaponGettodaje();
            Destroy(gameObject);
        }

    } //Update End

} //WeaponCube End
