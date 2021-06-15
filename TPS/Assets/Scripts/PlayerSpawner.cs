using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{

    public static PlayerSpawner instance;

    [SerializeField] ParticleSystem respawnEffect;
    private void Awake()
    {
        instance = this;
    }

    public GameObject playerPrefab;
    public GameObject player;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
        
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();
        PhotonNetwork.Instantiate(respawnEffect.name, spawnPoint.position + new Vector3(0, 1, 0), Quaternion.identity);
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        player.transform.name = PhotonNetwork.NickName;
    }

    public void Die()
    {
        PhotonNetwork.Destroy(player);
        SpawnPlayer();
    }
}
