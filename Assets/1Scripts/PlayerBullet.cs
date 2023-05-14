using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float bulletSpeed;


    void Update()
    {
        Vector2 tp = transform.position;

        transform.Translate(bulletSpeed * Time.deltaTime * Vector2.right);

        if (Mathf.Abs(tp.x) > 100 || Mathf.Abs(tp.y) > 100) Destroy(gameObject);



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
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Attack")
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

        //int l = other.gameObject.layer;
        //if (l == 11 || l == 13) //플레이어
        //{
        //    Player.hurted = true;
        //    Destroy(gameObject);
        //}
    }

} //Bullet End
