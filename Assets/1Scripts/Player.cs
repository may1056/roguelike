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

    public float speed = 0; //�޸��� �ӵ�

    public static float maxAttackCooltime = 0.2f; // ��Ÿ�� �ð�
    public static float curAttackCooltime = 0; // ���� ��Ÿ��
    public float attackspeed = 0; // ���� ��Ÿ��
    public static Vector2 attackP;
    public Sprite attackSprite;
    bool attackuse = false;


    bool isWalking = false;
    bool isJumping = false;
    public float jumpPower; //�ٴ� ��
    float notJumpTime = 0; //������ �ִ� �ð�


    bool isDashing = false;
    bool onceDashed = false; //���߿��� �뽬�� �̹� �ߴ���
    public float maxDash; //�ִ� �뽬 ���� �ð�
    public float dashSpeed; //�뽬 ������
    float dashTime = 0; //�뽬�ϴ� �ð�
    public GameObject dashEffect;
    public Sprite dashSprite;

    SpriteRenderer attacksr;
    //���� ������Ʈ�� ��������Ʈ������, flipX ������ �ʿ��� ��

    //����� ȹ�� �� �÷��̾��� �ڼ� ��� �� ù ��°�� ������ �սô�!!!

    public int mp;
    public float cooltime = 0;

    bool skilluse; //��ų �����ϴ���
    public static Vector2 skillP; //��ų �� ��ġ
    public Sprite skillSprite;


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
    } //Awake End


    void Update()
    {
        //����
        if (Input.GetKeyDown("w") && !isJumping)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isJumping = true;
            sr.sprite = players[2];
        }

        //���� ���� �ӷ��� ���� 0�� ���°� 0.1�� �̻��̸� ���� �ߴ�
        if (Mathf.Abs(rigid.velocity.y) < 0.01f) notJumpTime += Time.deltaTime;
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
            onceDashed = false;

            if (isWalking) sr.sprite = players[1]; //�Ȱ� ������ �ȴ� ��������Ʈ
            else sr.sprite = players[0]; //���� ������ ���� ��������Ʈ
        }


        //���콺 ��Ŭ�� �뽬
        if (isJumping && !onceDashed && (Input.GetMouseButtonDown(1)
            || Input.GetKeyDown("k"))) //k�� �ӽ� �뽬 Ű
        {
            isDashing = true;
            onceDashed = true;
            gameObject.layer = 12; //12PlayerDash
        }


        //�Ϲݰ��� ��Ÿ��, �ִϸ��̼�
        if (curAttackCooltime <= maxAttackCooltime + 2) curAttackCooltime += Time.deltaTime;
        attackuse = (Input.GetMouseButton(0) || Input.GetKey("j")) && (curAttackCooltime >= maxAttackCooltime); //j�� �ӽ� ���� Ű
        if (attackuse)
        {
            
            float x = sr.flipX ? -2 : 2;
            attackP = new Vector2(transform.position.x + x, transform.position.y);
            attacksr.color = new Color(1, 1, 1, 1);
            Debug.Log(curAttackCooltime);
        }
        else attacksr.color = new Color(1, 1, 1, 0);

        //��ų
        if (cooltime > 0) cooltime -= Time.deltaTime;
        skilluse = cooltime <= 0 && Input.GetKeyDown("s") && mp >= 1;
        if (skilluse) //���� ��ų
        {
            cooltime = 3;
            float x = sr.flipX ? -3 : 3;
            skillP = new Vector2(transform.position.x + x, transform.position.y);
            MakeEffect(skillP, skillSprite, -2);
            mp--;
            manager.ChangeHPMP();
        }
        else skillP = new Vector2(9999, 9999); //�� �ָ�


        if (hurtTime >= 0) Hurt(); //���� ��
        else sr.color = Color.white; //�⺻

        if (hp <= 0) SceneManager.LoadScene(0); //���̸�
    } //Update End


    void FixedUpdate()
    {
        //�¿� �̵� (���, �� ���� �ٷ� ����)
        float h = Input.GetAxisRaw("Horizontal");
        transform.Translate((10+speed) * Time.deltaTime * new Vector2(h, 0)); // �ӵ� �⺻ �� 10 + speed

        isWalking = h != 0;


        if (isDashing) //�뽬 ���̴�
        {
            dashTime += Time.deltaTime;
            rigid.AddForce(dashSpeed * (sr.flipX ? Vector2.left : Vector2.right),
                ForceMode2D.Impulse);
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            MakeEffect(transform.position, dashSprite, -1);
        }
        if (dashTime >= maxDash) //�뽬 �ð�
        {
            isDashing = false;
            rigid.velocity = new Vector2(0, 0);
            gameObject.layer = 11; //11Player
            dashTime = 0;
        }

    } //FixedUpdate End


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //�÷��� ������ ���� ���� ���� ���� (�� �� ����..)
        if (collision.gameObject.CompareTag("Platform")) isJumping = false;

        //����
        if (gameObject.layer == 11 && collision.gameObject.CompareTag("Enemy"))
        {
            hp--;
            manager.ChangeHPMP();
            hurtTime = 1;
        }
    }

    //�������� �𸣰����� �ݶ��̴� ���� �Լ����� �˴� �̻��ϰ� �۵��Ѵ�. ������


    void Hurt() //��� ������ �Ǿ��ٰ� ������ ȸ��
    {
        sr.color = new Color(1, 1 - hurtTime, 1 - hurtTime);
        hurtTime -= 4 * Time.deltaTime;
    }


    //position, damage, sprite, layer
    void MakeEffect(Vector2 p, Sprite s, int l) //Fade ��ũ��Ʈ�� ���� ������Ʈ ����
    {
        GameObject effect = Instantiate(dashEffect, p, Quaternion.identity);
        SpriteRenderer esr = effect.transform.GetComponent<SpriteRenderer>();
        esr.sprite = s;
        esr.flipX = sr.flipX;
        esr.sortingOrder = l;
    }

} //Player End
