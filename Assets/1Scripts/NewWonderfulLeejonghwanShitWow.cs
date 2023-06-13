using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewWonderfulLeejonghwanShitWow : MonoBehaviour
{
    public static int savedcoin = 0;
    public Text coinText, itemText;

    private void Start()
    {
        Player.itemNum = (-1, -1);
        GameManager.savedItem = (-1, -1);
    }

    void Update()
    {
        coinText.text = savedcoin.ToString();

        if (GameManager.savedItem.Item2 != -1) itemText.text = "아이템1, 아이템2 장착됨";
        else if (GameManager.savedItem.Item1 != -1) itemText.text = "아이템1 장착됨";
        else itemText.text = null;
    }

    public void BuyRandomItem()
    {
        if (savedcoin >= 50)
        {
            if (GameManager.savedItem.Item1 == -1)
            {
                GameManager.savedItem.Item1 = Random.Range(0, 15);
                savedcoin -= 50;
            }
            else if (GameManager.savedItem.Item2 == -1)
            {
            re:
                GameManager.savedItem.Item2 = Random.Range(0, 15);
                if (GameManager.savedItem.Item1 == GameManager.savedItem.Item2) goto re;
                savedcoin -= 50;
            }
        }
    }

} //NewWonderfulLeejonghwanShitWow End
