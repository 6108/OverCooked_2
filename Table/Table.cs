using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Table : MonoBehaviourPun
{
    //±ôºý°Å¸²
    Color startColor;
    Color endColor;

    //¿ÀºêÁ§Æ® ¹Þ±â
    public GameObject getObject;
    Vector3 objectPosition;

    Ray ray;
    RaycastHit hit;

    void Start()
    {
        startColor = GetComponent<Renderer>().material.color;
        endColor = new Color(startColor.r + 0.4f, startColor.g + 0.4f, startColor.b + 0.4f);
        objectPosition = new Vector3(0.5f, 1, 0.5f);
    }

    void Update()
    {
        ray = new Ray(transform.position, transform.up);
        Debug.DrawRay(transform.position, transform.up, Color.blue);
        if (Physics.Raycast(ray, out hit, 1))
        {
            //if (hit.transform.gameObject.tag == "Food")
                //SetObject(hit.transform.gameObject);
        }
    }

    //Å×ÀÌºí ±ôºý°Å¸®°Ô
    public void BlinkTable()
    {
        GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * 2.5f, 1));
    }

    //Å×ÀÌºí ±ôºý°Å¸² ÁßÁö
    public void StopBlink()
    {
        GetComponent<Renderer>().material.color = startColor;
    }

    public void SetObject(int id)
    {
        photonView.RPC("RpcSetTableObject", RpcTarget.All, id);
    }

    GameObject obj;
    [PunRPC]
    public void RpcSetTableObject(int id)
    {
        for (int i = 0; i < ObjectManager.instance.photonObjectIdList.Count; i++)
        {
            if (!ObjectManager.instance.photonObjectIdList[i])
            {
                ObjectManager.instance.photonObjectIdList.RemoveAt(i);
                continue;
            }
            if (ObjectManager.instance.photonObjectIdList[i].GetComponent<PhotonView>().ViewID == id)
            {
                obj = ObjectManager.instance.photonObjectIdList[i];
            }
        }
        if (obj)
        {
            getObject = obj;
            getObject.transform.parent = transform;
            objectPosition.y = 1;
            getObject.transform.localPosition = objectPosition;
        }
    }

    public void DeleteSetObject()
    {
        photonView.RPC("RpcDeleteSetObject", RpcTarget.All);
    }

    [PunRPC]
    public void RpcDeleteSetObject()
    {
        getObject = null;
    }
}
