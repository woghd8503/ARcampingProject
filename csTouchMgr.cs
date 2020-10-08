using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCoreInternal;

public class csTouchMgr : MonoBehaviour
{
    private Camera ARCamera;    //ARCore 카메라
    public Object placeObject; //터치 시 평면에 생성할 프리팹

    private void Start()
    {
        //ARCore Device 프리팹 하위에 있는 카메라를 찾아서 변수에 할당
        ARCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
        Debug.Log("--------------ARCamera : " + ARCamera.ToString());
    }

    private void Update()
    {
        if (Input.touchCount == 0) return;          // 터치가 이루어지지 않았다면 더 이상 진행하지 말고 return해라

        // 첫 번째 터치 정보 추출
        Touch touch = Input.GetTouch(0);

        Vector3 mousePosition = Input.mousePosition;            // 현재 터치한 위치를 얻는다
        Ray ray = ARCamera.ScreenPointToRay(mousePosition);     // ARCamera로부터 발사한 광선 정보
        RaycastHit rHit;                                        // 광선이 부딪히는 GameObject 정보

        Debug.Log("-------------- " + mousePosition.ToString());

        // 터치가 발생하였고 And 광선과 부딪힌 GameObject가 존재한다면
        if (touch.phase == TouchPhase.Began && Physics.Raycast(ray, out rHit))
        {
            // 부딪힌 GameObject의 Tag가 Speaker라면 더 이상 진행하지 말고 return 해라
            if (rHit.collider.tag == "Speaker")
                return;
        }

        //ARCore에서 제공하는 RaycastHit와 유사한 구조체
        TrackableHit hit;

        //검출 대상을 평면 또는 Feature Point로 한정
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

        //터치한 지점으로 레이 발사하고 And 터치한 위치에서 광선을 발사했을 때 평면이나 특징점(Feature Point)과 충돌했다면
        if (touch.phase == TouchPhase.Began && Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            Debug.Log("-------------- Touch");
            //객체를 고정할 앵커를 생성
            var anchor = hit.Trackable.CreateAnchor(hit.Pose);
            //placeObject에 담긴 Prefab 객체를 생성
            GameObject obj = (GameObject)Instantiate(placeObject, hit.Pose.position, Quaternion.identity, anchor.transform);

            // 생성한 객체가 사용자 쪽을 바라보도록 회전값 계산
            var rot = Quaternion.LookRotation(ARCamera.transform.position - hit.Pose.position);

            // 사용자 쪽 회전값 적용
            obj.transform.rotation = Quaternion.Euler(ARCamera.transform.position.x, rot.eulerAngles.y, ARCamera.transform.position.z);
        }        
    }
}
