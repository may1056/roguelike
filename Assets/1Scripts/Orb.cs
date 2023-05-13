using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour //구체
{
    Rigidbody2D rigid;

    float dist;
    public int kind; //0: hp오브, 1: mp오브, 2: 동전

    Vector2 tp;

    float magnetTime = 0;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        Vector2 ptp = Player.player.transform.position;
        tp = transform.position;

        dist = Vector2.Distance(tp, ptp);


        magnetTime += Time.deltaTime;

        if (dist < 7 && magnetTime > 0.5f) //끌려간다
        {
            transform.rotation = Quaternion.Euler(new Vector3
                (0, 0, Mathf.Rad2Deg * Mathf.Atan2(ptp.y - tp.y, ptp.x - tp.x)));
            transform.Translate(5 * Time.deltaTime * Vector2.right);

            rigid.mass = 0;
            rigid.gravityScale = 0;
        }
        else //그냥 있는다
        {
            rigid.mass = 0.2f;
            rigid.gravityScale = 1;
        }


        if (dist < 0.5f)
        {
            switch (kind)
            {
                case 0: Player.player.hp++; break;
                case 1: Player.player.mp++; break;
                case 2: GameManager.coins++; break;
            }
            Player.getOrb = true;
            Destroy(gameObject);
        }
    }

} //Orb End
