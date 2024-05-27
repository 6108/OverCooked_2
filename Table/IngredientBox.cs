using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IngredientBox : MonoBehaviourPun
{
    public GameObject ingredientPrefab;

    public GameObject CreateIngredient()
    {
        //»ý¼º 
        GameObject ingredient = PhotonNetwork.Instantiate(ingredientPrefab.name, transform.position, Quaternion.identity);
        return ingredient;
    }
}
