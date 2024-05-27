using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SinkPlateTable : MonoBehaviourPun
{
    public int cleanPlate;
    public GameObject plate;
    public GameObject PlatePrefab;
    public List<GameObject> plateList = new List<GameObject>();

    public void CreateCleanPlates()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            plate = PhotonNetwork.Instantiate(PlatePrefab.name, transform.position, Quaternion.identity);
            StartCoroutine(IeCreateCleanPlates(plate.GetComponent<PhotonView>().ViewID));
        }

    }

    IEnumerator IeCreateCleanPlates(int id)
    {
        yield return new WaitForSeconds(0.1f);
        photonView.RPC("RpcCreateCleanPlates", RpcTarget.All, id);
    }

    [PunRPC]
    public void RpcCreateCleanPlates(int id)
    {
        for (int i = 0; i < ObjectManager.instance.photonObjectIdList.Count; i++)
        {
            if (!ObjectManager.instance.photonObjectIdList[i])
            {
                ObjectManager.instance.photonObjectIdList.RemoveAt(i);
                i--;
                continue;
            }
            if (ObjectManager.instance.photonObjectIdList[i].GetComponent<PhotonView>().ViewID == id)
            {
                plate = ObjectManager.instance.photonObjectIdList[i];
                GetComponent<M_Table>().getObject = plate;
                plate.transform.parent = transform;
                plate.transform.localPosition = new Vector3(0, 0.2f * (plateList.Count + 1), 0);
                plate.layer = LayerMask.NameToLayer("Plate");
                plateList.Add(plate);
            }
        }
    }

    public void RemovePlate()
    {
        //plate = null;
        photonView.RPC("RpcRemovePlate", RpcTarget.All);
    }

    [PunRPC]
    void RpcRemovePlate()
    {
        cleanPlate--;
        plateList.RemoveAt(cleanPlate);
        if (cleanPlate > 0)
        {
            GetComponent<M_Table>().getObject = plateList[cleanPlate - 1];
        }
        else
        {
            GetComponent<M_Table>().getObject = null;
        }
    }
}
