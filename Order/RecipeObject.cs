using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class RecipeObject : ScriptableObject
{
    public string recipeName; //요리 이름
    public GameObject[] cookLevel; //단계별 모델링
    public Sprite recipeSprite; //완성 음식 모델링
    public IngredientObject[] ingredients; //재료
    public Sprite[] ingredientSprites; //재료 이미지
}
