using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class M_ServerManager : MonoBehaviourPunCallbacks
{
    public GameObject serverPanel;
    public GameObject startButton;
    public GameObject joinButton;
    public GameObject playerPrefab;

    void Start()
    {
        startButton.SetActive(false);
        joinButton.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        startButton.SetActive(true);
        joinButton.SetActive(true);
    }

    public void OnClickStartButton()
    {
        PhotonNetwork.CreateRoom("1");
    }

    public void OnClickJoinButton()
    {
        PhotonNetwork.JoinRoom("1");
    }

    public override void OnJoinedRoom()
    {
        serverPanel.SetActive(false);
            PhotonNetwork.LoadLevel("Stage1");
    }
}
