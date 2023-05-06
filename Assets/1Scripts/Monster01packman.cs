using UnityEngine;
using System.Collections;

public class Monster01packman : MonoBehaviour //팩맨
{
    Vector2 nowPosition;

    bool inAttackArea = false;

    int hp;
    public int maxhp;
    bool leejong;
    SpriteRenderer sr;
    public Sprite Hurt;

    void Start()
	{
        nowPosition = new Vector2(999, 999);

        hp = maxhp;

        sr = GetComponent<SpriteRenderer>();

    } //Start End

	void Update()
	{
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

        //���̸�
        if (hp <= 0) Destroy(this.gameObject);
    } //Update End


   void FixedUpdate()
    {
        float h = leejong ? 3 : -3;
        transform.Translate(h * Time.deltaTime * Vector2.right);

        if (Mathf.Abs(transform.position.x - nowPosition.x) < 0.01f)
            leejong = !leejong;

        sr.flipX = leejong;

        nowPosition = transform.position;
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


} //Monster01packman End


