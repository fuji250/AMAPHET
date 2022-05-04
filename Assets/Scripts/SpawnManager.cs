using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    //SpawnPoint格納配列作成
    public Transform[] spawnPoints;

    //生成するプレイヤーオブジェクト
    public GameObject playerPrefab;
    //生成したプレイヤーオブジェクト
    GameObject player;
    //生成するゴーストオブジェクト
    public GameObject ghostPrefab;
    //生成したゴーストオブジェクト
    GameObject ghost;

    

    //生成するMotionManagerオブジェクト
    public GameObject motionManagerPrefab;
    
    //生成したプレイヤーオブジェクト
    GameObject motionManager;

    //スポーンまでのインターバル
    public float respawnInterval = 5f;


    bool firstSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        //スポーンオブジェクトを全て非表示
        foreach(Transform position in spawnPoints)
        {
            position.gameObject.SetActive(false);
        }

        //生成関数呼び出し
        if (PhotonNetwork.IsConnected)
        {
            //ネットワークオブジェクトとしてプレイヤーを生成する
            SpawnPlayer();

            SpawnMotionManager();
        }
    }

    //ランダムにスポーンポイントの一つを選択する関数
    public Transform GetSpawnPoint()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            firstSpawned = true;
            return spawnPoints[0];
        }
        else
        {
            return spawnPoints[1];
        }
    }

    //ネットワークオブジェクトとしてプレイヤーを生成する
    public void SpawnPlayer()
    {
        //適切なスポーンポジションを変数に格納
        Transform spawnPoint = GetSpawnPoint();
        //ネットワークオブジェクト生成
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position,
            spawnPoint.rotation);
        //ネットワークオブジェクト生成
        ghost = PhotonNetwork.Instantiate(ghostPrefab.name, spawnPoint.position,
            spawnPoint.rotation);

        //それぞれにスクリプトを取得
        player.GetComponent<PlayerManager>().ghostManager = ghost.GetComponent<GhostManager>();
        ghost.GetComponent<GhostManager>().playerManager = player.GetComponent<PlayerManager>();
    }

    public void SpawnMotionManager()
    {
        //ネットワークオブジェクト生成
        motionManager = PhotonNetwork.Instantiate(motionManagerPrefab.name, new Vector3(0,0,0),
            Quaternion.identity);
        motionManager.GetComponent<Forgevision.InputCapture.MotionRecorder>()._recordHead = player.transform;
        motionManager.GetComponent<Forgevision.InputCapture.MotionPlayer>()._targetHead = ghost.transform;
    }
}
