using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Clear : MonoBehaviour
{
    public TextMeshProUGUI time, kill, coin;

    public Image[] goods;
    public Sprite[] items, weapons;

    public ParticleSystem[] particles;

    public Image[] lights;
    bool[] dir = new bool[3]; //방향
    int[] spd = new int[3]; //속도
    float[] ang = new float[3]; //회전각



    void Start()
    {
        Time.timeScale = 1;


        //기록
        bool playedfromscratch = NewWonderfulLeejonghwanShitWow.selectedFloor == 1 && NewWonderfulLeejonghwanShitWow.selectedStage == 1;

        int min = (int)(GameManager.게임실행시간 / 60);
        int sec = (int)(GameManager.게임실행시간 % 60);

        if (GameManager.hardmode) //하드모드
        {
            NewWonderfulLeejonghwanShitWow.clearCount_hard++;

            if (playedfromscratch &&
            (min < NewWonderfulLeejonghwanShitWow.shortestTime_hard.Item1 ||
            (min == NewWonderfulLeejonghwanShitWow.shortestTime_hard.Item1 && sec < NewWonderfulLeejonghwanShitWow.shortestTime_hard.Item2)))
                NewWonderfulLeejonghwanShitWow.shortestTime_hard = (min, sec);

            if (GameManager.killed > NewWonderfulLeejonghwanShitWow.maxKill_hard)
                NewWonderfulLeejonghwanShitWow.maxKill_hard = GameManager.killed;

            //저장
            PlayerPrefs.SetInt("ClearCount_hard", NewWonderfulLeejonghwanShitWow.clearCount_hard);
            PlayerPrefs.SetInt("ShortestTimeMinute_hard", NewWonderfulLeejonghwanShitWow.shortestTime_hard.Item1);
            PlayerPrefs.SetInt("ShortestTimeSecond_hard", NewWonderfulLeejonghwanShitWow.shortestTime_hard.Item2);
            PlayerPrefs.SetInt("MaxKill_hard", NewWonderfulLeejonghwanShitWow.maxKill_hard);
        }
        else //이지모드
        {
            NewWonderfulLeejonghwanShitWow.clearCount_easy++;

            if (playedfromscratch &&
            (min < NewWonderfulLeejonghwanShitWow.shortestTime_easy.Item1 ||
            (min == NewWonderfulLeejonghwanShitWow.shortestTime_easy.Item1 && sec < NewWonderfulLeejonghwanShitWow.shortestTime_easy.Item2)))
                NewWonderfulLeejonghwanShitWow.shortestTime_easy = (min, sec);

            if (GameManager.killed > NewWonderfulLeejonghwanShitWow.maxKill_easy)
                NewWonderfulLeejonghwanShitWow.maxKill_easy = GameManager.killed;

            //저장
            PlayerPrefs.SetInt("ClearCount_easy", NewWonderfulLeejonghwanShitWow.clearCount_easy);
            PlayerPrefs.SetInt("ShortestTimeMinute_easy", NewWonderfulLeejonghwanShitWow.shortestTime_easy.Item1);
            PlayerPrefs.SetInt("ShortestTimeSecond_easy", NewWonderfulLeejonghwanShitWow.shortestTime_easy.Item2);
            PlayerPrefs.SetInt("MaxKill_easy", NewWonderfulLeejonghwanShitWow.maxKill_easy);
        }

        NewWonderfulLeejonghwanShitWow.SaveWhenGameEnds();


        //표시
        time.text = playedfromscratch
            ? ((int)(GameManager.게임실행시간 / 60)).ToString() + ":" + ((int)(GameManager.게임실행시간 % 60)).ToString("D2")
            : "-- : --";
        kill.text = GameManager.killed.ToString();
        coin.text = "+" + GameManager.coins.ToString();


        //사용 아이템과 무기
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
        NewWonderfulLeejonghwanShitWow.savedcoin += GameManager.coins;
        PlayerPrefs.SetInt("SavedCoin", NewWonderfulLeejonghwanShitWow.savedcoin);
        SceneManager.LoadScene(0);
    }

} //Clear End
