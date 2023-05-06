using UnityEngine;
using System.Collections;

public class Monster01 : MonoBehaviour
{
    //Rigidbody2D rigid;
    Vector2 nowPosition;

    bool inAttackArea = false;

    int hp;
    public int maxhp;
    bool leejong;
    SpriteRenderer sr;
    public Sprite Hurt;


    void Start()
	{
        //rigid = GetComponent<Rigidbody2D>();
        nowPosition = new Vector2(999, 999);

        hp = maxhp;

        sr = GetComponent<SpriteRenderer>();
    }


	void Update()
	{
        if (inAttackArea && (Input.GetMouseButtonDown(0)
       || Input.GetKeyDown("j")) && Player.attackCooltime <= 0) 
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
    }


   void FixedUpdate()
    {
        float h = leejong ? 3 : -3;
        transform.Translate(h * Time.deltaTime * Vector2.right);

        if (Mathf.Abs(transform.position.x - nowPosition.x) < 0.01f)
            leejong = !leejong;

        sr.flipX = leejong;

        nowPosition = transform.position;
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


} //Monster01 End


