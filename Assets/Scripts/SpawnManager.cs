using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class SpawnManager : MonoBehaviour
{
    //SpawnPoint�i�[�z��쐬
    public Transform[] spawnPoints;

    //��������v���C���[�I�u�W�F�N�g
    public GameObject playerPrefab;
    //���������v���C���[�I�u�W�F�N�g
    GameObject player;
    //��������S�[�X�g�I�u�W�F�N�g
    public GameObject ghostPrefab;
    //���������S�[�X�g�I�u�W�F�N�g
    GameObject ghost;

    

    //��������MotionManager�I�u�W�F�N�g
    public GameObject motionManagerPrefab;
    
    //���������v���C���[�I�u�W�F�N�g
    GameObject motionManager;

    //�X�|�[���܂ł̃C���^�[�o��
    public float respawnInterval = 5f;

    // Start is called before the first frame update
    void Start()
    {
        //�X�|�[���I�u�W�F�N�g��S�Ĕ�\��
        foreach(Transform position in spawnPoints)
        {
            position.gameObject.SetActive(false);
        }

        //�����֐��Ăяo��
        if (PhotonNetwork.IsConnected)
        {
            //�l�b�g���[�N�I�u�W�F�N�g�Ƃ��ăv���C���[�𐶐�����
            SpawnPlayer();

            SpawnMotionManager();
        }
    }

    //�����_���ɃX�|�[���|�C���g�̈��I������֐�
    public Tuple<Transform, int> GetSpawnPoint()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var result = Tuple.Create<Transform, int>(spawnPoints[0],0);
            return result;
        }
        else
        {
            //return spawnPoints[1];
            var result = Tuple.Create<Transform, int>(spawnPoints[1],1);
            return result;
        }
    }

    //�l�b�g���[�N�I�u�W�F�N�g�Ƃ��ăv���C���[�𐶐�����
    public void SpawnPlayer()
    {
        //�K�؂ȃX�|�[���|�W�V������ϐ��Ɋi�[
        Tuple<Transform, int> tuple = GetSpawnPoint();
        Transform spawnPoint = tuple.Item1;
        int spawnPointNum = tuple.Item2;

        //�l�b�g���[�N�I�u�W�F�N�g����
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position,
            spawnPoint.rotation);
        //�l�b�g���[�N�I�u�W�F�N�g����
        ghost = PhotonNetwork.Instantiate(ghostPrefab.name, spawnPoint.position,
            spawnPoint.rotation);

        //���ꂼ��ɃX�N���v�g���擾
        player.GetComponent<PlayerManager>().ghostManager = ghost.GetComponent<GhostManager>();
        ghost.GetComponent<GhostManager>().playerManager = player.GetComponent<PlayerManager>();
        player.GetComponent<PlayerManager>().spawnPointNumber = spawnPointNum;
    }

    public void SpawnMotionManager()
    {
        //�l�b�g���[�N�I�u�W�F�N�g����
        motionManager = PhotonNetwork.Instantiate(motionManagerPrefab.name, new Vector3(0,0,0),
            Quaternion.identity);
        motionManager.GetComponent<Forgevision.InputCapture.MotionRecorder>()._recordHead = player.transform;
        motionManager.GetComponent<Forgevision.InputCapture.MotionPlayer>()._targetHead = ghost.transform;
    }
}
