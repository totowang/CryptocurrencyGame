namespace Thesis
{
    public enum CoinType
    {
        /// <summary>
        /// 以太幣
        /// </summary>
        Ether,
        /// <summary>
        /// 自己發行的貨幣
        /// </summary>
        Token,
    }

    public enum WeaponBoxType
    {
        Normal,
        Silver,
        Golden,
    }

    public interface ILedger
    {
    }

    public class Ledger : ILedger
    {
        /// <summary>
        /// 哪種貨幣類型
        /// </summary>
        public CoinType coinType { get; set; }

        // 購買武器相關
        /// <summary>
        /// 選擇的武器盒子類型
        /// </summary>
        public WeaponBoxType weaponBoxType { get; set; }
        /// <summary>
        /// 購買的價格
        /// </summary>
        public int weaponValue { get; set; }

        // 捐錢相關
        /// <summary>
        /// 已經捐過錢了嗎
        /// </summary>
        public bool donated = false;
        /// <summary>
        /// 捐多少錢
        /// </summary>
        public int donateValue { get; set; }

        /// <summary>
        /// 獲得多少錢
        /// </summary>
        public int earnValue { get; set; }

        /// <summary>
        /// 餘額
        /// </summary>
        public int balance { get; set; }

        public void SelectBox(WeaponBoxType type)
        {
            throw new System.NotImplementedException();
        }

        public void BuyBox(WeaponBoxType type)
        {
            throw new System.NotImplementedException();
        }
    }
}