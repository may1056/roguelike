using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour //��
{
    float H; //�̵� ��?��
    bool inAttackArea = false; //�÷��̾��� ���� ���� ���� �ִ���

    int hp; //ü��
    public int maxhp; //�ִ� ü��

    float dist; //�÷��̾���� �Ÿ�
    public float noticeDist; //�÷��̾� �ν� ���� ����

    SpriteRenderer sr;
    public Sprite Hurt;


    void Start()
    {
        hp = maxhp;

        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //�÷��̾ ���� �̵� ������ �����Ѵ� (������ ������)
        if (transform.position.x > Player.player.transform.position.x)
            H = hp == maxhp ? -1 : -5;
        else H = hp == maxhp ? 1 : 5;

        //�÷��̾�� �� ������ �Ÿ�
        dist =
            Vector2.Distance(transform.position, Player.player.transform.position);

        //�� ��� �ý���
        if (inAttackArea && Input.GetKeyDown("k"))
        {
            hp--;
            sr.sprite = Hurt;
        }

        //���̸�
        if (hp <= 0) Destroy(this.gameObject);
    }

    void FixedUpdate()
    {
        //������ �̵�
        if (dist < noticeDist) transform.Translate(H * Time.deltaTime * Vector2.right);
    }


    private void OnDestroy()
    {
        GameManager.killed++; //�����鼭 ų �� �ø��� ��
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            inAttackArea = true; //����
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
            inAttackArea = false; //�������´�
    }

} //Enemy End
