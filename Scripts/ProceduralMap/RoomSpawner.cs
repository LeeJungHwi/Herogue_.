using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    // 문이 열리는 방향
    // 1 : 아래쪽문이 열려있는 방이 필요한 방이다
    // 2 : 위쪽문이 열려있는 방이 필요한 방이다
    // 3 : 왼쪽문이 열려있는 방이 필요한 방이다
    // 4 : 오른쪽문이 열려있는 방이 필요한 방이다
    [SerializeField] private int openingDirection;

    // 랜덤맵 오브젝트 풀링
    private PoolingManager poolingManager;

    // 방 모델 가져오기
    private RoomTemplates templates;

    // 랜덤방을 붙이기 위해
    private int rand;

    // 스폰되었는지 체크
    [HideInInspector] public bool spawned = false;

    // 맵 0.1초 뒤에 생성
    [HideInInspector] public float waitTime = 0.1f;

    private void Start()
    {
        // 방 모델
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();

        // 오브젝트 풀링
        poolingManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolingManager>();
    }

    private void Update()
    {
        // 대기시간이 0이하일때
        if(waitTime <= 0)
        {
            // 방이 스폰이 되지 않았을때
            if (spawned == false)
            {
                // 방 생성 제한 부분
                // 기본스테이지 + (현재스테이지 / 스테이지계수) * 스테이지계수
                // 0~2 스테이지는 5개까지, 3~5 스테이지는 8개까지, 6~8 스테이지는 11개까지, ...
                if (templates.rooms.Count > templates.baseStage + templates.currentStage / templates.stageCoef * templates.stageCoef)
                {
                    // 스폰상태
                    spawned = true;
                    return;
                }

                // 각 방향 방 생성
                if (openingDirection == 1) RoomSpawn(templates.bottomRooms);
                else if (openingDirection == 2) RoomSpawn(templates.topRooms);
                else if (openingDirection == 3) RoomSpawn(templates.leftRooms);
                else if (openingDirection == 4) RoomSpawn(templates.rightRooms);

                // 스폰상태
                spawned = true;
            }
        }
        else
        {
            // 대기시간 감소
            waitTime -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 이미 생성된곳에 비밀방 생성
        if(other.CompareTag("SpawnPoint"))
        {
            try
            {
                if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
                {
                    // 시크릿방 생성
                    GameObject instanceSecretRoom = poolingManager.GetObj(ObjType.SecretRoom);
                    instanceSecretRoom.transform.position = transform.position;
                    instanceSecretRoom.transform.rotation = poolingManager.RandomMapPrefs[18].transform.rotation;

                    // 시크릿 문 원래대로
                    Transform[] transforms = instanceSecretRoom.GetComponentsInChildren<Transform>();
                    for (int j = 0; j < transforms.Length; j++)
                    {
                        if (transforms[j].name == "SecretDoorLeft")
                        {
                            // 왼쪽문
                            if (transforms[j].transform.GetComponent<SecretDoor>().isOpen == true)
                            {
                                // 열렸다면
                                transforms[j].gameObject.transform.rotation *= Quaternion.Euler(0, -90f, 0);

                                // 플래그
                                transforms[j].transform.GetComponent<SecretDoor>().isOpen = false;
                            }
                        }

                        if (transforms[j].name == "SecretDoorRight")
                        {
                            // 오른쪽문
                            if (transforms[j].transform.GetComponent<SecretDoor>().isOpen == true)
                            {
                                // 열렸다면
                                transforms[j].gameObject.transform.rotation *= Quaternion.Euler(0, 90f, 0);

                                // 플래그
                                transforms[j].transform.GetComponent<SecretDoor>().isOpen = false;
                            }
                        }
                    }

                    // 시크릿박스 생성
                    SecretBoxSpawn(new Vector3(0, 5f, 0));
                    SecretBoxSpawn(new Vector3(0, 5f, 15f));
                    SecretBoxSpawn(new Vector3(0, 5f, -15f));
                    SecretBoxSpawn(new Vector3(15f, 5f, 0));
                    SecretBoxSpawn(new Vector3(-15f, 5f, 0));
                }
            }
            catch (System.Exception e)
            {
                // 예외던지기 : 널 에러
                throw e;
            }
            // 스폰상태
            spawned = true;
        }
    }

    private void RoomSpawn(GameObject[] roomType)
    {
        // 방 생성 함수
        rand = Random.Range(0, roomType.Length);
        GameObject randomRoom = roomType[rand];
        GameObject instantRoom = poolingManager.GetObj(randomRoom.GetComponent<AddRoom>().type);
        instantRoom.transform.position = transform.position;
        instantRoom.transform.rotation = roomType[rand].transform.rotation;
    }

    private void SecretBoxSpawn(Vector3 spawnPosition)
    {
        // 시크릿박스 생성 함수
        rand = Random.Range(0, 2);
        if (rand == 0)
        {
            GameObject instantSecretBox = poolingManager.GetObj(ObjType.시크릿박스);
            instantSecretBox.transform.position = transform.position + spawnPosition;
            instantSecretBox.transform.rotation = poolingManager.SecretBoxPrefs[0].transform.rotation;
        }
    }
}
