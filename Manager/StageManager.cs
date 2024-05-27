using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StageManager : MonoBehaviourPun
{
    public GameObject platePrefab;
    public GameObject[] platePositionTable;
    public int[] targetCoin = new int[3]; //목표 금액
    public int curCoin; //현재 금액
    public Text curCoinText;
    public int curTime; //현재 시간
    public Text curTimeText;
    public UI_ReadyStart readyStart;
    public GameObject timeOver;
    public GameObject plateTable;
    public GameObject playerPrefab;

    public static StageManager instance;

    private bool isOnce = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        timeOver.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            curTime = 5;
        }
        if (readyStart.IsReady && !isOnce)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            isOnce = true;
            StartCoroutine(IeTimer());
        }
    }

    IEnumerator IeTimer()
    {
        while (curTime >= 1)
        {
            yield return new WaitForSecondsRealtime(1f);
            curTime -= 1;
            int h = curTime / 60;
            int m = curTime % 60;

            curTimeText.text = "0" + h + ":";
            if (m < 10)
                curTimeText.text += "0" + m;
            else
                curTimeText.text += "" + m;
        }
        timeOver.SetActive(true);
    }

    public void CoinPlus(int i)
    {
        curCoin += i;
        curCoinText.text = "" + curCoin;
    }
}
