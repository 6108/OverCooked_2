# OverCooked_2


### 게임

제목: 오버쿡드2

빌드: Window

조작: 마우스, 키보드 

내용: 주문서에 맞게 요리를 해야하는 게임.

2인 멀티로 진행됩니다.

---

### 개발

인원: 프로그래머 2인

기간: 1개월

사용 프로그램: Unity3D(C#), Photon

---

## 코드 요약

### Ingredient

`IngredientDisplay` 음식의 조리 단계에 따라 모델링과 텍스쳐 변경

`IngredientObject` 재료의 정보를 담는 ScriptableObject

### Manager

`ObjectManager` Photon객체 아이디 관리

`OrderSheetManager` 주문서 생성, 관리

`PlateManager` 접시 상태 관리(더러움, 깨끗함)

`StageManager` 스테이지 타이머

### Order

`OrderSheet` 상단에 뜨는 주문서 표시, 타이머 UI

`RecipeObject` 레시피의 정보를 담는 ScriptableObject

### Table

`CuttingTable` 자르기

`FireBox` 굽기

`FryingPan` 요리에 필요한 도구, 이동 가능

`IngredientBox` 상호작용 시 재료 생성

`Plate` 재료를 합성해서 요리로 만듬

`PlayerRayCheck` 플레이어 Ray 체크, 상호작용 관리

`Sink` 설거지

`SinkPlateTable` 싱크대 옆 접시 놓는 곳

`Table` 플레이어와 닿아있으면 반짝거림
