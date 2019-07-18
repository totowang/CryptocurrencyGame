using System;
using System.Collections;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.JsonRpc.UnityClient;
using UnityEngine;

namespace Thesis
{
    public class TransferToken : TransferBase
    {
        public TransferToken(string url, string contractAddress, string adminPrivateKey, string adminAddress, string userPrivateKey, string userAddress) : base(url, contractAddress, adminPrivateKey, adminAddress, userPrivateKey, userAddress)
        {
        }

        #region functions
        [Function("transfer", "bool")]
        public class TransferFunctionBase : FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To { get; set; }
            [Parameter("uint256", "_value", 2)]
            public BigInteger Value { get; set; }
        }

        public partial class TransferFunction : TransferFunctionBase
        {

        }

        [Function("balanceOf", "uint256")]
        public class BalanceOfFunction : FunctionMessage
        {
            [Parameter("address", "_owner", 1)]
            public string Owner { get; set; }
        }

        [FunctionOutput]
        public class BalanceOfFunctionOutput : IFunctionOutputDTO
        {
            [Parameter("uint256", 1)]
            public int Balance { get; set; }
        }

        [Event("Transfer")]
        public class TransferEventDTOBase : IEventDTO
        {

            [Parameter("address", "_from", 1, true)]
            public virtual string From { get; set; }
            [Parameter("address", "_to", 2, true)]
            public virtual string To { get; set; }
            [Parameter("uint256", "_value", 3, false)]
            public virtual BigInteger Value { get; set; }
        }

        public partial class TransferEventDTO : TransferEventDTOBase
        {
            public static EventABI GetEventABI()
            {
                return EventExtensions.GetEventABI<TransferEventDTO>();
            }
        }
        #endregion

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
            GameCore.CoinManager.StartCoroutine(QueryToken(adminAddress, callback));
        }

        public override void QueryUserBalance(Action<float> callback = null)
        {
            GameCore.CoinManager.StartCoroutine(QueryToken(userAddress, callback));
        }

        IEnumerator QueryToken(string address, Action<float> callback)
        {
            Debug.Log("QueryBalance address :" + address);

            //Query request using our acccount and the contracts address (no parameters needed and default values)
            var queryRequest = new QueryUnityRequest<BalanceOfFunction, BalanceOfFunctionOutput>(url, address);
            yield return queryRequest.Query(new BalanceOfFunction() { Owner = address }, contractAddress);

            //Getting the dto response already decoded
            var dtoResult = queryRequest.Result;
            var balance = dtoResult.Balance;
            GameCore.Log.Write("Balance," + balance);
            Debug.Log(balance);

            if (listeners.Count > 0)
            {
                foreach (var item in listeners)
                {
                    item.OnCheckBalance(balance);
                }
            }
            if (callback != null) callback.Invoke(balance);
        }

        IEnumerator Transfer(int value, string fromAddress, string fromPK, string targetAddress)
        {
            GameCore.Log.Write("Transfer transaction,Start," + value);
            GameCore.Log.Write("Transfer transaction,FromAddress," + fromAddress);
            GameCore.Log.Write("Transfer transaction,FromPrivateKey," + fromPK);
            GameCore.Log.Write("Transfer transaction,TargetAddress," + targetAddress);
            var transactionTransferRequest = new TransactionSignedUnityRequest(url, fromPK, fromAddress);

            var transactionMessage = new TransferFunction
            {
                FromAddress = fromAddress,
                To = targetAddress,
                Value = value,
            };

            yield return transactionTransferRequest.SignAndSendTransaction(transactionMessage, contractAddress);

            var transactionTransferHash = transactionTransferRequest.Result;

            GameCore.Log.Write("Transfer transaction hash," + transactionTransferHash);

            var transactionReceiptPolling = new TransactionReceiptPollingRequest(url);
            yield return transactionReceiptPolling.PollForReceipt(transactionTransferHash, 2);
            var transferReceipt = transactionReceiptPolling.Result;

            var transferEvent = transferReceipt.DecodeAllEvents<TransferEventDTO>();
            var getLogsRequest = new EthGetLogsUnityRequest(url);

            var eventTransfer = TransferEventDTO.GetEventABI();
            yield return getLogsRequest.SendRequest(eventTransfer.CreateFilterInput(contractAddress, fromAddress));

            var eventDecoded = getLogsRequest.Result.DecodeAllEvents<TransferEventDTO>();

            var eventValue = eventDecoded[0].Event.Value;
            BigInteger bi = 0;
            GameCore.Log.Write("Transferd amount from get logs event," + eventValue);
            QueryUserBalance();

            if (eventValue > bi)
            {
                if (listeners.Count > 0)
                {
                    foreach (var item in listeners)
                    {
                        item.OnTransferSussess();
                    }
                }
            }
            else
            {
                if (listeners.Count > 0)
                {
                    foreach (var item in listeners)
                    {
                        item.OnTransferFail();
                    }
                }
            }
        }
    }
}