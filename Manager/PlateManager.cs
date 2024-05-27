using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlateManager : MonoBehaviourPun
{
    public List<GameObject> plateList = new List<GameObject>();
    public GameObject platePrefab;
    public int plateCount = 0;
    public static PlateManager instance;

    private void Awake()
    {
        instance = this;
    }

    GameObject plate;
    public void AddDirtyPlate()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        plate = PhotonNetwork.Instantiate(platePrefab.name, new Vector3(0, 20, 0), Quaternion.identity);
        StartCoroutine(DirtyPlateSetting(plate));
    }

    IEnumerator DirtyPlateSetting(GameObject plate)
    {
        yield return new WaitForSeconds(0.1f);
        photonView.RPC("RpcDirtyPlateSetting", RpcTarget.All, plate.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    void RpcDirtyPlateSetting(int id)
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
            }
        }
        plate.GetComponent<Plate>().isdirty = true;
        //plate.SetActive(true);
        //접시 반환 테이블에 접시가 하나도 없으면 하나 추가
        if (plateList.Count == 0)
        {
            GetComponent<M_Table>().SetObject(plate.GetComponent<PhotonView>().ViewID);
            plate.transform.parent = transform;
            plate.transform.localPosition = new Vector3(0.6f, 1.4f, 0.4f);
            plate.layer = LayerMask.NameToLayer("Plate");
            plateList.Add(plate);
        }
        else
        {
            plate.GetComponent<BoxCollider>().enabled = false;
            //이전 접시위에 올리기
            plateList[plateList.Count - 1].GetComponent<Plate>().AddPlate(plate);
            plateList.Add(plate);

        }
        plateCount++;
    }

    public void RemovePlateList()
    {
        photonView.RPC("RpcRemovePlateList", RpcTarget.All);
    }    

    [PunRPC]
    public void RpcRemovePlateList()
    {
        plateList.Clear();
        plateCount = 0;
    }
}
