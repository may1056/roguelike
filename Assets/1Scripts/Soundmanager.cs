using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Soundmanager : MonoBehaviour
{

    public static Soundmanager soundmanager;

    public AudioSource[] diesounds;
    public AudioSource[] firesounds;

    public AudioSource[] swordsounds;
    public AudioSource[] magicgunsounds;

    public AudioSource dashsound;

    public AudioSource basicskillsound;

    public AudioSource[] bgm, bossbgm;
    public static float bgmTime;


    private void Awake()
    {
        soundmanager = this;
    }
}
