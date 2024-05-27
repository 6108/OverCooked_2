using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IngredientDisplay : MonoBehaviourPun
{
    public IngredientObject ingredientObject;
    public GameObject curObject;
    public bool isRaw;
    public bool isCut;
    public bool isBake;
    public bool isBurn;
    int modelLevel = 0;
    int maxModelLevel;
    GameObject player;
    Mesh model;
    Transform modelTransform;
    MeshRenderer modelTexture;
    BoxCollider modelCollider;

    void Start()
    {
        ObjectManager.instance.SetPhotonObject(gameObject);

        //자르거나 구워야 하는 오브젝트가 아니면 바로 접시에 담을 수 있음
        if (ingredientObject.isPossibleBake)
            maxModelLevel = 2;
        else if (ingredientObject.isPossibleCut)
            maxModelLevel = 1;
        MeshChange();
    }

    public void CookLevelUp()
    {
        if (modelLevel < maxModelLevel)
            modelLevel++;
        MeshChange();
    }

    void MeshChange()
    {
        modelTransform = ingredientObject.model[modelLevel].GetComponent<Transform>();
        GetComponent<Transform>().localScale = modelTransform.localScale;
        GetComponent<Transform>().localRotation = modelTransform.localRotation;
        model = ingredientObject.model[modelLevel].GetComponent<MeshFilter>().sharedMesh;
        GetComponent<MeshFilter>().sharedMesh = model;
        if (ingredientObject.model[modelLevel].GetComponent<MeshRenderer>().sharedMaterial.mainTexture)
        {
            modelTexture = ingredientObject.model[modelLevel].GetComponent<MeshRenderer>();
            GetComponent<MeshRenderer>().material.mainTexture = modelTexture.sharedMaterial.mainTexture;
        }
        else
        {
            modelTexture = ingredientObject.model[modelLevel].GetComponent<MeshRenderer>();
            GetComponent<MeshRenderer>().materials = modelTexture.sharedMaterials;
        }
        if (ingredientObject.model[modelLevel].GetComponent<BoxCollider>())
        {
            modelCollider = ingredientObject.model[modelLevel].GetComponent<BoxCollider>();
            GetComponent<BoxCollider>().size = modelCollider.size;
            GetComponent<BoxCollider>().center = modelCollider.center;
        }
    }

    void Update()
    {
        if (!transform.parent)
            GetComponent<PhotonTransformView>().enabled = true;
    }
}
