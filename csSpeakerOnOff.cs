using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csSpeakerOnOff : MonoBehaviour
{
    Camera arCamera;        // ARCamera 게임오브젝트에 포함된 Camera컴포넌트를 얻어오기 위한 변수
    AudioSource audioSource;      // 이 소스가 포함된 GameObject에 함께 포함된 AusioSource컴포넌트를 얻어오기 위한 변수
    bool isOn = false;      // true면 소리가 발생, false면 소리가 꺼짐
    public Object[] mats;   // 2개의 머티리얼을 외부에서 추가, 0 index는 소리가 꺼졌을 때 머티리얼, 1 index는 소리가 켜졌을 때 머티리얼
    Renderer render;        // 머티러얼을 적용하기 위해 Mesh Render 컴포넌트를 얻어오기 위한 변수
    public Object pointLightResource;   // 포인트 라이트 프리팹
    GameObject pointLightObj = null;    // 포인트 라이트 프리팹으로 생성한 GameObject

    void Start()
    {
        audioSource = GetComponent<AudioSource>();        // 이 GameObject에 포함된 AudioSource 컴포넌트를 얻어옴
        arCamera = GameObject.Find("First Person Camera").GetComponent<Camera>(); // 현재 씬에서 "First Person Camera" 이름의 GameObject를 찾아서 담아라
        render = GetComponent<Renderer>();          // 이 GameObject에 포함된 Renderer 컴포넌틀 얻어옴
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0) return;      // 터치가 발생하지 않았다면 더 이상 아래를 진행하지 말 것

        Touch touch = Input.GetTouch(0);        // 첫 번째 터치 정보 추출

        Vector3 mousePosition = Input.mousePosition;    // 터치한 위치를 얻는다
        Ray ray = arCamera.ScreenPointToRay(mousePosition);     // ARCamera위치에서 광선을 터치한 위치로 발사한 정보를 담을 객체
        RaycastHit hit;                                 // ray와 충돌한 GameObject의 정보가 담기는 객체

        Debug.Log("-------------- " + mousePosition.ToString());

        // 터치가 시작되었고 And 카메라로부터 발사한 광선에 충돌한 GameObject가 있다면
        if (touch.phase == TouchPhase.Began && Physics.Raycast(ray, out hit))   
        {
            Debug.Log("-------------- SpeakerOnOff - Touch");

            if (hit.collider.tag != "Speaker") return;              // 충돌한 GameObject가 "Speaker"태그가 아니라면 더이상 진행하지 않고 return한다
            if (hit.collider.gameObject != this.gameObject) return; // 충돌한 GameObject와 현재 GameObject가 같지 않으면 더이상 진행하지 않고 return한다

            if (isOn)               // 현재 소리가 켜진 상태라면
            {
                Debug.Log("-------------- SpeakerOnOff - Speaker - isOn");
                audioSource.Stop();       // 소리를 꺼라
                isOn = false;       // 소리가 꺼졌다는 상태를 저장
                render.material = (Material)mats[1];    // 소리가 꺼졌을 때 머티리얼을 적용

                // 포인트 라이트 GameObject 생성할 위치를 현재 오브젝트의 위치보다 y축으로 10정도 위에 위치시킨다
                Vector3 pointPosition = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
                // 포인트 라이트 프리팹으로 포인트 라이트 GameObject를 생성하고, pointPosition위치와 이 GameObject의 각도와 일치시킨다
                pointLightObj = (GameObject)Instantiate(pointLightResource, pointPosition, transform.rotation);
            }
            else               // 현재 소리가 꺼진 상태라면
            {
                Debug.Log("-------------- SpeakerOnOff - Speaker - isOn");
                audioSource.Play();   // 현재 소리를 다시 켜라
                isOn = true;    // 소리가 켜졌다는 상태를 저장
                render.material = (Material)mats[0];    // 소리가 켜졌을 때 머티리얼을 적용
                if (pointLightObj != null)              // 포인트 라이트 GameObject가 생성되어있다면
                {
                    Destroy(pointLightObj);             // 포인트 라이트 GameObject를 제거해라
                    pointLightObj = null;               // 포인트 라이트 GameObject 저장 변수에 null을 저장한다
                }
            }
        }
    }
}
