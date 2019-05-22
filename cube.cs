using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube : MonoBehaviour
{
    private int[, ,] map;
    public GameObject player;  
	public Material otherMaterial;  

    string underPlayerName;
    Vector3 vector = new Vector3();

    public float velocity;
    private bool firstPressFLAG = true;
    // 标志位记录绿色方块下一步可选的三种状态（1，2，3）
    private int goFrontFlag = 0;
    private int goBackFlag = 0;
    private int goLeftFlag = 0;
    private int goRightFlag = 0;
    private int goFrontAngle = 0;
    private int goBackAngle = 0;
    private int goLeftAngle = 0;
    private int goRightAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 生成地板
        createGround();
        // 生成地图
        createGameMap();
        // 生成起始方块
        player = createPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        inputHandler();
    }
    // 生成地板
    void createGround()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "Ground";
        obj.transform.localScale = new Vector3(25f, 1f, 25f);
        obj.transform.position = new Vector3(0.0f, -1.25f, 0.0f);
        obj.GetComponent<MeshRenderer>().material.color = Color.black;
        // 禁止触发碰撞
        obj.GetComponent<BoxCollider>().isTrigger = false;
        obj.AddComponent<Rigidbody>().useGravity = false;
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

    }
    // 生成地图
    void createGameMap()
    {
        map = new int[2, 4, 5] 
        { 
            {{1,1,1,1,1},{1,0,1,0,2},{0,1,1,1,0},{0,1,1,1,0}},
            {{0,0,0,0,0},{0,0,0,2,1},{0,0,0,0,1},{0,0,0,0,0}},
        };
        for (int z = 0; z < map.GetLength(0); z++) 
        {
            for (int y = 0; y < map.GetLength(1); y++) 
            {
                for (int x = 0; x < map.GetLength(2); x++) 
                {
                    if (map[z, y, x] == 1) 
                    {
                        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        obj.transform.localScale = new Vector3(1, 1, 1);
                        obj.transform.position = new Vector3(x, z, y);
                        obj.name = "Cube_" + x.ToString() + "-" + z.ToString() + "-" + y.ToString();
                        obj.GetComponent<MeshRenderer>().material.color = Color.gray;
                        obj.GetComponent<BoxCollider>().isTrigger = false;
                        obj.AddComponent<Rigidbody>().useGravity = false;
                        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }
                    else if (map[z, y, x] == 2)
                    {
                        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        obj.transform.localScale = new Vector3(1, 1, 1);
                        obj.transform.position = new Vector3(x, z, y);
                        obj.name = "Cube_" + x.ToString() + "-" + z.ToString() + "-" + y.ToString();
                        obj.GetComponent<MeshRenderer>().material.color = Color.red;
                        obj.GetComponent<BoxCollider>().isTrigger = false;
                        obj.AddComponent<Rigidbody>().useGravity = false;
                        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }
                } 
            }
        }
    }
    // 生成可操纵的方块
    GameObject createPlayer() 
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "Player";
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.position = new Vector3(3, 1, 0);
        obj.GetComponent<MeshRenderer>().material.color = Color.green;
        obj.GetComponent<BoxCollider>().isTrigger = false;
        obj.AddComponent<Rigidbody>().useGravity = false;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        return obj;
    }
    // 滑块消失
    void rmvCube(string cubeName)
    {
        string pos = cubeName.Split('_')[1];
        double xf = double.Parse(pos.Split('-')[0]);
        double yf = double.Parse(pos.Split('-')[1]);
        double zf = double.Parse(pos.Split('-')[2]);

        int x = (int)Math.Round(xf, 0);
        int y = (int)Math.Round(yf, 0);
        int z = (int)Math.Round(zf, 0);

        map[y, z, x] = 0;
        // Debug.Log(cubeName);
        GameObject underPlayerCube = GameObject.Find(cubeName);
        underPlayerCube.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        underPlayerCube.GetComponent<BoxCollider>().isTrigger = true;
        // underPlayerCube.GetComponent<Rigidbody>().useGravity = true;

        GameObject.Destroy(underPlayerCube);
    }
    string getRmvPlayerCubeName(GameObject cube)
    {
        string playerName = player.name;
        int x = (int)cube.transform.position.x;
        int y = (int)cube.transform.position.y - 1;
        int z = (int)cube.transform.position.z;  
        // Cube_x-z-y
        string underPlayerName = "Cube_" + x.ToString() + "-" + y.ToString() + "-" + z.ToString();
        if (map[y, z, x] == 2) underPlayerName = "red";
        return underPlayerName;
    }

    Vector3 vecF2I(Vector3 v) 
    {
        // 3D向量浮点整数型转换
        double x = Math.Round(v.x, 0);
        double y = Math.Round(v.y, 0);
        double z = Math.Round(v.z, 0);

        return new Vector3((int)x, (int)y, (int)z);
    }

    void inputHandler() 
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && firstPressFLAG)
        {   // 按下 向上键
            vector = player.transform.position;
            // 已在最顶层
            if ((int)vector.y >= map.GetLength(0))
            {
                if (map[(int)vector.y - 1, (int)vector.z + 1, (int)vector.x] != 0)
                {
                    firstPressFLAG = false;
                    goFrontFlag = 2;
                    goFrontAngle = 0;
                    underPlayerName = getRmvPlayerCubeName(player);
                }
                else if (map[(int)vector.y - 2, (int)vector.z + 1, (int)vector.x] != 0)
                {
                    firstPressFLAG = false;
                    goFrontFlag = 3;
                    goFrontAngle = 0;
                    underPlayerName = getRmvPlayerCubeName(player);
                }
            }
            // 状态1:向上方旋转180
            else if (map[(int)vector.y, (int)vector.z + 1, (int)vector.x] != 0)
            {
                firstPressFLAG = false;
                goFrontFlag = 1;
                goFrontAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }
            // 状态2:向前旋转90
            else if (map[(int)vector.y - 1, (int)vector.z + 1, (int)vector.x] != 0)
            {
                firstPressFLAG = false;
                goFrontFlag = 2;
                goFrontAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }

            // 状态3:向下方旋转180
            else if (map[(int)vector.y - 2, (int)vector.z + 1, (int)vector.x] != 0)
            {
                firstPressFLAG = false;
                goFrontFlag = 3;
                goFrontAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }

        }
        if (goFrontFlag == 1)
        {
            goFrontAngle += 6;
            if (goFrontAngle <= 180)
            {
                player.transform.RotateAround(vector + new Vector3(0.0f, 0.5f, 0.5f), new Vector3(1, 0, 0), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goFrontFlag = 0;
                goFrontAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }
        if (goFrontFlag == 2)
        {
            goFrontAngle += 6;
            if (goFrontAngle <= 90)
            {
                player.transform.RotateAround(vector + new Vector3(0.0f, -0.5f, 0.5f), new Vector3(1, 0, 0), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goFrontFlag = 0;
                goFrontAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }
        if (goFrontFlag == 3)
        {
            goFrontAngle += 6;
            if (goFrontAngle <= 180)
            {
                player.transform.RotateAround(vector + new Vector3(0.0f, -0.5f, 0.5f), new Vector3(1, 0, 0), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goFrontFlag = 0;
                goFrontAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }


        if (Input.GetKeyDown(KeyCode.DownArrow) && firstPressFLAG)
        {   // 按下 向下键
            vector = player.transform.position;
            if ((int)vector.y >= map.GetLength(0))
            {
                if (map[(int)vector.y - 1, (int)vector.z - 1, (int)vector.x] != 0)
                {
                    firstPressFLAG = false;
                    goBackFlag = 2;
                    goBackAngle = 0;
                    underPlayerName = getRmvPlayerCubeName(player);
                }
                else if (map[(int)vector.y - 2, (int)vector.z - 1, (int)vector.x] != 0)
                {
                    firstPressFLAG = false;
                    goBackFlag = 3;
                    goBackAngle = 0;
                    underPlayerName = getRmvPlayerCubeName(player);
                }
            }
            // 状态1:向上方旋转180
            else if (map[(int)vector.y, (int)vector.z - 1, (int)vector.x] != 0)
            {
                firstPressFLAG = false;
                goBackFlag = 1;
                goBackAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }
            // 状态2:向前旋转90
            else if (map[(int)vector.y - 1, (int)vector.z - 1, (int)vector.x] != 0)
            {
                firstPressFLAG = false;
                goBackFlag = 2;
                goBackAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }
            // 状态3:向下方旋转180
            else if (map[(int)vector.y - 2, (int)vector.z - 1, (int)vector.x] != 0)
            {
                firstPressFLAG = false;
                goBackFlag = 3;
                goBackAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }

        }
        if (goBackFlag == 1)
        {
            goBackAngle += 6;
            if (goBackAngle <= 180)
            {
                player.transform.RotateAround(vector + new Vector3(0.0f, 0.5f, -0.5f), new Vector3(-1, 0, 0), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goBackFlag = 0;
                goBackAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }
        if (goBackFlag == 2)
        {
            goBackAngle += 6;
            if (goBackAngle <= 90)
            {
                player.transform.RotateAround(vector + new Vector3(0.0f, -0.5f, -0.5f), new Vector3(-1, 0, 0), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goBackFlag = 0;
                goBackAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }

        if (goBackFlag == 3)
        {
            goBackAngle += 6;
            if (goBackAngle <= 180)
            {
                player.transform.RotateAround(vector + new Vector3(0.0f, -0.5f, -0.5f), new Vector3(-1, 0, 0), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goBackFlag = 0;
                goBackAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow) && firstPressFLAG)
        {   // 按下 向左键
            vector = player.transform.position;
            if ((int)vector.y >= map.GetLength(0))
            {
                if (map[(int)vector.y - 1, (int)vector.z, (int)vector.x - 1] != 0)
                {
                    firstPressFLAG = false;
                    goLeftFlag = 2;
                    goLeftAngle = 0;
                    underPlayerName = getRmvPlayerCubeName(player);
                }
                else if (map[(int)vector.y - 2, (int)vector.z, (int)vector.x - 1] != 0)
                {
                    firstPressFLAG = false;
                    goLeftFlag = 3;
                    goLeftAngle = 0;
                    underPlayerName = getRmvPlayerCubeName(player);
                }
            }
            // 状态1:向上方旋转180
            else if (map[(int)vector.y, (int)vector.z, (int)vector.x - 1] != 0)
            {
                firstPressFLAG = false;
                goLeftFlag = 1;
                goLeftAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }
            // 状态2:向前旋转90
            else if (map[(int)vector.y - 1, (int)vector.z, (int)vector.x - 1] != 0)
            {
                firstPressFLAG = false;
                goLeftFlag = 2;
                goLeftAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }
            // 状态3:向下方旋转180
            else if (map[(int)vector.y - 2, (int)vector.z, (int)vector.x - 1] != 0)
            {
                firstPressFLAG = false;
                goLeftFlag = 3;
                goLeftAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }

        }
        if (goLeftFlag == 1)
        {
            goLeftAngle += 6;
            if (goLeftAngle <= 180)
            {
                player.transform.RotateAround(vector + new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(0, 0, 1), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goLeftFlag = 0;
                goLeftAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }
        if (goLeftFlag == 2)
        {
            goLeftAngle += 6;
            if (goLeftAngle <= 90)
            {
                player.transform.RotateAround(vector + new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0, 0, 1), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goLeftFlag = 0;
                goLeftAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }
        if (goLeftFlag == 3)
        {
            goLeftAngle += 6;
            if (goLeftAngle <= 180)
            {
                player.transform.RotateAround(vector + new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0, 0, 1), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goLeftFlag = 0;
                goLeftAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }


        if (Input.GetKeyDown(KeyCode.RightArrow) && firstPressFLAG)
        {   // 按下 向右键
            vector = player.transform.position;
            if ((int)vector.y >= map.GetLength(0))
            {
                if (map[(int)vector.y - 1, (int)vector.z, (int)vector.x + 1] != 0)
                {
                    firstPressFLAG = false;
                    goRightFlag = 2;
                    goRightAngle = 0;
                    underPlayerName = getRmvPlayerCubeName(player);
                }
                else if (map[(int)vector.y - 2, (int)vector.z, (int)vector.x + 1] != 0)
                {
                    firstPressFLAG = false;
                    goRightFlag = 3;
                    goRightAngle = 0;
                    underPlayerName = getRmvPlayerCubeName(player);
                }
            }
            // 状态1:向上方旋转180
            else if (map[(int)vector.y, (int)vector.z, (int)vector.x + 1] != 0)
            {
                firstPressFLAG = false;
                goRightFlag = 1;
                goRightAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }
            // 状态2:向前旋转90
            else if (map[(int)vector.y - 1, (int)vector.z, (int)vector.x + 1] != 0)
            {
                firstPressFLAG = false;
                goRightFlag = 2;
                goRightAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }

            // 状态3:向下方旋转180
            else if (map[(int)vector.y - 2, (int)vector.z, (int)vector.x + 1] != 0)
            {
                firstPressFLAG = false;
                goRightFlag = 3;
                goRightAngle = 0;
                underPlayerName = getRmvPlayerCubeName(player);
            }

        }
        if (goRightFlag == 1)
        {
            goRightAngle += 6;
            if (goRightAngle <= 180)
            {
                player.transform.RotateAround(vector + new Vector3(0.5f, 0.5f, 0.0f), new Vector3(0, 0, -1), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goRightFlag = 0;
                goRightAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }
        if (goRightFlag == 2)
        {
            goRightAngle += 6;
            if (goRightAngle <= 90)
            {
                player.transform.RotateAround(vector + new Vector3(0.5f, -0.5f, 0.0f), new Vector3(0, 0, -1), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goRightFlag = 0;
                goRightAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }

        if (goRightFlag == 3)
        {
            goRightAngle += 6;
            if (goRightAngle <= 180)
            {
                player.transform.RotateAround(vector + new Vector3(0.5f, -0.5f, 0.0f), new Vector3(0, 0, -1), 6);
            }
            else
            {
                // 调整姿态
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                player.transform.position = vecF2I(player.transform.position);
                firstPressFLAG = true;
                goRightFlag = 0;
                goRightAngle = 0;
                if (underPlayerName == "red") { }
                else rmvCube(underPlayerName);
            }
        }
    }
}




