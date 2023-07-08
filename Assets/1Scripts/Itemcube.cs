using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Itemcube : MonoBehaviour
{
    public int cubeNum;
    float dist;

    public bool IsPickaxe = false; // 보스전 곡괭이



    void Start()
    {
        var particleColor = cubeNum switch
        {
            0 => new Color(1, 0.7f, 0),
            1 or 2 or 3 or 4 or 5 or 6 => new Color(0.5f, 0.4f, 1),
            7 or 8 or 9 or 10 or 11 or 12 or 13 or 14 => new Color(0.8f, 0.3f, 0),
            _ => Color.white,
        };

        for (int i = 1; i <= 2; i++)
        {
            ParticleSystem.MainModule main = transform.GetChild(i).GetComponent<ParticleSystem>().main;
            main.startColor = particleColor;
        }

        for (int i = 0; i < 8; i++)
            transform.GetChild(2).GetChild(i).GetComponent<SpriteRenderer>().sprite
                = transform.GetComponent<SpriteRenderer>().sprite;

    } //Start End

    void Update()
    {
        dist = Vector2.Distance(transform.position, Player.player.transform.position);

        transform.GetChild(0).gameObject.SetActive(dist < 1 && Mainmenu.markkey); // [E]
        transform.GetChild(2).gameObject.SetActive(dist < 1);

        if (!IsPickaxe) {
            if (dist < 1 && Input.GetKeyDown("e"))
            {
                Player.player.pickupitem.Play();
                GameManager.gameManager.ItemGettodaje();
                Destroy(gameObject);
            }
        }
        else
        {
            Player.Pickaxe = true;
        }

    } //Update End


} //Itemcube End
