using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class PhotonMaster : MonoBehaviourPunCallbacks
{
    public Text statusText;
    private const int MaxPlayerPerRoom = 2;

    public GameObject loadingPanel;//ロードパネル
    public Text loadingText;//ロードテキスト
    //public GameObject buttons;//ボタン
    
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    private void Start()
    {   
        //ロードパネルを表示してテキスト更新
        loadingPanel.SetActive(true);
        loadingText.text = "ネットワークに接続中...";

        //ネットワークに接続しているのか確認
        if (!PhotonNetwork.IsConnected)
        {
            //最初に設定したPhotonServerSettingsファイルの設定に従ってPhotonに接続
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }

    /// 一旦すべてを非表示にする
    void CloseMenuUI()//なぜ作るのか：UI切り替えが非常に楽だから
    {
        loadingPanel.SetActive(false);//ロードパネル非表示
        //buttons.SetActive(false);//ボタン非表示
    }

    /// クライアントがMaster Serverに接続されていて、マッチメイキングやその他のタスクを行う準備が整ったときに呼び出されます
    public override void OnConnectedToMaster()//
    {

        PhotonNetwork.JoinLobby();//マスターサーバー上で、デフォルトロビーに入ります

        loadingText.text = "ロビーへの参加...";//テキスト更新
        Debug.Log("マスターに繋ぎました。");


        PhotonNetwork.AutomaticallySyncScene = true;//マスターと同じシーンに行くように設定
    }

    /// マスターサーバーのロビーに入るときに呼び出されます。
    public override void OnJoinedLobby()//
    {
        CloseMenuUI();


        LobbyMenuDisplay();

        //PhotonNetwork.NickName = Random.Range(0, 1000).ToString();//ユーザーネームをとりあえず適当に決める

        //ConfirmationName();//名前が入力されていればその名前を入力テキストに反映させる
    }

    //ロビーメニュー表示(エラーパネル閉じる時もこれ)
    public void LobbyMenuDisplay()
    {
        CloseMenuUI();
        //buttons.SetActive(true);
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    //これをボタンにつける
    public void FindOponent()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"{cause}の理由で繋げませんでした。");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ルームを作成します。");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerPerRoom });
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.NickName = "1P";
        }
        else
        {
            PhotonNetwork.NickName = "2P";
        }
        Debug.Log("ルームに参加しました");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MaxPlayerPerRoom)
        {
            statusText.text = "対戦相手を待っています。";
        }
        else
        {
            statusText.text = "対戦相手が揃いました。バトルシーンに移動します。";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                statusText.text = "対戦相手が揃いました。バトルシーンに移動します。";
                PhotonNetwork.LoadLevel("Main");
            }
        }
    }
}