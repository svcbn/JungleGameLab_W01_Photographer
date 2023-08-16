using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// [MJ] UI Manager
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        _gameManager = GameManager.instance;
        _gameManager.UIManager = this;
        
        // 변경
        _gameManager.CompleteLoadShop();
        UpdateAllText();
        shopConfirmButton.onClick.AddListener(_gameManager.B_ShopConfirm);
    }
    #endregion

    private GameManager _gameManager;
    
    [Header("View by Game State")]
    public GameObject gameObj;
    public GameObject shopObj;  
    public GameObject clearObj;
    public GameObject gameOverObj; 
    
    
    [Header("Default Game Info Area")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI coinText;
    
    [Header("Item Info")]
    public Transform itemCreatePosTr;
    public GameObject itemPrefab;
    
    [Header("Day or Night Text")]
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI timerText;

    [Header("Shop")]
    public GameObject noMoneyObject;

    public Button shopConfirmButton;

    [Header("Clear")]
    public TextMeshProUGUI stageClearText;


    #region Method by GameState
    public void SetGameViewShop()
    {
        SetViewObject(game:true, shop:true);
        goalText.SetText(string.Empty);
    }
    
    public void SetGameViewDay(int goalCnt)
    {
        SetViewObject(game:true, timerTextObj:true);
        goalText.SetText($"The number of keys you need to collect is {goalCnt}."); 
    }

    public void SetGameViewNight()
    {
        SetViewObject(game:true);
        UpdateGoalText(_gameManager.CollectedGoalCnt, _gameManager.GoalCnt);
    }

    public void SetClearView()
    {
        stageClearText.SetText($"Stage {_gameManager.Stage} Clear!!");
    }

    public void SetGameViewDie()
    {
        SetViewObject(gameOver:true);
    }

    /// <summary>
    /// 설정되지 않은 항목들은 다 비활성화 시킴
    /// </summary>
    private void SetViewObject(bool game = false, bool shop= false, bool gameOver = false, bool timerTextObj = false, bool clear = false)
    {
        gameObj.SetActive(game);
        shopObj.SetActive(shop);
        gameOverObj.SetActive(gameOver);
        clearObj.SetActive(clear);
        timerText.gameObject.SetActive(timerTextObj);   
    }
    #endregion 
    
    #region Other Text Update Method

    private void UpdateAllText()
    {
        UpdatePlayerHp(_gameManager.DefaultPlayerHp);
        UpdateStageText(_gameManager.Stage);
        UpdateCoin(_gameManager.Inventory.Coin);
    }

    public void UpdatePlayerHp(int hpValue)
    {
        string newText = string.Empty;
        for (int i = 0; i < hpValue; i++)
            newText += "♥";
        hpText.SetText(newText);
    }

    public void UpdateStageText(int stageIdx)
    {
        stageText.SetText($"STAGE {stageIdx}");
    }

    public void UpdateCoin(int cnt)
    {
        coinText.SetText($"COIN {cnt}");
    }

    public void UpdateGoalText(int curCnt, int goalCnt)
    {
        goalText.SetText($"GOAL {curCnt} / {goalCnt}"); 
    }

    public void UpdateTimerText(float time)
    {
        timerText.SetText($"{time:F2}");
    }
    #endregion
    
    
    #region Item-Related Method
    public void AddItemOnView(ItemType type)
    {
        // 생성 후 데이타 설정
        var obj = Instantiate(itemPrefab, itemCreatePosTr);
        obj.GetComponent<ShopItem>().Init(type);
    }

    /// <summary>
    /// 해당 오브젝트는 혹시라도 아이템이 비어있는 경우에 호출되지 않음. 호출부에서 빈 값일 때 수행안하도록 처리됨
    /// </summary>
    public void RemoveItemOnView()
    {
        var obj = itemCreatePosTr.GetChild(0).gameObject;
        Destroy(obj);
    }
    #endregion

    #region Shop-Related Method
    public void ShowShop()
    {
         shopObj.SetActive(true);   
    }

    public void CompleteShopping()
    {
        
    }


    #region 구매 실패 관련 변수 및 함수
    private int noMoneyTextShowTime = 0;
    
    /// <summary>
    /// 돈이 부족해서 구매 못했다는 텍스트 출력하는 메서드
    /// </summary>
    public void ShowBuyFailText()
    {
        if (noMoneyTextShowTime == 0)
        {
            noMoneyObject.SetActive(true);
            StartCoroutine(nameof(ShowNoMoneyText));
        }
        
        noMoneyTextShowTime = 1;
    }

    IEnumerator ShowNoMoneyText()
    {
        do
        {
            yield return new WaitForSeconds(1.0f);
            noMoneyTextShowTime--;
        } while (noMoneyTextShowTime > 0);
        
        noMoneyObject.SetActive(false);
    }
    
    #endregion
    #endregion
}
