using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class WorldEnder : MonoBehaviour
{
    float t = 0;
    public Camera cam;
    public GameObject bg;
    public Canvas canvas, pscanvas;
    public Image white, black;

    void Start()
    {
        canvas.gameObject.SetActive(false);
        pscanvas.gameObject.SetActive(false);
        bg.SetActive(false);
        white.transform.parent.gameObject.SetActive(true);

        StartCoroutine(Ender());
    }

    void Update()
    {
        t += 5 * Time.deltaTime;
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, t * t);
        cam.orthographicSize = 12 + t * 0.1f;

        if (t > 40 && t < 45) white.color = new Color(1, 1, 1, 0.2f * (t - 40));
        if (t > 50 && t < 53) white.color = new Color(1, 1, 1, 0.333f * (53 - t));
        //if (t > 50 && t < 85) Boss2.boss2.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, (85 - t) / 35);
        if (t > 65 && t < 85) black.color = new Color(0, 0, 0, 0.05f * (t - 65));

        Player.player.hp = 6; Player.player.shield = 2;

        if (Input.GetKey(KeyCode.Return)) GameManager.gameManager.NextStage();
    }

    IEnumerator Ender()
    {
        yield return new WaitForSeconds(9.5f);
        GameManager.gameManager.transform.GetChild(0).GetChild(0).GetComponent<Tilemap>().color = Color.black; //ground
        GameManager.gameManager.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(true); //pieces
        GameManager.gameManager.transform.GetChild(0).GetChild(2).gameObject.SetActive(false); //maplimit
        Player.player.transform.position = new Vector2(0, -30);

        yield return new WaitForSeconds(7.5f);
        GameManager.gameManager.NextStage();

        yield break;
    }

} //WorldEnder End
