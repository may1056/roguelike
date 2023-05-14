using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour //탄막
{
    public float bulletSpeed;


    void Update()
    {
        Vector2 tp = transform.position;

        transform.Translate(bulletSpeed * Time.deltaTime * Vector2.right);

        if (Mathf.Abs(tp.x) > 100 || Mathf.Abs(tp.y) > 100) Destroy(gameObject);


        //스킬 범위 내에 있음
        if (Mathf.Abs(PlayerAttack.skillP.y) < 100 &&
            Vector2.Distance(tp, PlayerAttack.skillP) < 5.5f) Destroy(gameObject);



        //무기 파생 스킬 범위 내에 있음
        Vector2 wsp = PlayerAttack.wsP;

        if (Mathf.Abs(wsp.y) < 100)
        {
            switch (Player.weaponNum)
            {
                case 0:
                    bool inX = Mathf.Abs(wsp.x - tp.x) < 7.5f
                        && Mathf.Abs(wsp.y - tp.y) < 1;
                    bool inY = Mathf.Abs(wsp.y - tp.y) < 7.5f
                        && Mathf.Abs(wsp.x - tp.x) < 1;
                    if (inX || inY) Destroy(gameObject);
                    break;
            }
        }


        //위치 저장에 의한 파괴
        if (Mathf.Abs(Player.posP[0].y) < 100)
        {
            for (int i = 0; i < 2; i++)
            {
                if (Vector2.Distance(tp, Player.posP[i]) < 3) Destroy(gameObject);
            }
        }

    } //Update End


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform"|| collision.gameObject.tag == "Attack")
        {
            Destroy(gameObject);
        } //플랫폼
            
    }

    //플랫폼 닿았는데 왜 안 없어짐????????
    //그럼 어쩔 수 없이 땅 뚫어서 쏘게 놔둬야겠네~ 누군가는 깨겠지

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Platform")) //플랫폼
            Destroy(gameObject);

        int l = other.gameObject.layer;
        if (l == 11 || l == 13) //플레이어
        {
            Player.hurted = true;
            Destroy(gameObject);
        }
    }

} //Bullet End
