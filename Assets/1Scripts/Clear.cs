using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Clear : MonoBehaviour
{
    public TextMeshProUGUI time, kill;

    public Image[] goods;
    public Sprite[] items, weapons;

    public ParticleSystem[] particles;

    public Image[] lights;
    bool[] dir = new bool[3]; //방향
    int[] spd = new int[3]; //속도
    float[] ang = new float[3]; //회전각



    void Start()
    {
        time.text = ((int)(GameManager.게임실행시간 / 60)).ToString() + ":" + ((int)(GameManager.게임실행시간 % 60)).ToString("D2");
        kill.text = GameManager.killed.ToString();

        goods[0].sprite = items[Player.itemNum.Item1 + 1];
        goods[1].sprite = items[Player.itemNum.Item2 + 1];
        goods[2].sprite = weapons[PlayerAttack.weaponNum.Item1 + 1];
        goods[3].sprite = weapons[PlayerAttack.weaponNum.Item2 + 1];

        for (int i = 0; i < 4; i++)
        {
            var ps = particles[i].shape;
            ps.texture = goods[i].sprite.texture;

            if (i < 3)
            {
                dir[i] = Random.Range(0, 2) == 1;
                spd[i] = Random.Range(1, 7);
                ang[i] = Random.Range(0, 360);
            }
        }

    } //Start End


    void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            ang[i] += (dir[i] ? 1 : -1) * spd[i] * 30 * Time.deltaTime;
            lights[i].rectTransform.rotation = Quaternion.Euler(0, 0, ang[i]);
        }

    } //Update End


    public void ToMainmenu()
    {
        SceneManager.LoadScene(0);
    }

} //Clear End
