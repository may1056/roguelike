using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour //적
{
    float H;
    bool inAttackArea = false; //플레이어의 공격 범위 내에 있는지


    void Update()
    {
        //플레이어를 향해 이동 방향을 변경한다
        if (transform.position.x > Player.player.transform.position.x) H = -1;
        else H = 1;

        //일단은 공격당하면 사망인데 우리는 이걸 피 닳는 시스템으로 바꿔야 한다
        if (inAttackArea && Input.GetKeyDown(KeyCode.DownArrow))
            Destroy(gameObject);

        //빠른 재시작
        if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(0);
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
