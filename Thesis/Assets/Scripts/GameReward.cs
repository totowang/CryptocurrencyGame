using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Thesis
{
    public class GameReward : MonoBehaviour
    {
        public int randomMin = 1;
        public int randomMax = 10;

        public TransferBase transfer;
        public Text userCoinText;
        public Text adminCoinText;
        public Text messageText;
        public Slider donateSlider;
        public float spendValue { get; set; }

        [SerializeField]
        public UnityEvent<int> onGetReward;

        private void Start()
        {
            transfer.onTransfer += ShowUserCoin;
            transfer.onTransfer += ShowAdminCoin;
            ShowUserCoin();
            ShowAdminCoin();
        }

        public void GiveReward()
        {
            int value = UnityEngine.Random.Range(randomMin, randomMax);
            messageText.text = "Reward : " + value.ToString();
            transfer.TransferToUser(value);
        }

        public void Spend()
        {
            messageText.text = "Spend : " + spendValue.ToString();
            transfer.TransferToAdmin((int)spendValue);
        }

        public void ShowUserCoin()
        {
            transfer.QueryUserBalance(ShowUserCoin);
        }

        public void ShowAdminCoin()
        {
            transfer.QueryAdminBalance(ShowAdminCoin);
        }

        void ShowUserCoin(float value)
        {
            userCoinText.text = value.ToString("f4"); //"User Coin : " + value.ToString();
            donateSlider.maxValue = Mathf.Floor(value);
        }

        void ShowAdminCoin(float value)
        {
            adminCoinText.text = value.ToString("f4"); //"Admin Coin : " + value.ToString();
        }
    }
}
