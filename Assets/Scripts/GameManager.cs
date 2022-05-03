using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        //ネットワーク接続されていない場合
        if (!PhotonNetwork.IsConnected)
        {
            //タイトルに戻る
            SceneManager.LoadScene(0);
        }
    }
}