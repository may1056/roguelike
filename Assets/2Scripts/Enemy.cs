using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour //적
{
    float H; //이동 상?수
    bool inAttackArea = false; //플레이어의 공격 범위 내에 있는지

    int hp; //체력
    public int maxhp; //최대 체력

    float dist; //플레이어와의 거리
    public float noticeDist; //플레이어 인식 가능 범위

    SpriteRenderer sr;
    public Sprite Hurt;


    void Start()
    {
        hp = maxhp;

        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //플레이어를 향해 이동 방향을 변경한다 (아프면 빨라짐)
        if (transform.position.x > Player.player.transform.position.x)
            H = hp == maxhp ? -1 : -5;
        else H = hp == maxhp ? 1 : 5;

        //플레이어와 적 사이의 거리
        dist =
            Vector2.Distance(transform.position, Player.player.transform.position);

        //피 닳는 시스템
        if (inAttackArea && Input.GetKeyDown("k"))
        {
            hp--;
            sr.sprite = Hurt;
        }

        //쉐이망
        if (hp <= 0) Destroy(this.gameObject);
    }

    void FixedUpdate()
    {
        //가까우면 이동
        if (dist < noticeDist) transform.Translate(H * Time.deltaTime * Vector2.right);
    }


    private void OnDestroy()
    {
        GameManager.killed++; //죽으면서 킬 수 올리고 감
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            inAttackArea = true; //들어간다
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            inAttackArea = false; //빠져나온다
    }

} //Enemy End
