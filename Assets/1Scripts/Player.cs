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
    float hurtTime = 0; //�ǰ� �� ����� �ð� ����

    bool isWalking = false;
    bool isJumping = false;
    public float jumpPower; //�ٴ� ��
    float notJumpTime = 0; //������ �ִ� �ð�

    bool isDashing = false;
    public float maxDash; //�ִ� �뽬 ���� �ð�
    public float dashSpeed; //�뽬 ������
    float dashTime = 0; //�뽬�ϴ� �ð�

    bool isAttacking = false;
    SpriteRenderer attacksr;
    //���� ������Ʈ�� ��������Ʈ������, flipX ������ �ʿ��� ��

    //����� ȹ�� �� �÷��̾��� �ڼ� ��� �� ù ��°�� ������ �սô�!!!


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
        transform.GetChild(0).transform.localPosition
            = new Vector2(sr.flipX ? -2f : 2f, 0);


        if (!isJumping) //���� ��
        {
            if (isWalking) sr.sprite = players[1]; //�Ȱ� ������ �ȴ� ��������Ʈ
            else sr.sprite = players[0]; //���� ������ ���� ��������Ʈ
        }


        if (Input.GetMouseButtonDown(1)) //���콺 ��Ŭ�� �뽬
        {
            isDashing = true;
            gameObject.layer = 12; //12PlayerDash
        }


        //����
        isAttacking = Input.GetMouseButton(0);
        if (isAttacking) attacksr.color = new Color(1, 1, 1, 1); //��������
        else attacksr.color = new Color(1, 1, 1, 0); //������

        if (hurtTime >= 0) Hurt(); //���� ��
        else sr.color = Color.white; //�⺻

        if (hp <= 0) SceneManager.LoadScene(0); //���̸�
    }


    void FixedUpdate()
    {
        //�¿� �̵� (���, �� ���� �ٷ� ����)
        float h = Input.GetAxisRaw("Horizontal");
        transform.Translate(10 * Time.deltaTime * new Vector2(h, 0));

        isWalking = h != 0;


        if (isDashing) //�뽬 ���̴�
        {
            dashTime += Time.deltaTime;
            rigid.AddForce(dashSpeed * (sr.flipX ? Vector2.left : Vector2.right),
                ForceMode2D.Impulse);
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
        }
        if (dashTime >= maxDash) //�뽬 �ð�
        {
            isDashing = false;
            rigid.velocity = new Vector2(0, 0);
            gameObject.layer = 11; //11Player
            dashTime = 0;
        }

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
        //�÷��� ������ ���� ���� ���� ���� (�� �� ����..)
        if (collision.gameObject.CompareTag("Platform")) isJumping = false;

        //����
        if (gameObject.layer == 11 && collision.gameObject.CompareTag("Enemy"))
        {
            hp--;
            manager.ChangeHP();
            hurtTime = 1;
        }
    }

    //�������� �𸣰����� �ݶ��̴� ���� �Լ����� �˴� �̻��ϰ� �۵��Ѵ�. ������


    void Hurt() //��� ������ �Ǿ��ٰ� ������ ȸ��
    {
        sr.color = new Color(1, 1 - hurtTime, 1 - hurtTime);
        hurtTime -= 4 * Time.deltaTime;
    }

} //Player End
