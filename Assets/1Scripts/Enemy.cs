using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour //��
{
    Rigidbody2D rigid;
    Vector2 nowPosition;

    float H; //�̵� ��?��
    bool inAttackArea = false; //�÷��̾��� ���� ���� ���� �ִ���

    int hp; //ü��
    public int maxhp; //�ִ� ü��

    float dist; //�÷��̾���� �Ÿ�
    public float noticeDist; //�÷��̾� �ν� ���� ����
    bool moving = false;

    SpriteRenderer sr;
    public Sprite Hurt;


    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        nowPosition = new Vector2(999,999);

        hp = maxhp;

        sr = GetComponent<SpriteRenderer>();
    } //Start End


    void Update()
    {
        //�÷��̾ ���� �̵� ������ �����Ѵ� (������ ������)
        if (transform.position.x > Player.player.transform.position.x)
            H = hp == maxhp ? -1 : -5;
        else H = hp == maxhp ? 1 : 5;

        //�÷��̾�� �� ������ �Ÿ�
        dist =
            Vector2.Distance(transform.position, Player.player.transform.position);

        //������ �̵� ����
        if (dist < noticeDist) moving = true;

        //�� ��� �ý���
        if (inAttackArea && (Input.GetMouseButtonDown(0)
            || Input.GetKeyDown("j"))) //���� ���콺�� ��� �ӽ÷� ������ Ű
        {
            hp--;
            sr.sprite = Hurt;
        }

        if (Mathf.Abs(Player.skillP.y) < 20 &&
            Vector2.Distance(transform.position, Player.skillP) < 3.5f)
        {
            hp--;
            sr.sprite = Hurt;
        }

        //���̸�
        if (hp <= 0) Destroy(this.gameObject);

    } //Update End

    void FixedUpdate()
    {
        //�÷��̾ ������ �Ŀ��� ��� �̵�
        if (moving)
        {
            transform.Translate(H * Time.deltaTime * Vector2.right);

            //���� ���� �� �����̸� ����
            if (Mathf.Abs(transform.position.x - nowPosition.x) < 0.01f)
                rigid.AddForce(0.3f * Vector2.up, ForceMode2D.Impulse);

            nowPosition = transform.position;
        }
    } //FixedUpdate End


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
