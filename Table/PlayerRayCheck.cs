using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerRayCheck : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject getObject; //현재 플레이어가 들고 있는 물건
    public GameObject interactiveObject; //현재 플레이어와 닿아있는 물건, 테이블
    public GameObject lastCuttingTable; //현재 어떤 컷팅 테이블과 닿고 있는지
    public GameObject sink;
    public GameObject lastTable;
    public int cleanPlate;
    public int dirtyPlate;

    Ray ray;
    RaycastHit hit;


    void Update()
    {
        if (photonView.IsMine)
        {
            SetPlayerObject();
            ShootRay();
            CheckLastTable();
            CheckPlayerInteractive();
        }
    }

    void ShootRay()
    {
        Vector3 playerPos = transform.position;
        playerPos.y = playerPos.y * 2 / 3 ;
        ray = new Ray(playerPos, transform.forward);
        if (Physics.Raycast(ray, out hit, 1))
        {
           interactiveObject = hit.transform.gameObject;
        }
        else
        {
            interactiveObject = null;
        }
        if (interactiveObject && interactiveObject.tag == "Player")
            return;
        if (interactiveObject && interactiveObject.tag == "Food")
        {
            ray = new Ray(interactiveObject.transform.position, transform.forward);
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, 1))
            {
                interactiveObject = hit2.transform.gameObject;
            }
            else
                interactiveObject = null;
        }
        else if (interactiveObject && interactiveObject.GetComponent<FryingPan>() && getObject)
        {
            ray = new Ray(interactiveObject.transform.position, transform.forward);
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, 1))
            {
                interactiveObject = hit2.transform.gameObject;
            }
            else
                interactiveObject = null;
        }
        else if (interactiveObject && interactiveObject.GetComponent<Plate>())
        {
            ray = new Ray(interactiveObject.transform.position, transform.forward);
            Debug.DrawRay(interactiveObject.transform.position, transform.forward, Color.red, 1);
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, 1))
            {
                interactiveObject = hit2.transform.gameObject;
            }
            else
                interactiveObject = null;
        }
    }

    //플레이어가 들고 있는것, 닿아있는 것 체크
    void SetPlayerObject()
    {
        //플레이가 뭘 들고 있으면 getObject에 넣음
        if (GetComponent<PlayerInteract>().GrabbingObjectInfo)
            getObject = GetComponent<PlayerInteract>().GrabbingObjectInfo;
        else
            getObject = null;
        //플레어어가 어디에 닿아있으면 interactiveObject에 넣음
        if (GetComponent<PlayerInteract>().PointObject)
            interactiveObject = GetComponent<PlayerInteract>().PointObject;
        else
            interactiveObject = null;
    }

    //마지막에 닿은 테이블들 체크
    void CheckLastTable()
    {
        if (interactiveObject && interactiveObject.GetComponent<M_Table>())
        {
            if (interactiveObject.GetComponent<M_Table>())
                interactiveObject.GetComponent<M_Table>().BlinkTable();
            
            //현재 닿은 책상과 마지막에 닿은 책상이 다르다면 깜빡임 멈춤
            if (lastTable && interactiveObject != lastTable)
            {
                interactiveObject.GetComponent<M_Table>().StopBlink();
                lastTable = null;
            }
            //현재 닿은 책상과 마지막에 닿은 책상이 다르면 컷팅테이블 조건 삭제
            if (lastCuttingTable && interactiveObject != lastCuttingTable)
            {
                lastCuttingTable.GetComponent<CuttingTable>().CheckPlayerExist(false);
                lastCuttingTable = null;
            }
            if (sink && interactiveObject != sink )
            {
                sink.GetComponent<Sink>().CheckPlayerExist(false);
                sink = null;
            }
            lastTable = interactiveObject;
        }
        else if (!interactiveObject)
        {
            if (lastTable && lastTable.GetComponent<M_Table>())
            {
                lastTable.GetComponent<M_Table>().StopBlink();
                lastTable = null;
            }
            if (lastCuttingTable && lastCuttingTable.GetComponent<CuttingTable>())
            {
                lastCuttingTable.GetComponent<CuttingTable>().CheckPlayerExist(false);
                lastCuttingTable = null;
            }
        }   
    }

    public void CheckPlayerInteractive()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (interactiveObject && interactiveObject.GetComponent<M_Table>())
            {
                interactiveObject.GetComponent<M_Table>().StopBlink();
                if(interactiveObject.GetComponent<IngredientBox>())
                    InteractiveIngredientBox();
                else if (interactiveObject.GetComponent<CuttingTable>())
                    InteractiveCuttingTable();
                else if (interactiveObject.GetComponent<FireBox>())
                    InteractiveFireTable();
                else if (interactiveObject.name == "ServiceDesk")
                    InteractiveServiceDesk();
                else if (interactiveObject.GetComponent<Sink>())
                    InteractiveSink();
                else if (interactiveObject.GetComponent<SinkPlateTable>())
                    InteractiveSinkPlateTable();
                else if (interactiveObject.GetComponent<M_Table>())
                    InteractiveTable();
            }
            else if (interactiveObject && !getObject)
            {
                HavingSettingObject(interactiveObject);
            }
            else if (!interactiveObject && getObject)
            {
                /*GetComponent<PlayerInteract>().GrabbingObjectInfo.transform.parent = null;
                getObject.GetComponent<Rigidbody>().useGravity = true;
                getObject.GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<PlayerInteract>().GrabbingObjectInfo = null;*/
            }
            else if (interactiveObject)
            {
                if (interactiveObject.GetComponent<Door>())
                    InteractiveDoor();
                else if (interactiveObject.GetComponent<M_Trashcan>())
                    InteractiveTrashcan();
            }
        }
    }

    void InteractiveTrashcan()
    {
        if (getObject)
        {
            if (getObject.GetComponent<Plate>())
            {
                print("재료삭제");
                for(int i = 0; i < 5; i++)
                    getObject.GetComponent<Plate>().DestroyIngredient();
            }
            else if (getObject.GetComponent<IngredientDisplay>())
            {
                PhotonNetwork.Destroy(getObject);
            }
            else if (getObject.GetComponent<FryingPan>())
            {
                PhotonNetwork.Destroy(getObject.GetComponent<FryingPan>().getObject);
            }
        }
    }

    void InteractiveDoor()
    {
        if (getObject)
        {
            print("들어옴");
            if (getObject.GetComponent<Plate>())
            {
                print("들어오오오오오");
                interactiveObject.GetComponent<Door>().CheckOrder(getObject.GetComponent<PhotonView>().ViewID);
            }

        }

    }

    void InteractiveSink()
    {
        if (getObject && getObject.GetComponent<Plate>() && dirtyPlate > 0)
        {
            if (getObject.GetComponent<Plate>().isdirty)
            {
                interactiveObject.GetComponent<Sink>().SetPlate(dirtyPlate);
                photonView.RPC("RpsResetDirtyPlateCount", RpcTarget.All);
                getObject.GetComponent<Plate>().DestroyThisPlate();
            }
        }
        else if (!getObject)
        {
            sink = interactiveObject;
            interactiveObject.GetComponent<Sink>().CheckPlayerExist(true);
        }
    }

    [PunRPC]
    void RpsResetDirtyPlateCount()
    {
        dirtyPlate = 0;
    }

    void InteractiveSinkPlateTable()
    {
        if (!getObject)
        {
            if (interactiveObject.GetComponent<SinkPlateTable>().cleanPlate > 0)
            {
                HavingSettingObject(interactiveObject.GetComponent<M_Table>().getObject);
                interactiveObject.GetComponent<SinkPlateTable>().RemovePlate();
            }
        }
    }

   IEnumerator SetCleanPlate()
    {
        yield return new WaitForSeconds(0.1f);
        HavingSettingObject(interactiveObject.GetComponent<SinkPlateTable>().plate);
        interactiveObject.GetComponent<SinkPlateTable>().RemovePlate();
    }

    void InteractiveServiceDesk()
    {
        if (getObject)
        {
            if (getObject.GetComponent<Plate>())
            {
                OrderSheetManager.instance.CheckOrderSheet(getObject.GetComponent<PhotonView>().ViewID);
            }
        }
    }

    void InteractiveTable()
    {
        if (getObject)
        {
            if (getObject.GetComponent<Plate>() && interactiveObject.GetComponent<M_Table>().getObject)
            {
                getObject.GetComponent<Plate>().GetIngredient(interactiveObject.GetComponent<M_Table>().getObject.GetComponent<PhotonView>().ViewID);
            }
            else if (interactiveObject.GetComponent<M_Table>().getObject && interactiveObject.GetComponent<M_Table>().getObject.GetComponent<Plate>())
            {
                if (getObject.GetComponent<IngredientDisplay>())
                {
                    interactiveObject.GetComponent<M_Table>().getObject.GetComponent<Plate>().GetIngredient(getObject.GetComponent<PhotonView>().ViewID);
                }
                else if (getObject.GetComponent<FryingPan>() && getObject.GetComponent<FryingPan>().getObject)
                {
                    interactiveObject.GetComponent<M_Table>().getObject.GetComponent<Plate>().GetIngredient(getObject.GetComponent<FryingPan>().getObject.GetComponent<PhotonView>().ViewID);
                    getObject.GetComponent<FryingPan>().GetObjectNull();
                }
            }
            else
            {
                if (getObject.GetComponent<Plate>() && getObject.GetComponent<Plate>().isdirty)
                    return;
                interactiveObject.GetComponent<M_Table>().SetObject(getObject.GetComponent<PhotonView>().ViewID);
            }
            /*getObject.GetComponent<Rigidbody>().useGravity = true;
            getObject.GetComponent<Rigidbody>().isKinematic = false;*/
            GetComponent<PlayerInteract>().GrabbingObjectInfo = null;
        }
        else
        {
            if (interactiveObject.GetComponent<M_Table>().getObject)
            {
                HavingSettingObject(interactiveObject.GetComponent<M_Table>().getObject);
                interactiveObject.GetComponent<M_Table>().DeleteSetObject();
                //interactiveObject.GetComponent<M_Table>().getObject = null;
                //더러워진 접시들기
                if (interactiveObject.GetComponent<PlateManager>())
                {
                    dirtyPlate = PlateManager.instance.plateCount;
                    PlateManager.instance.RemovePlateList();
                }
                    
            }
        }
    }

   /* [PunRPC]
    void RpcTableNull()
    {
        interactiveObject.GetComponent<M_Table>().getObject = null;
    }*/

    //재료 상자
    [PunRPC]
    void InteractiveIngredientBox()
    {
        GameObject ingredient = interactiveObject.GetComponent<IngredientBox>().CreateIngredient();
        HavingSettingObject(ingredient);
    }

    //자르는 테이블
    void InteractiveCuttingTable()
    {
        CuttingTable cuttingTable = interactiveObject.GetComponent<CuttingTable>();
        if (getObject)
        {
            //들고 있는 오브젝트 테이블에 내려 놓기
            //자를 수 있는 음식을 들고 있으면 내려 놓음
            if (getObject.GetComponent<IngredientDisplay>() && getObject.GetComponent<IngredientDisplay>().ingredientObject.isPossibleCut)
            {
                cuttingTable.SetObject(getObject.GetComponent<PhotonView>().ViewID);
            }
            //접시를 들고있으면서 컷팅테이블에 올라간 재료가 잘린 상태라면
            else if (getObject.tag == "Plate" && cuttingTable.cutTableObject.GetComponent<IngredientDisplay>().isCut)
                getObject.GetComponent<Plate>().GetIngredient(cuttingTable.cutTableObject.GetComponent<PhotonView>().ViewID);    
        }
        else
        {
            lastCuttingTable = interactiveObject;
            //만약 다 잘린 재료가 테이블에 있다면 플레이어가 가져감
            if (cuttingTable.cutTableObject && cuttingTable.cutTableObject.GetComponent<IngredientDisplay>().isCut)
            {
                HavingSettingObject(cuttingTable.cutTableObject);
                cuttingTable.cutTableObject = null;
            }
            //만약 다 잘리지 않은 재료가 테이블에 있다면 썰기 시작
            else
            {
                cuttingTable.GetComponent<CuttingTable>().CheckPlayerExist(true);
            }
        }
    }

    //화덕
    void InteractiveFireTable()
    {
        FireBox fireBox = interactiveObject.GetComponent<FireBox>();
        if (getObject)
        {
            //도구가 있으면서 도구 위에 아무것도 없고 가진게 재료일 때
            if (fireBox.cookingTool && !fireBox.cookingTool.GetComponent<FryingPan>().getObject && getObject.GetComponent<IngredientDisplay>())
            {
                //화덕 위 요리 도구 위에 재료 셋팅
                fireBox.cookingTool.GetComponent<FryingPan>().SetObject(getObject.GetComponent<PhotonView>().ViewID);
            }
            else if (!fireBox.cookingTool && getObject.GetComponent<FryingPan>())
            {
                fireBox.SetObject(getObject.GetComponent<PhotonView>().ViewID);
            }
        }
        else
        {
            if (fireBox.cookingTool)
            {
                photonView.RPC("RpcSetCookingTool", RpcTarget.All, fireBox.cookingTool.GetComponent<PhotonView>().ViewID);
                //HavingSettingObject(fireBox.cookingTool);
                fireBox.CookingToolNull();
            }
                
        }
    }

    GameObject cookingTool;
    [PunRPC]
    void RpcSetCookingTool(int id)
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
                cookingTool = ObjectManager.instance.photonObjectIdList[i];
            }
        }
        HavingSettingObject(cookingTool);
    }

    [PunRPC]
    public void HavingSettingObject(GameObject obj)
    {
        for (int i = 0; i < ObjectManager.instance.photonObjectIdList.Count; i++)
        {
            if (!ObjectManager.instance.photonObjectIdList[i])
            {
                ObjectManager.instance.photonObjectIdList.RemoveAt(i);
                i--;
            }
        }
        int id = obj.GetComponent<PhotonView>().ViewID;
        GetComponent<PlayerInteract>().CallGrabOnTable_RPC(id);
        obj.GetComponent<PhotonTransformView>().enabled = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
