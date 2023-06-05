using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Soundmanager : MonoBehaviour
{

    public AudioSource dodge;
    public AudioSource jump;
    public AudioSource land;
    public AudioSource recover;
    public AudioSource ouch;
    public AudioSource pickupcoin;
    public AudioSource pickupitem;

    static public GameObject[] SoundObjects; // 소리마다 각각의 오브젝트를 가짐, 순서는 위 변수순대로
    void Start()
    {
        dodge = SoundObjects[0].GetComponent<AudioSource>();
        jump = SoundObjects[1].GetComponent<AudioSource>();
        land = SoundObjects[2].GetComponent<AudioSource>();
        recover = SoundObjects[3].GetComponent<AudioSource>();
        ouch = SoundObjects[4].GetComponent<AudioSource>();
        pickupcoin = SoundObjects[5].GetComponent<AudioSource>();
        pickupitem = SoundObjects[6].GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    void test()
    {
        dodge.Play();
    }
    public void soundplay(int index)
    {

    }


}
