using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//CharacterControllerを入れる
[RequireComponent(typeof(CharacterController))]

public class PlayerScript: MonoBehaviour
{
    //敵
    public EnemyScript Enemy;
    //ゲームマネージャー
    public GameManagerScript GameManager;
    //落ちる速さ、重力
    public float gravity = 100f;
    private float jumpSpeed;

    //通常時のスピードとジャンプ力
    public float normalSpeed = 20;
    public float normalJump = 80;
    //左Shift押したときのスピードとジャンプ力
    public float shiftSpeed = 20;
    public float shiftJump = 80;

    //Playerの移動や向く方向を入れる
    Vector3 moveDirection;

    //CharacterControllerを変数にする
    CharacterController controller;

    //マウスの横縦に動かす速さ
    public float horizontalSpeed = 2.0F;
    public float verticalSpeed = 2.0F;

    //ウシ
    public GameObject targetObject;

    //赤マント
    public GameObject RedCloth;
    public bool isShaking;
    public float timer;

    // ワールド座標を基準に、座標を取得
    Vector3 worldPos;
    public float Posx;
    public float Posy;
    public float Posz;

    float h;
    float v;

    float Slowtimer;
    public bool isSlowing;

    public GameObject ClothSound;
    public GameObject SlowStart;
    public GameObject SlowFinish;

    //Main Cameraを入れる
    GameObject came;

    void Start()
    {
        //CharacterControllerを取得
        controller = GetComponent<CharacterController>();

        /*
        //Main Cameraを検索し子オブジェクトにしてPlayerに設置する
        came = Camera.main.gameObject;
        came.transform.parent = this.transform;
        //カメラを目線の高さにする
        came.transform.localPosition = new Vector3(0, 0.4f, 0);
        //カメラの向きをこのオブジェクトと同じにする
        came.transform.rotation = this.transform.rotation;
        */

        //カーソルロック
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        //スローモーション時間
        if(isSlowing)
        {
            SlowStart.SetActive(true);
            SlowFinish.SetActive(true);
            Time.timeScale = 0.1f;
            Slowtimer += Time.deltaTime;
            if(Slowtimer >= 0.2f)
            {
                SlowStart.SetActive(false);
                SlowFinish.SetActive(false);
                //SlowFinish.SetActive(true);
                //Enemy.agent.speed = 100;
                Time.timeScale = 1.0f;
                Slowtimer = 0f;
                isSlowing = false;
            }
        }
        //マントを振る
        if (isShaking)
        {
            ClothSound.SetActive(true);
            timer += Time.deltaTime;
            RedCloth.SetActive(true);
            worldPos = RedCloth.transform.position;
            Posx = worldPos.x;    // ワールド座標を基準にした、x座標が入っている変数
            Posy = 4 + 2 * Mathf.Sin(Time.time * 10);
            Posz = worldPos.z;    // ワールド座標を基準にした、z座標が入っている変数
            RedCloth.transform.position = new Vector3(Posx, Posy, Posz);

            if (timer >= 1f)
            {
                ClothSound.SetActive(false);
                isShaking = false;
                timer = 0f;
                Enemy.Anger++;
            }
        }
        else if (!isShaking)
        {
            RedCloth.SetActive(false);
        }

        //視点を常にウシに向ける
        //this.transform.LookAt(targetObject.transform);

        //HorizontalとVertical
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        // 補完スピードを決める
        float speed = 0.2f;
        // ターゲット方向のベクトルを取得
        Vector3 relativePos = targetObject.transform.position - this.transform.position;
        // 方向を、回転情報に変換
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        // 現在の回転情報と、ターゲット方向の回転情報を補完する
        transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);

        //左右どちらかのShift押した場合と離している場合
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = shiftSpeed;
            jumpSpeed = shiftJump;
        }
        else
        {
            speed = normalSpeed;
            jumpSpeed = normalJump;
        }

        //マウスでカメラの向きとPlayerの横の向きを変える
        //float h = horizontalSpeed * Input.GetAxis("Mouse X");
        //float v = verticalSpeed * Input.GetAxis("Mouse Y");
        //transform.Rotate(0, h, 0);
        //came.transform.Rotate(v, 0, 0);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (isSlowing)
            {
                // Spin the object around the world origin at 20 degrees/second.
                transform.RotateAround(Enemy.transform.position, Vector3.up, 700 * Time.deltaTime);
                return;
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (isSlowing)
            {
                // Spin the object around the world origin at 20 degrees/second.
                transform.RotateAround(Enemy.transform.position, Vector3.down, 700 * Time.deltaTime);
                return;
            }
        }

        //Playerが地面に設置していることを判定
        if (controller.isGrounded)
        {
            if(isShaking)
            {
                return;
            }
            //XZ軸の移動と向きを代入する
            //WASD,上下左右キー
            if(!isSlowing)
            {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0,
                                    Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;

                // Y軸方向にジャンプさせる
                if (Input.GetButtonDown("Jump"))
                    moveDirection.y = jumpSpeed;
            }
        }

        // 重力を設定しないと落下しない
        moveDirection.y -= gravity * Time.deltaTime;

        // Move関数に代入する
        controller.Move(moveDirection * Time.deltaTime);

        //bool切替
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!isShaking)
            {
                isShaking = true;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Slow")
        {
            if(!Enemy.DestinationSetOn)
            {
                return;
            }
            isSlowing = true;
        }
    }
}
