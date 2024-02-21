using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera myCamera;
    float inicialCameraSize;
    Vector3 inicialPosition, inicialPositionCollider;
    int speed;
    bool growCamera;
    float timer;

    void Awake()
    {
        myCamera = GetComponent<Camera>();
        inicialCameraSize = myCamera.orthographicSize;
        inicialPosition = myCamera.transform.position;
        growCamera = false;
        speed = 50;
        timer = 0;
    }


    void Update()
    {

        if (growCamera)
        {
            myCamera.orthographicSize = myCamera.orthographicSize + (speed * Time.deltaTime);
            myCamera.transform.position = myCamera.transform.position + (Vector3.up * speed * Time.deltaTime);
            myCamera.transform.localScale = myCamera.transform.localScale + ((Vector3.right + Vector3.up) * 5 * Time.deltaTime);

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
        growCamera = false;
        timer = 0;
        Debug.Log("RESET");
    }
}
