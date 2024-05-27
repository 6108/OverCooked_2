using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FireBox : MonoBehaviourPun
{
    public GameObject cookingTool;
    public Vector3 objectPosition;
    public bool isFire;
    public GameObject fireEffectPrefab;
    public GameObject fireEffect;
    public float fireGauge;
    public GameObject fireGaugeCanvas;
    public Image fireGaugeImage;
    GameObject tool;
    AudioSource audioSoure;

    void Start()
    {
        audioSoure = GetComponent<AudioSource>();
        objectPosition = new Vector3(0.5f, 1f, 0.5f);
        if (PhotonNetwork.IsMasterClient && cookingTool)
        {
            tool = PhotonNetwork.Instantiate(cookingTool.name, transform.position, Quaternion.identity);
            StartCoroutine(IeToolSetting(tool.GetComponent<PhotonView>().ViewID));
        }
        fireGaugeImage.GetComponent<Image>().fillAmount = 0;
        fireGaugeCanvas.SetActive(false);
    }

    IEnumerator IeToolSetting(int id)
    {
        yield return new WaitForSeconds(2f);
        photonView.RPC("RpcToolSetting", RpcTarget.All, id);
    }

    [PunRPC]
    void RpcToolSetting(int id)
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
                tool = ObjectManager.instance.photonObjectIdList[i];
            }
        }
        tool.transform.parent = transform;
        tool.transform.localPosition = objectPosition;
        cookingTool = tool;
        if (!cookingTool.GetComponent<FryingPan>().getObject)
            cookingTool.GetComponent<FryingPan>().time = 0;
    }

    void Update()
    {
        if (isFire && fireGauge <= 0)
        {
            fireGauge = 0;
            fireGaugeCanvas.SetActive(false);
            Destroy(fireEffect);
            isFire = false;
        }
        if (isFire && fireGauge > 0)
        {
            fireGaugeImage.GetComponent<Image>().fillAmount = fireGauge / 100;
            return;
        }
        if (!audioSoure.isPlaying && cookingTool && cookingTool.GetComponent<FryingPan>().getObject)
        {
            audioSoure.Play();
        }
        else if (audioSoure.isPlaying && !cookingTool)
        {
            audioSoure.Stop();
        }
    }

    public void Fire()
    {
        isFire = true;
        fireEffect = Instantiate(fireEffectPrefab);
        Vector3 firePos = transform.position;
        firePos.y += 1;
        fireEffect.transform.position = firePos;
        fireGauge = 100;
    }

    public void FireSuppression(float i)
    {
        fireGauge -= i;
    }

    public void SetObject(int id)
    {
        photonView.RPC("RpcSetObject", RpcTarget.All, id);
    }

    GameObject obj;
    [PunRPC]
    public void RpcSetObject(int id)
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
        if (!cookingTool && obj.GetComponent<FryingPan>())
        {
            cookingTool = obj;
            cookingTool.transform.parent = transform;
            cookingTool.transform.localPosition = objectPosition;
        }
    }

    public void CookingToolNull()
    {
        photonView.RPC("RpcCookingToolNull", RpcTarget.All);
    }

    [PunRPC]
    void RpcCookingToolNull()
    {
        cookingTool.GetComponent<FryingPan>().bakeGauge.SetActive(false);
        cookingTool = null;
        
    }
}
