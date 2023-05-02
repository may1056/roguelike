using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour //��
{
    float H;
    bool inAttackArea = false; //�÷��̾��� ���� ���� ���� �ִ���


    void Update()
    {
        //�÷��̾ ���� �̵� ������ �����Ѵ�
        if (transform.position.x > Player.player.transform.position.x) H = -1;
        else H = 1;

        //�ϴ��� ���ݴ��ϸ� ����ε� �츮�� �̰� �� ��� �ý������� �ٲ�� �Ѵ�
        if (inAttackArea && Input.GetKeyDown(KeyCode.DownArrow))
            Destroy(gameObject);

        //���� �����
        if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(0);
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
