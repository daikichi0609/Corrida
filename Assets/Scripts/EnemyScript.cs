using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    //プレイヤー
    public GameObject Player;
    public PlayerScript PlayerScript;
    //プレイヤーが元いた場所
    public GameObject PlayerOldPos;

    public NavMeshAgent agent;
    Animator animator;
    float speed;
    int num;

    public int Anger;
    public int oldAnger;
    public bool isRushing;

    public bool DestinationSetOn;



    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Anger > 0)
        {
            if(DestinationSetOn)
            {
                return;
            }
            // 補完スピードを決める
            float speed = 0.2f;
            // ターゲット方向のベクトルを取得
            Vector3 relativePos = Player.transform.position - this.transform.position;
            // 方向を、回転情報に変換
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            // 現在の回転情報と、ターゲット方向の回転情報を補完する
            transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, speed);
        }
        if(Anger >= 5)
        {
            if (DestinationSetOn)
            {
                return;
            }
            //NavMesh停止
            agent.isStopped = false;
            isRushing = true;
            DestinationSetOn = true;
        }
        if(isRushing)
        {
            isRushing = false;
            PlayerOldPos.transform.position = Player.transform.position;
            animator.SetBool("isRunning", true);
            Vector3 tmp = PlayerOldPos.transform.position;
            float x = tmp.x;
            float y = tmp.y;
            float z = tmp.z;
            //目的地を設定してあげる
            agent.SetDestination(tmp);//☆追加
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "OldPlayerPos")
        {
            Debug.Log("a");
            isRushing = false;
            DestinationSetOn = false;
            animator.SetBool("isRunning", false);
            //NavMesh停止
            agent.isStopped = true;
            Anger = 0;
        }
    }
}
