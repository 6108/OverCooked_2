using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Sink : MonoBehaviourPun
{
    public int cleanPlate = 0;
    public int dirtyPlate = 0;
    public GameObject washGauge;
    public Image washGaugeImage;
    public bool isPlayerExist;
    float time;
    float washTime = 3;
    public GameObject sinkPlateTable;
    public GameObject sinkModel;
    public GameObject dirtySinkModel;

    void Start()
    {
        washGaugeImage.GetComponent<Image>().fillAmount = 0;
    }

    void Update()
    {
        WashPlate();
        if (dirtyPlate > 0)
        {
            sinkModel.SetActive(false);
            dirtySinkModel.SetActive(true);
        }
        else
        {
            sinkModel.SetActive(true);
            dirtySinkModel.SetActive(false);
        }
    }

    
    public void SetPlate(int i)
    {
        photonView.RPC("RpcPlate", RpcTarget.All, i);
    }

    [PunRPC]
    void RpcPlate(int i)
    {
        dirtyPlate += i;
    }

    public void CheckPlayerExist(bool exist)
    {
        photonView.RPC("RpcCheckPlayerExist", RpcTarget.All, exist);
    }

    [PunRPC]
    public void RpcCheckPlayerExist(bool exist)
    {
        isPlayerExist = exist;
    }

    public void WashPlate()
    {
        if (isPlayerExist && dirtyPlate > 0)
        {
            washGauge.SetActive(true);
            time += Time.deltaTime;
            washGaugeImage.GetComponent<Image>().fillAmount = time / washTime;
            if (time > washTime)
            {
                dirtyPlate--;
                sinkPlateTable.GetComponent<SinkPlateTable>().cleanPlate++;
                sinkPlateTable.GetComponent<SinkPlateTable>().CreateCleanPlates();
                time = 0;
            }
        }
        else
        {
            washGauge.SetActive(false);
        }
    }
}
