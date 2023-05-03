using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour //�÷��̾�
{
    public GameManager manager;

    public static Player player;
    //public static���� ������ ������ �ٸ� ��ũ��Ʈ���� ����� �۰� �� �ִ�

    Rigidbody2D rigid;
    SpriteRenderer sr;
    Animator anim;

    public Sprite[] players = new Sprite[3];
    //�ִϸ��̼� ����� �����Ƽ� �ӽù������� ��������Ʈ ��ü��
    //0: ��� ����(����), 1: �ȱ�(�޸���), 2: �ٱ�(����)

    public int hp;

    bool isWalking = false;
    bool isJumping = false;
    public float jumpPower; //�ٴ� ��
    float notJumpTime = 0; //������ �ִ� �ð�

    bool isAttacking = false;
    SpriteRenderer attacksr;
    //���� ������Ʈ�� ��������Ʈ�������� �ѵ� ���߿� ������ �� ��������


    void Awake()
    {
        player = this; //�̰� ����

        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //�÷��̾��� 0��° �ڼ��� Attack�� ��������Ʈ�������� ���� �´�.
        //������ ���� ������ ����� ����.
        attacksr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        attacksr.color = new Color(1, 1, 1, 0);
    }


    void Update()
    {
        //����
        if (Input.GetKeyDown("w") && !isJumping)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isJumping = true;
            sr.sprite = players[2];
        }

        //���� ���� �ӷ��� 0�� ���°� 0.1�� �̻��̸� ���� �ߴ�
        if (rigid.velocity.y == 0) notJumpTime += Time.deltaTime;
        else notJumpTime = 0;
        isJumping = notJumpTime < 0.1f;

        //���� ��ȯ
        if (Input.GetButton("Horizontal"))
            sr.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //��?�� ���� ������Ʈ�� ������ ��ġ ����. �̰� �� ���� �պ��� �Ѵ�.
        attacksr.flipX = sr.flipX;
        float X = sr.flipX ? -2f : 2f;
        transform.GetChild(0).transform.localPosition = new Vector2(X, 0);

        if (!isJumping) //���� ��
        {
            if (isWalking) sr.sprite = players[1]; //�Ȱ� ������ �ȴ� ��������Ʈ
            else sr.sprite = players[0]; //���� ������ ���� ��������Ʈ
        }

        //�����ε� ���� Ű ��ü�� �ʿ�
        isAttacking = Input.GetKey("k");
        if (isAttacking) attacksr.color = new Color(1, 1, 1, 1); //��������
        else attacksr.color = new Color(1, 1, 1, 0); //������

        if (hp <= 0) SceneManager.LoadScene(0); //���̸�
    }


    void FixedUpdate()
    {
        //�¿� �̵� (���, �� ���� �ٷ� ����)
        float h = Input.GetAxisRaw("Horizontal");
        transform.Translate(10 * Time.deltaTime * new Vector2(h, 0));

        isWalking = h != 0;

        /*
        //�÷��� ����
        if (rigid.velocity.y < 0)
        {
            //�����ͻ󿡼��� Ray�� �׷��ִ� �Լ�
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            //Ray�� ���� ������Ʈ
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down,
                2, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 2.5f)
                {
                    isJumping = false;
                    sr.sprite = players[0];
                }
            }
        }
        */ //����ĳ��Ʈ ���� ��
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform")) isJumping = false;
        //�÷��� ������ ���� ���� ���� ���� (�� �� ����..)

        if (collision.gameObject.CompareTag("Enemy")) //����
        {
            hp--;
            manager.ChangeHP();
        }
    }

} //Player End
