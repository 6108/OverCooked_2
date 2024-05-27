using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectManager : MonoBehaviourPun
{
    public List<GameObject> photonObjectIdList = new List<GameObject>();

    public static ObjectManager instance;

    private void Awake()
    {
        instance = this;
        StartCoroutine(IeRemoveNull());
    }

    public void SetPhotonObject(GameObject obj)
    {
        photonObjectIdList.Add(obj);
    }

    IEnumerator IeRemoveNull()
    {
        for (int i = 0; i < 1000; i++)
        {
            yield return new WaitForSeconds(0.5f);
            RemoveNull();
        }
    }

    public void RemoveNull()
    {
        for (int i = 0; i < photonObjectIdList.Count; i++)
        {
            if (!photonObjectIdList[i])
            {
                photonObjectIdList.RemoveAt(i);
                i--;
            }
        }
    }
}
