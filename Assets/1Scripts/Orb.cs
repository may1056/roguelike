using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour //구체
{
    float dist;
    public bool hp_mp;

    Vector2 tp;

    private void Awake()
    {
        //tp = transform.position;
    }

    void Update()
    {
        Vector2 ptp = Player.player.transform.position;
        tp = transform.position;

        dist = Vector2.Distance(tp, ptp);

        //if (dist < 3) transform.Translate(Time.deltaTime
        //    * new Vector2(ptp.y - tp.y, ptp.x - tp.x));

        if (dist < 1.5f)
        {
            if (hp_mp) Player.player.hp++;
            else Player.player.mp++;

            Player.getOrb = true;
            Destroy(gameObject);
        }
    }

} //Orb End
