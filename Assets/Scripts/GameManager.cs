using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        //�l�b�g���[�N�ڑ�����Ă��Ȃ��ꍇ
        if (!PhotonNetwork.IsConnected)
        {
            //�^�C�g���ɖ߂�
            SceneManager.LoadScene(0);
        }
    }
}