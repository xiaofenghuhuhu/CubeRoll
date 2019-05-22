using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameracontorller : MonoBehaviour
{
    //自身旋转的速度
    public float rotateSpeed = 50;
    //缩放速度
    public float fieldOfViewSpeed = 50;
    //绕物体旋转的速度
    public float rotateLookForwardSpeed = 50;
    //声明水平、垂直位移
    float horizontal, vertical;
    //声明摄像机旋转的对象
    public Transform targetTF;
    //声明摄像机
    Camera camera;
    Vector3 center;
    void Start()
    {
        //初始化摄像机
        camera = this.transform.GetComponent<Camera>();
        InitCamera();
        targetTF = GameObject.Find("Ground").transform;
        center = new Vector3(2f, 0f, 1.5f);
    }
    void Update()
    {
        RotateByMouse();
        ChangeFieldOfView();
        RotateAndLookForward();
    }
    //获取水平以及垂直位移
    private void GetMouseXY()
    {
        horizontal = Input.GetAxis("Mouse X");
        vertical = Input.GetAxis("Mouse Y");
    }
    //缩放方法
    private void ChangeFieldOfView()
    {
        float value = Input.GetAxis("Mouse ScrollWheel");
        if (value > 0f && camera.fieldOfView > 70 || value < 0f && camera.fieldOfView < 40)
        {
            value = 0f;
        }
        camera.fieldOfView += value * fieldOfViewSpeed * Time.deltaTime;
    }
    //自身旋转方法
    private void RotateByMouse()
    {
        if (Input.GetMouseButton(1))
        {
            GetMouseXY();
            this.transform.Rotate(-vertical * Time.deltaTime * rotateSpeed, horizontal * Time.deltaTime * rotateSpeed, 0);
        }
    }
    //绕物体旋转方法
    private void RotateAndLookForward()
    {
        if (Input.GetMouseButton(0))
        {
            GetMouseXY();
            this.transform.RotateAround(center, Vector3.up, horizontal * Time.deltaTime * rotateLookForwardSpeed);
            this.transform.RotateAround(center, Vector3.right, -vertical * Time.deltaTime * rotateLookForwardSpeed);

        }
    }

    private void InitCamera()
    {
        this.transform.position = new Vector3(2, 2, -7);
    }
}

