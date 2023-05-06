﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster02slime : MonoBehaviour //적
{
    Rigidbody2D rigid;
    Vector2 nowPosition;

    float H; //이동 상?수
    bool inAttackArea = false; //플레이어의 공격 범위 내에 있는지

    int hp; //체력
    public int maxhp; //최대 체력
    float lezong = 0.3f;//점프 시간 카운트
    public float lezonghan; //점프파워
    float dist; //플레이어와의 거리
    public float noticeDist; //플레이어 인식 가능 범위
    bool moving = false;

    SpriteRenderer sr;
    public Sprite Hurt;


    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        nowPosition = new Vector2(999, 999);

        hp = maxhp;

        sr = GetComponent<SpriteRenderer>();

        inAttackArea = false;
    } //Start End


    void Update()
    {
        //플레이어를 향해 이동 방향을 변경한다 (아프면 빨라짐)
        H = transform.position.x > Player.player.transform.position.x ? -1 : 1;

        sr.flipX = transform.position.x < Player.player.transform.position.x - 0.1f;

        //플레이어와 적 사이의 거리
        dist =
            Vector2.Distance(transform.position, Player.player.transform.position);

        //가까우면 이동 시작
        if (dist < noticeDist) moving = true;

        //피 닳는 시스템
        if (inAttackArea && (Input.GetMouseButtonDown(0)
            || Input.GetKeyDown("j")) && Player.curAttackCooltime >= Player.maxAttackCooltime) //내가 마우스가 없어서 임시로 설정한 키
        {
            hp--;
            sr.sprite = Hurt;
            Player.curAttackCooltime = 0;
        }

        if (Mathf.Abs(Player.skillP.y) < 20 &&
            Vector2.Distance(transform.position, Player.skillP) < 3.5f)
        {
            hp--;
            sr.sprite = Hurt;
        }

        //쉐이망
        if (hp <= 0) Destroy(this.gameObject);

        //점프

        if (moving && Mathf.Abs(rigid.velocity.y)< 0.1f)
        {
            lezong -= Time.deltaTime;
        }

    } //Update End

    void FixedUpdate()
    {
        //플레이어를 감지한 후에는 계속 이동
        if (moving)
        {
            transform.Translate(H * Time.deltaTime * Vector2.right);

         
        }
        if (lezong<=0)
        {
            rigid.AddForce(Vector2.up*lezonghan, ForceMode2D.Impulse);
            lezong = 0.3f;
        }
    } //FixedUpdate End


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
