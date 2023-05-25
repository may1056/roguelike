using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    public Transform textbarTriangle;
    Image trianglesr;
    bool triangleenable = true;

    public Text textbar;
    public string[] texts; // 보기 편?하게 Storymanager컴포넌트에서 입력하겠습니다
    public int textindex = 1;

    public bool onOption = false; // 옵션이 켜져있는가?

    public Transform background;
    public Sprite[] backgroundimages; // 아마도 텍스트바 뒤에 뜰 그림
    Sprite backgroundimage;


    void Start()
    {
        trianglesr = textbarTriangle.GetComponent<Image>();
        backgroundimage = background.GetComponent<Sprite>();

        InvokeRepeating("textbarTriangleAnimaition", 0, 1f);
    }


    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !onOption) textindex++; // 옵션 위에 있지 않을 때, 클릭&스페이스바 입력시 대화 넘어감

        textbar.text = texts[textindex];
    }

    public void IsclickOnButton() // 버튼을 눌렀는지 확인하는 함수, 옵션버튼 눌렀을때 텍스트가 넘어가지 않게 하는 역할
    {
            if(!onOption)
            textindex--;

            onOption = !onOption;
            return;
    }
    void textbarTriangleAnimaition() // 단순하게 깜빡거리는게 전부인 개선의 여지가 많아 보이는 코드,, 그냥 애니메이션으로 대체할 예정
    {
        triangleenable = !triangleenable;
        if (triangleenable) trianglesr.color = new Color(1, 1, 1, 1);
        else trianglesr.color = new Color(1, 1, 1, 0);
    }
}
