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

    public GameObject loadingPanel;//���[�h�p�l��
    public Text loadingText;//���[�h�e�L�X�g
    //public GameObject buttons;//�{�^��
    
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    private void Start()
    {   
        //���[�h�p�l����\�����ăe�L�X�g�X�V
        loadingPanel.SetActive(true);
        loadingText.text = "�l�b�g���[�N�ɐڑ���...";

        //�l�b�g���[�N�ɐڑ����Ă���̂��m�F
        if (!PhotonNetwork.IsConnected)
        {
            //�ŏ��ɐݒ肵��PhotonServerSettings�t�@�C���̐ݒ�ɏ]����Photon�ɐڑ�
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }

    /// ��U���ׂĂ��\���ɂ���
    void CloseMenuUI()//�Ȃ����̂��FUI�؂�ւ������Ɋy������
    {
        loadingPanel.SetActive(false);//���[�h�p�l����\��
        //buttons.SetActive(false);//�{�^����\��
    }

    /// �N���C�A���g��Master Server�ɐڑ�����Ă��āA�}�b�`���C�L���O�₻�̑��̃^�X�N���s���������������Ƃ��ɌĂяo����܂�
    public override void OnConnectedToMaster()//
    {

        PhotonNetwork.JoinLobby();//�}�X�^�[�T�[�o�[��ŁA�f�t�H���g���r�[�ɓ���܂�

        loadingText.text = "���r�[�ւ̎Q��...";//�e�L�X�g�X�V
        Debug.Log("�}�X�^�[�Ɍq���܂����B");


        PhotonNetwork.AutomaticallySyncScene = true;//�}�X�^�[�Ɠ����V�[���ɍs���悤�ɐݒ�
    }

    /// �}�X�^�[�T�[�o�[�̃��r�[�ɓ���Ƃ��ɌĂяo����܂��B
    public override void OnJoinedLobby()//
    {
        CloseMenuUI();


        LobbyMenuDisplay();

        //PhotonNetwork.NickName = Random.Range(0, 1000).ToString();//���[�U�[�l�[�����Ƃ肠�����K���Ɍ��߂�

        //ConfirmationName();//���O�����͂���Ă���΂��̖��O����̓e�L�X�g�ɔ��f������
    }

    //���r�[���j���[�\��(�G���[�p�l�����鎞������)
    public void LobbyMenuDisplay()
    {
        CloseMenuUI();
        //buttons.SetActive(true);
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    //������{�^���ɂ���
    public void FindOponent()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"{cause}�̗��R�Ōq���܂���ł����B");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���[�����쐬���܂��B");
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
        Debug.Log("���[���ɎQ�����܂���");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MaxPlayerPerRoom)
        {
            statusText.text = "�ΐ푊���҂��Ă��܂��B";
        }
        else
        {
            statusText.text = "�ΐ푊�肪�����܂����B�o�g���V�[���Ɉړ����܂��B";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                statusText.text = "�ΐ푊�肪�����܂����B�o�g���V�[���Ɉړ����܂��B";
                PhotonNetwork.LoadLevel("Main");
            }
        }
    }
}