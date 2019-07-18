using System;
using System.Collections.Generic;

namespace Thesis
{
    public interface ITransferListener
    {
        void OnCheckBalance(float value);
        void OnTransferSussess();
        void OnTransferFail();
    }

    public abstract class TransferBase
    {
        public event Action onTransfer;

        protected string url = "";//https://ropsten.infura.io";
        protected string contractAddress = String.Empty;
        protected string adminPrivateKey = String.Empty;
        protected string adminAddress = String.Empty;
        protected string userPrivateKey = String.Empty;
        protected string userAddress = String.Empty;

        public abstract void TransferToUser(int value);
        public abstract void TransferToAdmin(int spendValue);
        public abstract void QueryUserBalance(Action<float> showUserCoin = null);
        public abstract void QueryAdminBalance(Action<float> showAdminCoin = null);

        protected List<ITransferListener> listeners = new List<ITransferListener>();

        public void RegisterListener(ITransferListener listener)
        {
            lock (listeners)
            {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(ITransferListener listener)
        {
            lock (listeners)
            {
                listeners.Remove(listener);
            }
        }

        public  TransferBase(string url, string contractAddress, string adminPrivateKey, string adminAddress, string userPrivateKey, string userAddress)
        {
            this.url = url;
            this.contractAddress = contractAddress;
            this.adminPrivateKey = adminPrivateKey;
            this.adminAddress = adminAddress;
            this.userPrivateKey = userPrivateKey;
            this.userAddress = userAddress;
        }
    }
}