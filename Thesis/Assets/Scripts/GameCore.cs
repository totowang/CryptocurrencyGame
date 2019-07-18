using System;
using System.Collections;
using System.IO;
using Gamekit2D;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Thesis
{
    public static class GameCore
    {
        public static CoinUI CoinManager;
        public static ILogManager Log;

        private static TransferBase transfer;
        //private static ILedger ledger;

        /// <summary>
        /// 已經購買武器了嗎
        /// </summary>
        private static bool boughtWeapon = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            // 取文字設定檔
            if (File.Exists(DataConfig.ConfigPath))
            {
                var text = File.ReadAllText(DataConfig.ConfigPath);
                JsonConvert.DeserializeObject<DataConfig>(text);
            }

            // 錢錢介面
            if (CoinManager != null) return;
            var coinui = Resources.Load<CoinUI>("CoinCanvas");
            CoinManager = GameObject.Instantiate(coinui);
            GameObject.DontDestroyOnLoad(CoinManager);

            // 掛上區塊鏈$$功能
            if (DataConfig.isToken)
            {
                transfer = new TransferToken(DataConfig.url, DataConfig.contractAddress, DataConfig.adminPrivateKey, DataConfig.adminAddress, DataConfig.userPrivateKey, DataConfig.userAddress);
            }
            else
            {
                transfer = new TransferEther(DataConfig.url, DataConfig.contractAddress, DataConfig.adminPrivateKey, DataConfig.adminAddress, DataConfig.userPrivateKey, DataConfig.userAddress);
            }
            transfer.RegisterListener(CoinManager);

            // 啟動Log
            var filename = DataConfig.userAddress.Substring(2);
            Log = new LogManager("LogData", filename);
            Log.Write("Game Start");

            // 不知道要不要用的帳本
            //ledger = new Ledger();

            // 顯示錢包
            transfer.QueryUserBalance();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 有些場景不要出現錢錢介面
            if (scene.name == "Home" || scene.name == "Boss" || scene.name == "Area4")
            {
                CoinManager.Display(false);
            }
            else if (scene.name.StartsWith("Area") || scene.name == "Ending")
            {
                CoinManager.Display(true);
            }
        }

        public static void GiveReward(int value)
        {
            CoinManager.GiveReward(value);
            transfer.TransferToUser(value);
            Log.Write("Reward,Start," + value);
        }

        public static void Donate(int value)
        {
            if (value > 0)
            {
                transfer.TransferToAdmin(value);
                CoinManager.Spend(value);
            }
            Log.Write("Donate," + value);
        }

        public static void BuyWeaponBox(WeaponBoxType weaponBoxType)
        {
            boughtWeapon = true;
            var value = 999;
            if (weaponBoxType == WeaponBoxType.Normal)
            {
                value = DataConfig.nBoxValue;
            }
            else if (weaponBoxType == WeaponBoxType.Silver)
            {
                value = DataConfig.sBoxValue;
            }
            else if (weaponBoxType == WeaponBoxType.Golden)
            {
                value = DataConfig.gBoxValue;
            }
            
            transfer.TransferToAdmin(value);
            CoinManager.Spend(value);
            Log.Write("BuyWeaponBox,Type," + weaponBoxType.ToString("g"));
            Log.Write("BuyWeaponBox,Value," + value);
        }

        
        static bool inPause = false;
        static string sceneName = String.Empty;

        public static bool BoughtWeapon
        {
            get
            {
                return boughtWeapon;
            }
        }
        
        /// <summary>
        /// 從 Unity 2D Game Kit 複製過來用的，為了在對話等介面出現時遊戲時間可以暫時停止
        /// </summary>
        public static void OpenUIScene(string scenename)
        {
            if (!inPause)
            {
                if (ScreenFader.IsFading)
                    return;

                sceneName = scenename;
                PlayerInput.Instance.ReleaseControl(false);
                PlayerInput.Instance.Pause.GainControl();
                inPause = true;
                Time.timeScale = 0;
                SceneManager.LoadSceneAsync(scenename, LoadSceneMode.Additive);
            }
        }

        /// <summary>
        /// 從 Unity 2D Game Kit 複製過來用的，為了在對話等介面出現時遊戲時間可以暫時停止、然後繼續
        /// </summary>
        public static void Unpause()
        {
            //if the timescale is already > 0, we 
            if (Time.timeScale > 0)
                return;

            CoinManager.StartCoroutine(UnpauseCoroutine());
        }
        
        static IEnumerator UnpauseCoroutine()
        {
            Time.timeScale = 1;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            PlayerInput.Instance.GainControl();
            //we have to wait for a fixed update so the pause button state change, otherwise we can get in case were the update
            //of this script happen BEFORE the input is updated, leading to setting the game in pause once again
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            inPause = false;
        }

        static IEnumerator Transition(string newSceneName)
        {
            PersistentDataManager.SaveAllData();
            yield return CoinManager.StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Loading));
            PersistentDataManager.ClearPersisters();
            yield return SceneManager.LoadSceneAsync(newSceneName);
            PersistentDataManager.LoadAllData();
            yield return CoinManager.StartCoroutine(ScreenFader.FadeSceneIn());
        }
        
        public static void TransitionTo(string newSceneName)
        {
            CoinManager.StartCoroutine(Transition(newSceneName));
        }
    }
}