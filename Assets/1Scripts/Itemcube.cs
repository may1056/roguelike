using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Itemcube : MonoBehaviour
{
    public int cubeNum;
    float dist;

    public bool IsPickaxe = false; // 보스전 곡괭이

    ParticleSystem particle;



    void Start()
    {
        particle = transform.GetChild(1).GetComponent<ParticleSystem>();
        Color particleColor;

        switch (cubeNum)
        {
            case 0:
                particleColor = new Color(1, 0.7f, 0); break;
            case 1: case 2: case 3: case 4: case 5: case 6:
                particleColor = new Color(0.5f, 0.4f, 1); break;
            case 7: case 8: case 9: case 10: case 11: case 12: case 13: case 14:
                particleColor = new Color(0.8f, 0.3f, 0); break;
            default:
                particleColor = Color.white; break;
        }

        ParticleSystem.MainModule main = particle.main;
        main.startColor = particleColor;

    } //Start End

    void Update()
    {
        dist = Vector2.Distance(transform.position, Player.player.transform.position);

        transform.GetChild(0).gameObject.SetActive(dist < 1); // [E]

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
