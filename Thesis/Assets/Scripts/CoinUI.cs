using System;
using System.Collections;
using Gamekit2D;
using Thesis;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class BalanceEvent : UnityEvent<float>
{
    public BalanceEvent()
    {
    }
}

public class CoinUI : MonoBehaviour, ITransferListener
{
    public static CoinUI Instance
    {
        get
        {
            if (s_Instance != null)
                return s_Instance;

            s_Instance = FindObjectOfType<CoinUI>();

            return s_Instance;
        }
    }
    
    protected static CoinUI s_Instance;
    protected CoinUI m_OldInstanceToDestroy = null;

    public int randomMin = 1;
    public int randomMax = 10;
    
    public GameObject display;

    [SerializeField]
    Text userCoinText;

    [SerializeField]
    Text userNtdText;
    [SerializeField]
    Text messageText;
    [SerializeField]
    Slider donateSlider;

    [SerializeField]
    Sprite Normal;
    [SerializeField]
    Sprite Highlight;
    [SerializeField]
    Image Background;
    [SerializeField]
    GameObject MessagePanel;
    [SerializeField]
    GameObject PendingMask;
    [SerializeField]
    GameObject DonateMask;
    [SerializeField]
    GameObject DonateUI;
    [SerializeField]
    GameObject RewardUI;
    [SerializeField]
    Text RewardValueText;
    [SerializeField]
    Text RewardValueNTDText;
    [SerializeField]
    GameObject[] ntdObjects;
    
    public Button DonateSubmitButton;
    public BalanceEvent OnCheckBalance;
    public UnityEvent OnTransferSussess;
    public UnityEvent OnTransferFail;
    
    private float rate = 1f;
    private float spendValue;

    public float GetspendValue()
    {
        return spendValue;
    }

    public void SetspendValue(float value)
    {
        spendValue = value;
        spendValueNTD.text = (value * rate).ToString("g5");
    }

    public Text spendValueNTD;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            m_OldInstanceToDestroy = Instance;
        }

        s_Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //if delete & trasnfer time only in Start so we avoid the small gap that doing everything at the same time in Awake would create 
        if (m_OldInstanceToDestroy != null)
        {
            Destroy(m_OldInstanceToDestroy.gameObject);
        }
        
        rate = DataConfig.exRate;
        if (!DataConfig.hasNTD && ntdObjects.Length > 1)
        {
            foreach (var item in ntdObjects)
            {
                item.SetActive(false);
            }
        }
    }

    public void GiveReward(int value)
    {
        PendingMask.SetActive(true);
        StartCoroutine(ShowMessage("Reward : " + value.ToString()));
        OpenRewardUI(value);
    }

    public void Donate()
    {
        GameCore.Donate((int)spendValue);
    }

    public void Spend(int value)
    {
        SetspendValue(value);
        Spend();
    }

    void Spend()
    {
        PendingMask.SetActive(true);
        StartCoroutine(ShowMessage("Spend : " + GetspendValue().ToString()));
        SetspendValue(0);
    }

    IEnumerator ShowMessage(string message)
    {
        var speed = 0.3f;
        var cg = MessagePanel.GetComponent<CanvasGroup>();
        cg.alpha = 1f;
        messageText.text = message;

        yield return new WaitForSeconds(3f);

        var wait = new WaitForEndOfFrame();
        while (cg.alpha > 0.01f)
        {
            cg.alpha -= Time.deltaTime * speed;
            yield return wait;
        }
        cg.alpha = 0f;
    }

    public void OpenDonateUI(bool active)
    {
        DonateUI.SetActive(active);
        if (PendingMask.activeSelf)
        {
            DonateMask.SetActive(active);
        }
    }

    void OpenRewardUI(int value)
    {
        PlayerInput.Instance.ReleaseControl(true);
        PlayerInput.Instance.GainControl();
        Time.timeScale = 0;
        RewardValueText.text = value.ToString("g5");
        RewardValueNTDText.text = (value * rate).ToString("g5");
        RewardUI.SetActive(true);

        var button = RewardUI.GetComponentInChildren<Button>();
        button.onClick.AddListener(() => {
            StartCoroutine(BackGame());
        });
    }

    IEnumerator BackGame()
    {
        Time.timeScale = 1;
        PlayerInput.Instance.Pause.GainControl();
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
    }

    void ShowUserCoin(float value)
    {
        userCoinText.text = value.ToString("g5");
        userNtdText.text = (value * rate).ToString("g5");
        donateSlider.maxValue = Mathf.Floor(value);
    }

    public void Display(bool on)
    {
        display.SetActive(on);
    }

    public void TransferDone()
    {
        PendingMask.SetActive(false);
    }

    void ITransferListener.OnCheckBalance(float value)
    {
        ShowUserCoin(value);
        OnCheckBalance.Invoke(value);
    }

    void ITransferListener.OnTransferSussess()
    {
        OnTransferSussess.Invoke();
    }

    void ITransferListener.OnTransferFail()
    {
        OnTransferFail.Invoke();
    }
}
