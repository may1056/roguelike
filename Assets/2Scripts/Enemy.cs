using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour //적
{
    float H;
    bool inAttackArea = false; //플레이어의 공격 범위 내에 있는지

    int hp;
    SpriteRenderer sr;
    public Sprite Hurt;


    void Start()
    {
        hp = 3;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //플레이어를 향해 이동 방향을 변경한다 (아프면 빨라짐)
        if (transform.position.x > Player.player.transform.position.x)
            H = hp == 3 ? -1 : -5;
        else H = hp == 3 ? 1 : 5;

        //피 닳는 시스템
        if (inAttackArea && Input.GetKeyDown(KeyCode.DownArrow))
        {
            hp--;
            sr.sprite = Hurt;
        }

        //쉐이망
        if (hp <= 0) Destroy(this.gameObject);
    }

    void FixedUpdate()
    {
        transform.Translate(H * Time.deltaTime * Vector2.right); //이동
    }


    private void OnDestroy()
    {
        GameManager.killed++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack") inAttackArea = true; //들어간다
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack") inAttackArea = false; //빠져나온다
    }

} //Enemy End
