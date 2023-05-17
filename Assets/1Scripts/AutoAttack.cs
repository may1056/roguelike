using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    public GameManager manager;

    Transform target; //목표

    public GameObject pollutingbullet;





    void Start()
    {
        InvokeRepeating(nameof(ShootBullet), 1, 0.3f);
    }


    void Update()
    {
        Transform nowtarget = null;
        float nowdist = 999;

        if (manager.transform.childCount == 1)
        {
            if (manager.transform.GetChild(0).childCount > 2)
            {
                for (int i = 2; i < manager.transform.GetChild(0).childCount; i++)
                {
                    for (int j = 0; j < manager.transform.GetChild(0).GetChild(i).childCount; j++)
                    {
                        float d = Vector2.Distance(transform.position, manager.transform.GetChild(0).GetChild(i).GetChild(j).transform.position);

                        if (d <= nowdist)
                        {
                            nowdist = d;
                            nowtarget = manager.transform.GetChild(0).GetChild(i).GetChild(j).transform;
                        }
                    }
                }
            }
        }

        target = nowtarget;

        if (!manager.making) CancelInvoke();

    } //Update End


    void ShootBullet()
    {
        if (target != null) Instantiate(pollutingbullet, transform.position,
                Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x)));
    }

} //AutoAttack
