using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaronGameManager : MonoBehaviour
{
    public GameObject player;

    public Transform redPoint;
    public Transform bluePoint;

    public PlayerFSM playerFSM;

    void Start()
    {
        // 씬이 시작되면 포인트 추첨 함수를 호출한다.
        print("추첨 시작");
        RandomPoint();
    }

    void Update()
    {

    }

    // 랜덤으로 플레이어가 생성될 포인트를 추첨하는 함수
    void RandomPoint()
    {
        Transform spwanPoint;
        // 0과 2사이의 랜덤 숫자를 추첨한다. (0 or 1)
        int randomPoint = Random.Range(0, 2);
        // 만약 랜덤 숫자가 0이라면
        if (randomPoint == 0)
        {
            print(randomPoint);

            // 스폰포인트는 레드포인트다.
            spwanPoint = redPoint;
            print(spwanPoint);

        }
        else // 아니라면
        {
            print(randomPoint);
            // 스폰포인트는 블루포인트다.
            spwanPoint = bluePoint;
            print(spwanPoint);

        }
        // 정해진 스폰포인트를 매개변수로 스폰플레이어 함수를 호출한다.
        SpwanPlayer(spwanPoint);
        print("스폰플레이어 함수 호출");

    }

    // 플레이어를 생성하는 함수
    void SpwanPlayer(Transform point)
    {
        // 플레이어 오브젝트 py의 위치를 추첨한 포인트로 한다.
        GameObject py = Instantiate(player, point);
        print("플레이어 생성 완료");
        playerFSM = py.GetComponent<PlayerFSM>();

    }
}
