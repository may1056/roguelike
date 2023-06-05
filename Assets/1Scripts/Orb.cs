using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour //구체
{
    Rigidbody2D rigid;
    CircleCollider2D col;

    float dist;
    public int kind; //0: hp오브, 1: mp오브, 2: 동전

    Vector2 tp;

    float magnetTime = 0;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
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
            col.isTrigger = true;
        }
        else //그냥 있는다
        {
            rigid.mass = 0.2f;
            rigid.gravityScale = 0.5f;
            col.isTrigger = false;
        }


        if (dist < 0.5f)
        {
            switch (kind)
            {
                case 0: Player.player.hp++; GameManager.gameManager.recover.Play(); break;
                case 1: PlayerAttack.playerAtk.mp++; GameManager.gameManager.recover.Play(); break;
                case 2: GameManager.coins++; Player.player.pickupcoin.Play();  break;
            }
            Destroy(gameObject);
        }
    }




} //Orb End
