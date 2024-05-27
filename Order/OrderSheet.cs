using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

//주문서
public class OrderSheet : MonoBehaviourPun
{
    //재료와 요리법이 담긴 이미지를 보여줌
    //시간이 지나면 게이지가 줄어듬, 색깔 초록에서 빨강으로 바뀜(그라데이션)
    //시간이 조금 남으면 덜덜 떨림

    public string recipeName; //지금 무슨 주문인지 가시적으로 보여주기위함. 테스트끝나면 지워
    public RecipeObject recipe; //어떤 레시피인지
    public Sprite orderSheetSprite; //주문서 이미지
    public GameObject orderBG; //주문서 배경 이미지
    public GameObject recipeBG; //레시피 재료 배경이미지
    public GameObject recipeContentGroup; //재료 이미지 그룹
    public Image timeGauge;
    public float gauge = 100;
    int ingredientTime = 50;
    public Image wrongImage;

    void Start()
    {
        CreateRecipeContents();
        gauge = recipe.ingredients.Length * ingredientTime;
        StartCoroutine(IeTimer());
    }

    IEnumerator IeTimer()
    {
        for (int i = 0; i < recipe.ingredients.Length * ingredientTime; i++)
        {
            yield return new WaitForSecondsRealtime(1f);
            gauge--;
            timeGauge.GetComponent<Image>().fillAmount = gauge / (recipe.ingredients.Length * ingredientTime);
            Color c = Color.Lerp(Color.red, Color.green, gauge / (recipe.ingredients.Length * ingredientTime));
            timeGauge.color = c;
       }
        DestroyOrder();
        Destroy(gameObject);
    }

    //레시피 정보 얻어와서 주문서 표시
    void CreateRecipeContents()
    {
        //재료 수 만큼 배경 이미지 크기 넓힘
        orderBG.GetComponent<RectTransform>().sizeDelta = new Vector2(100 * recipe.ingredients.Length, 150);
        //재료 수만큼 재료 배경, 재료 이미지 추가
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            GameObject rBG = Instantiate(recipeBG);
            rBG.transform.SetParent(recipeContentGroup.transform);
            rBG.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            rBG.transform.GetChild(0).GetComponent<Image>().sprite = recipe.ingredients[i].recipeIcon;
        }
    }

    public void DestroyOrder()
    {
        OrderSheetManager.instance.DeleteOrderSheet(gameObject);
    }
}
