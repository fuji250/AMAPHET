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
        }
    }

    //ランダムにスポーンポイントの一つを選択する関数
    public Transform GetSpawnPoint()
    {
        if (firstSpawned == false)
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
        //ランダムなスポーンポジションを変数に格納
        Transform spawnPoint = GetSpawnPoint();

        //ネットワークオブジェクト生成
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position,
            spawnPoint.rotation);
    }
}
