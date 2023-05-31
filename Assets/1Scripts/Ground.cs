using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject ground;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("충돌은 함");
        if (other.gameObject.CompareTag("Attack")/* && Input.GetMouseButtonDown(0)*/)  Debug.Log("as");
    }
}   
    