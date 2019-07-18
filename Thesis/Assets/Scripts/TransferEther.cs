using System;
using System.Collections;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using UnityEngine;

namespace Thesis
{
    public class TransferEther : TransferBase
    {
        public TransferEther(string url, string contractAddress, string adminPrivateKey, string adminAddress, string userPrivateKey, string userAddress) : base(url, contractAddress, adminPrivateKey, adminAddress, userPrivateKey, userAddress)
        {
        }

        public override void TransferToUser(int value)
        {
            GameCore.CoinManager.StartCoroutine(Transfer(value, adminAddress, adminPrivateKey, userAddress));
        }

        public override void TransferToAdmin(int value)
        {
            GameCore.CoinManager.StartCoroutine(Transfer(value, userAddress, userPrivateKey, adminAddress));
        }

        public override void QueryAdminBalance(Action<float> callback = null)
        {
            GameCore.CoinManager.StartCoroutine(QueryBalance(adminAddress, callback));
        }

        public override void QueryUserBalance(Action<float> callback = null)
        {
            GameCore.CoinManager.StartCoroutine(QueryBalance(userAddress, callback));
        }

        IEnumerator QueryBalance(string address, Action<float> callback)
        {
            var balanceRequest = new EthGetBalanceUnityRequest(url);
            yield return balanceRequest.SendRequest(address, BlockParameter.CreateLatest());
            var balance = UnitConversion.Convert.FromWei(balanceRequest.Result.Value);
            GameCore.Log.Write("Balance," + balance);
            Debug.Log("Balance of account:" + balance);

            if (listeners.Count > 0)
            {
                foreach (var item in listeners)
                {
                    item.OnCheckBalance((float)balance);
                }
            }
            if (callback != null) callback.Invoke((float)balance);
        }

        IEnumerator Transfer(int value, string fromAddress, string fromPK, string targetAddress)
        {
            GameCore.Log.Write("Transfer transaction,Start," + value);
            GameCore.Log.Write("Transfer transaction,FromAddress," + fromAddress);
            GameCore.Log.Write("Transfer transaction,FromPrivateKey," + fromPK);
            GameCore.Log.Write("Transfer transaction,TargetAddress," + targetAddress);
            var ethTransfer = new EthTransferUnityRequest(url, fromPK, fromAddress);

            yield return ethTransfer.TransferEther(targetAddress, value * 1m, 2);

            if (ethTransfer.Exception != null)
            {
                if (listeners.Count > 0)
                {
                    foreach (var item in listeners)
                    {
                        item.OnTransferFail();
                    }
                }
                Debug.Log(ethTransfer.Exception.Message);
                yield break;
            }

            var transactionHash = ethTransfer.Result;

            GameCore.Log.Write("Transfer transaction hash," + transactionHash);

            //create a poll to get the receipt when mined
            var transactionReceiptPolling = new TransactionReceiptPollingRequest(url);
            //checking every 2 seconds for the receipt
            yield return transactionReceiptPolling.PollForReceipt(transactionHash, 2);

            GameCore.Log.Write("Transaction mined");
            QueryUserBalance();

            if (listeners.Count > 0)
            {
                foreach (var item in listeners)
                {
                    item.OnTransferSussess();
                }
            }
        }
    }
}