using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CuttingTable : MonoBehaviourPun, IPunObservable
{
    public GameObject cutTableObject; //테이블 위 오브젝트
    public Vector3 objectPosition; //오브젝트 배치
    float cutTime = 2; //자르는 시간
    float time = 0; //현재 시간
    public bool isPlayerExist; //플레이어가 존재 하는지
    public GameObject cutGauge; //얼마나 잘렸는지
    public Image cutGaugeImage; //얼마나 잘렸는지 이미지로 표현
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        objectPosition = new Vector3(0.5f, 1, 0.5f);
        cutGaugeImage.GetComponent<Image>().fillAmount = time / cutTime;
        cutGauge.SetActive(false);
    }

    void Update()
    {
        if (!isPlayerExist && audioSource.isPlaying)
            audioSource.Stop();
        //도마위에 올라온 오브젝트가 음식이면서 자르지 않은 상태라면 시간이 흐름
        if (cutTableObject && cutTableObject.GetComponent<IngredientDisplay>())
        {
            if (cutTableObject.GetComponent<IngredientDisplay>().ingredientObject.isPossibleCut && !cutTableObject.GetComponent<IngredientDisplay>().isCut)
            {
                if (isPlayerExist)
                {
                    if (!audioSource.isPlaying)
                        audioSource.Play();
                    cutGauge.SetActive(true);
                    time += Time.deltaTime;
                    cutGaugeImage.GetComponent<Image>().fillAmount = time / cutTime;
                }  
                if (cutTime < time)
                {
                    //상태를 자른걸로 변환
                    ChangeStateCut();
                }
            }
        }
    }

    [PunRPC]
    void ChangeStateCut()
    {
        audioSource.Stop();
        cutTableObject.GetComponent<IngredientDisplay>().isCut = true;
        cutTableObject.GetComponent<IngredientDisplay>().CookLevelUp(); 
        time = 0;
        cutGauge.SetActive(false);
    }

    public void SetObject(int id)
    {
        photonView.RPC("RpcSetObject", RpcTarget.All, id);
    }

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
                cutTableObject = ObjectManager.instance.photonObjectIdList[i];
                cutTableObject.transform.parent = transform;
                objectPosition.y = 0.6f;
                cutTableObject.transform.localPosition = objectPosition;
            }
        }
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

    [PunRPC]
    void GaugeImageActive(bool TF)
    {
        cutGauge.SetActive(TF);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(time);
        }
        else
        {
            time = (float)stream.ReceiveNext();
        }
    }
}
