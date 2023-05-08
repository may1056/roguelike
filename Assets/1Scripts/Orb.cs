using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour //구체
{
    float dist;
    public bool hp_mp;

    void Update()
    {
        dist =
            Vector2.Distance(transform.position, Player.player.transform.position);

        if (dist < 1)
        {
            if (hp_mp) Player.player.hp++;
            else Player.player.mp++;

            Player.getOrb = true;
            Destroy(gameObject);
        }
    }
} //Orb End
