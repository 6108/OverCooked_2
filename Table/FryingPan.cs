using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FryingPan : MonoBehaviourPun
{
    public GameObject getObject;
    Vector3 objectPosition = new Vector3(0, -0.2f, 0);

    //굽기
    float bakeTime = 10; //10
    public float time = 0;
    public GameObject bakeGauge;
    public Image bakeGaugeImage;

    //불
    float fireTime = 18;
    public GameObject burnWarning;

    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ObjectManager.instance.SetPhotonObject(gameObject);
        bakeGauge.SetActive(false);
    }

    void Update()
    {
        //화덕에서 불나면 아무것도 실행 X
        if (transform.parent && transform.parent.GetComponent<FireBox>() && 
            transform.parent.GetComponent<FireBox>().isFire)
            return;  
        //화덕 위에 있고 음식이 있다면
        if (transform.parent && transform.parent.GetComponent<FireBox>() && getObject)
        {
            Bake();
        }
        else
            bakeGauge.SetActive(false);
        if (!getObject)
        {
            audioSource.Stop();
            burnWarning.SetActive(false);
            time = 0;
        }
        if (transform.parent && !transform.parent.GetComponent<FireBox>())
        {
            audioSource.Stop();
            burnWarning.SetActive(false);
        }
           
    }

    void Bake()
    {
        //탄 음식이면 더 굽지 않
        if (getObject.GetComponent<IngredientDisplay>().isBurn)
            return;
        time += Time.deltaTime;
        if (!bakeGauge.activeSelf && !getObject.GetComponent<IngredientDisplay>().isBake && getObject)
            bakeGauge.SetActive(true);
        bakeGaugeImage.GetComponent<Image>().fillAmount = time / bakeTime;
        if (time > fireTime)
        {
                        transform.parent.GetComponent<FireBox>().Fire();
            getObject.GetComponent<IngredientDisplay>().isBurn = true;
            bakeGauge.SetActive(false);
            burnWarning.SetActive(false);
            time = 0;
        }
        else if (time > bakeTime && !getObject.GetComponent<IngredientDisplay>().isBake)
        {
            getObject.GetComponent<IngredientDisplay>().isBake = true;
            StartCoroutine(BurnWarning());
            ChangeStateBake();
        }
    }

    [PunRPC]
    void ChangeStateBake()
    {
        getObject.GetComponent<IngredientDisplay>().isBake = true;
        getObject.GetComponent<IngredientDisplay>().CookLevelUp();
        bakeGauge.SetActive(false);
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
        //박스 위에 오브젝트가 없으면 받은 오브젝트 셋팅
        if (!getObject && obj.GetComponent<IngredientDisplay>() && 
            obj.GetComponent<IngredientDisplay>().isCut && 
            obj.GetComponent<IngredientDisplay>().ingredientObject.isPossibleBake)
            {
                burnWarning.SetActive(false);
                time = 0;
                getObject = obj;
                getObject.transform.parent = transform;
                getObject.transform.localPosition = objectPosition;
            }
    }

    IEnumerator BurnWarning()
    {
        bakeGauge.SetActive(false);
        for (float i = 1; i < 10; i++) 
        {
            for (int j = 0; j < 2; j++)
            {
                if (!getObject)
                    break;
                burnWarning.SetActive(false);
                yield return new WaitForSeconds(Mathf.Lerp(0.4f, 0f, i / 10f));
                burnWarning.SetActive(true);
                yield return new WaitForSeconds(Mathf.Lerp(0.4f, 0f, i / 10f));
            }
        }
    }

    public void GetObjectNull()
    {
        photonView.RPC("RpcGetObjectNull", RpcTarget.All);
    }

    [PunRPC]
    void RpcGetObjectNull()
    {
        getObject = null;
    }
}
