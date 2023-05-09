using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour //탄막
{
    public float bulletSpeed;


    void Update()
    {
        transform.Translate(bulletSpeed * Time.deltaTime * Vector2.right);

        if (Mathf.Abs(transform.position.x) > 100 || Mathf.Abs(transform.position.y) > 100)
            Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform")) //플랫폼
            Destroy(gameObject);
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
