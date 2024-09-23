using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class MediaPipeClient : MonoBehaviour
{
    
    // 서버 주소
    private string baseUrl = " http://meta-ai.iptime.org/";

    // 캐릭터의 관절을 참조하기 위한 변수들
    public Transform leftElbowTransform;
    public Transform rightElbowTransform;
    public Transform leftShoulderTransform;
    public Transform rightShoulderTransform;

    private CoordinateData coordinateData;
    private AngleData angleData;

    void Start()
    {
        // 좌표 및 각도 데이터를 받아오는 코루틴 함수 호출
        StartCoroutine(GetCoordinates());
       // StartCoroutine(GetAngles());
    }

    public List<Vector3> testBodyPos = new List<Vector3>();
    // 좌표 데이터를 서버에서 가져오는 코루틴
    IEnumerator GetCoordinates()
    {
        string url = $"{baseUrl}/coordinates";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            // 요청 결과에 따라 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;

                // 제이슨 데이터 출력
                Debug.Log($"Coordinates JSON Response: {jsonResponse}");
                JObject jsonData = JObject.Parse(jsonResponse);
                for(int i = 0; i < 32; i++)
                {
                    string key = "POSE_" + i;
                    JObject pose = jsonData[key].ToObject<JObject>();
                    Vector3 pos = JsonUtility.FromJson<Vector3>(pose.ToString());
                    testBodyPos.Add(pos);

                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.transform.position = pos * 6;
                    go.transform.localScale = Vector3.one ;

                    //print(pose.ToString());
                }


                yield break;
                coordinateData = JsonUtility.FromJson<CoordinateData>(jsonResponse);

                // 데이터가 정상적으로 파싱되었는지 확인
                if (coordinateData != null && coordinateData.landmarks != null)
                {
                    Debug.Log("Coordinate Data Parsed Successfully.");
                    Debug.Log($"Landmarks Count: {coordinateData.landmarks.Length}");

                    // 각 랜드마크의 좌표를 출력
                    for (int i = 0; i < coordinateData.landmarks.Length; i++)
                    {
                        Debug.Log($"Landmark {i}: x={coordinateData.landmarks[i].x}, y={coordinateData.landmarks[i].y}, z={coordinateData.landmarks[i].z}");
                    }

                    UpdateCharacterPose(); // 캐릭터 포즈 업데이트
                }
                else
                {
                    Debug.LogWarning("Failed to parse coordinateData or landmarks array is null.");
                }
            }
            else
            {
                // 요청 오류 처리
                Debug.LogError($"Error: {request.error}");
            }
        }
    }

    // 각도 데이터를 서버에서 가져오는 코루틴
    IEnumerator GetAngles()
    {
        string url = $"{baseUrl}/angles";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            // 요청 결과에 따라 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;

                // 제이슨 데이터 출력
                Debug.Log($"Angles JSON Response: {jsonResponse}");

                angleData = JsonUtility.FromJson<AngleData>(jsonResponse);

                // 데이터가 정상적으로 파싱되었는지 확인
                if (angleData != null)
                {
                    Debug.Log("Angle Data Parsed Successfully.");
                    Debug.Log($"Angle1: {angleData.angle1}, Angle2: {angleData.angle2}");

                    UpdateCharacterPose(); // 캐릭터 포즈 업데이트
                }
                else
                {
                    Debug.LogWarning("Failed to parse angleData.");
                }
            }
            else
            {
                // 요청 오류 처리
                Debug.LogError($"Error: {request.error}");
            }
        }
    }

    // 캐릭터의 포즈를 업데이트하는 함수
    void UpdateCharacterPose()
    {
        if (coordinateData != null && angleData != null && coordinateData.landmarks != null)
        {
            if (coordinateData.landmarks.Length > 1) // 적어도 두 개의 랜드마크가 있는지 확인
            {
                // 왼쪽 팔꿈치와 오른쪽 팔꿈치의 위치 및 각도를 업데이트
                if (leftElbowTransform != null && coordinateData.landmarks.Length > 0)
                {
                    UpdateJointPosition(leftElbowTransform, coordinateData.landmarks[0], angleData.angle1);
                }

                if (rightElbowTransform != null && coordinateData.landmarks.Length > 1)
                {
                    UpdateJointPosition(rightElbowTransform, coordinateData.landmarks[1], angleData.angle2);
                }

                // 여기에 leftShoulderTransform과 rightShoulderTransform을 업데이트하는 코드를 추가하세요
            }
        }
    }

    // 관절의 위치와 회전을 업데이트하는 함수
    void UpdateJointPosition(Transform jointTransform, Coordinate coordinate, float angle)
    {
        if (jointTransform != null && coordinate != null) // Null 체크
        {
            Vector3 position = new Vector3(coordinate.x, coordinate.y, coordinate.z);
            jointTransform.position = position;
            jointTransform.rotation = Quaternion.Euler(0, angle, 0); // Y축 기준으로 회전
        }
    }

    // 서버 상태를 확인하는 코루틴 (선택적 기능)
    IEnumerator GetServerHealth()
    {
        string url = $"{baseUrl}/health";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            // 요청 결과에 따라 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"Server Health: {jsonResponse}");
            }
            else
            {
                // 요청 오류 처리
                Debug.LogError($"Error: {request.error}");
            }
        }
    }
}

// 좌표를 나타내는 클래스
[System.Serializable]
public class Coordinate
{
    public float x;
    public float y;
    public float z;
}

// 좌표 데이터를 포함하는 클래스
[System.Serializable]
public class CoordinateData
{
    public Coordinate[] landmarks;
}

// 각도 데이터를 포함하는 클래스
[System.Serializable]
public class AngleData
{
    public float angle1;
    public float angle2;
}
