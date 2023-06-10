using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    public static bool isEnding = true;

    // 텍스트바 점멸하는 세모
    public Transform textbarTriangle;
    public Image trianglesr;
    bool triangleenable = true;

    // 텍스트바
    public Text textbar;
    public string[] texts; // 보기 편?하게 Storymanager컴포넌트에서 입력하겠습니다
    public int textindex = 0;

    public bool onOption = false; // 옵션이 켜져있는가?

    // 배경화면
    public Sprite[] backgroundimages; // 텍스트바 뒤에 뜰 그림 // 놀랍게도 이미지 컴포넌트에 들어가는 것은 스프라이트였다..
    public Image backgroundimage;
    public int imageindex = 0;

    //배경음악
    public AudioClip[] storyAudio;
    public AudioSource storyAudioSource;
    public int audioindex = 0;

    AudioSource menusound;


    //엔딩
    public string[] endtexts;
    public int endtextindex = 0;
    public Sprite[] endbackgroundimages;
    public AudioClip endstoryAudio;




    void Start()
    {
        trianglesr = textbarTriangle.GetComponent<Image>();
        storyAudioSource = GetComponent<AudioSource>();
        menusound = transform.GetChild(0).GetComponent<AudioSource>();

        onOption = false;

        Images_Texts();

        storyAudioSource.clip = storyAudio[audioindex];
        storyAudioSource.Play();

        InvokeRepeating(nameof(textbarTriangleAnimaition), 0, 1f);
    }


    void Images_Texts()
    {
        if (isEnding)
        {
            backgroundimage.sprite = endbackgroundimages[imageindex];
            textbar.text = endtexts[textindex];
        }
        else
        {
            backgroundimage.sprite = backgroundimages[imageindex];
            textbar.text = texts[textindex];
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Nextscene(); // 스페이스바 입력시 Nextscene 함수 실행

    }

    public void Nextscene() // onclick 용 (배경에 투명버튼 설치해놓아서 옵션 이외에 공간을 클릭하면 이 함수 실행된다.)
    {
        if (!onOption)
        {
            textindex++;
            imageindex++;
        }

        if (!isEnding && textindex >= texts.Length-1) SceneManager.LoadScene(1);
        else if (isEnding && textindex >= endtexts.Length-1) SceneManager.LoadScene(3);

        Images_Texts();
    }

    public void OptionbuttonOn()
    {
        onOption = true;
        MenuSound();
    }

    public void OptionbuttonOff()
    {
        onOption = false;
        MenuSound();
    }

    public void audiotest()
    {
        audioindex++;
        storyAudioSource.clip = storyAudio[audioindex];
        storyAudioSource.Play();
    }
    void textbarTriangleAnimaition() // 단순하게 깜빡거리는게 전부인 개선의 여지가 많아 보이는 코드,, 그냥 애니메이션으로 대체할 예정
    {
        triangleenable = !triangleenable;
        trianglesr.color = new Color(1, 1, 1, triangleenable ? 1 : 0);
    }
    public void MenuSound()
    {
        menusound.Play();
    }
}
