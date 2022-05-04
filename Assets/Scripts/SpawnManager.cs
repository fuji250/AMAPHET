using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    //SpawnPoint�i�[�z��쐬
    public Transform[] spawnPoints;

    //��������v���C���[�I�u�W�F�N�g
    public GameObject playerPrefab;
    //���������v���C���[�I�u�W�F�N�g
    GameObject player;

    

    //��������MotionManager�I�u�W�F�N�g
    public GameObject motionManagerPrefab;
    
    //���������v���C���[�I�u�W�F�N�g
    GameObject motionManager;

    //�X�|�[���܂ł̃C���^�[�o��
    public float respawnInterval = 5f;


    bool firstSpawned = false;

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

    //�l�b�g���[�N�I�u�W�F�N�g�Ƃ��ăv���C���[�𐶐�����
    public void SpawnPlayer()
    {
        //�K�؂ȃX�|�[���|�W�V������ϐ��Ɋi�[
        Transform spawnPoint = GetSpawnPoint();
        //�l�b�g���[�N�I�u�W�F�N�g����
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position,
            spawnPoint.rotation);
    }

    public void SpawnMotionManager()
    {
        //�l�b�g���[�N�I�u�W�F�N�g����
        motionManager = PhotonNetwork.Instantiate(motionManagerPrefab.name, new Vector3(0,0,0),
            Quaternion.identity);
    }
}