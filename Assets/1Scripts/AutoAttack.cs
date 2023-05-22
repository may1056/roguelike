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
        InvokeRepeating(nameof(ShootBullet), 0, 0.5f); //0.5초마다 ShootBullet() 함수 실행
    }


    void Update()
    {
        Transform nowtarget = null; //타겟은 일단 아무것도 없다
        float nowdist = 999; //타겟까지 현재 거리는 일단 이상하게 둔다

        //게임매니저 > 맵 > 블록, 그라운드, 몬스터집합 > 몬스터

        if (manager.transform.childCount == 1) //게임매니저 자손이 있다면, 즉 맵(Grid)이 만들어져 있다면
        {
            if (manager.transform.GetChild(0).childCount > 2) //게임매니저 0번 자손인 맵(Grid)의 자손이 둘보다 크다면, 즉 Block과 Ground 말고도 몬스터집합이 존재한다면, 즉 전투가 진행 중이라면
            {
                for (int i = 2; i < manager.transform.GetChild(0).childCount; i++) //맵의 2번 자손부터 마지막 번호 자손까지, 즉 모든 몬스터집합에 대해
                {
                    for (int j = 0; j < manager.transform.GetChild(0).GetChild(i).childCount; j++) //몬스터집합의 모든 자손에 대해, 즉 모든 몬스터들에 대해
                    {
                        float d = Vector2.Distance(transform.position, manager.transform.GetChild(0).GetChild(i).GetChild(j).transform.position); //몬스터까지의 거리 일단 저장

                        if (d <= nowdist) //그 거리가 현재 지정된 타겟까지의 거리보다 작으면
                        {
                            nowdist = d; //가까운 거리 저장
                            nowtarget = manager.transform.GetChild(0).GetChild(i).GetChild(j).transform; //가까운 타겟으로 변경
                        }
                    }
                }
            }
        }

        if (nowdist < 5) target = nowtarget; //공격 범위 5 내에 최종 결정된 타겟이 존재하면 해당 몬스터를 타겟으로 지정

        if (!manager.making) CancelInvoke(nameof(ShootBullet)); //전투가 끝났다면 ShootBullet() 함수 반복 실행 중단

    } //Update End


    void ShootBullet() //타겟이 있다면, 오염 탄알을 자기 위치에서 타겟을 바라보는 방향의 각도로 생성
    {
        if (target != null) Instantiate(pollutingbullet, transform.position,
                Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x)));
    }

} //AutoAttack
