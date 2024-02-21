using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera myCamera;
    Collider myCollider;
    float inicialCameraSize;
    Vector3 inicialPosition, inicialPositionCollider;
    int speed;
    bool growCamera;
    float timer;

    void Awake()
    {
        myCamera = GetComponent<Camera>();
        myCollider = GetComponent<Collider>();
        inicialCameraSize = myCamera.orthographicSize;
        inicialPosition = myCamera.transform.position;
        inicialPositionCollider = myCollider.transform.position;
        growCamera = false;
        speed = 20;
        timer = 0;
    }


    void Update()
    {

        if (growCamera)
        {
            myCamera.orthographicSize = Mathf.Lerp(myCamera.orthographicSize, inicialCameraSize + speed, Time.deltaTime);
            myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, inicialPosition + Vector3.up * speed, Time.deltaTime);
            myCamera.transform.localScale = Vector3.Lerp(myCamera.transform.localScale, Vector3.one + (Vector3.left + Vector3.up) * speed, Time.deltaTime);
            //myCollider.transform.position = Vector3.Lerp(myCollider.transform.position, inicialPositionCollider + Vector3.up * speed, Time.deltaTime);
            if (timer > 0) timer -= Time.deltaTime;
            else { 
                timer = 0;
                growCamera = false;
            }
        }

    }

    void OnTriggerStay(Collider collision)
    {
        if (timer > 0) return;
        growCamera = true;
        timer = 0.5f;
        Debug.Log("ENTER");
    }

    public void resetSize()
    {
        myCamera.orthographicSize = inicialCameraSize;
        myCamera.transform.position = inicialPosition;
        myCamera.transform.localScale =  Vector3.one;
        //myCollider.transform.position = inicialPositionCollider;
        growCamera = false;
        timer = 0;
        Debug.Log("RESET");
    }
}
