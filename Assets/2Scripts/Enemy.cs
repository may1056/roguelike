using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour //��
{
    float H;
    bool inAttackArea = false; //�÷��̾��� ���� ���� ���� �ִ���

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
        //�÷��̾ ���� �̵� ������ �����Ѵ� (������ ������)
        if (transform.position.x > Player.player.transform.position.x)
            H = hp == 3 ? -1 : -5;
        else H = hp == 3 ? 1 : 5;

        //�� ��� �ý���
        if (inAttackArea && Input.GetKeyDown(KeyCode.DownArrow))
        {
            hp--;
            sr.sprite = Hurt;
        }

        //���̸�
        if (hp <= 0) Destroy(this.gameObject);
    }

    void FixedUpdate()
    {
        transform.Translate(H * Time.deltaTime * Vector2.right); //�̵�
    }


    private void OnDestroy()
    {
        GameManager.killed++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack") inAttackArea = true; //����
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack") inAttackArea = false; //�������´�
    }

} //Enemy End
