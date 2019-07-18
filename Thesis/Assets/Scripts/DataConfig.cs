using System;
using Newtonsoft.Json;

namespace Thesis
{
    [Serializable]
    public sealed class DataConfig
    {
        /// <summary>
        /// 預設 Config 檔案名稱
        /// </summary>
        [JsonProperty]
        public static string ConfigPath = "config.json";

        [JsonProperty]
        public static string url = "https://ropsten.infura.io";

        [JsonProperty]
        public static string contractAddress = String.Empty;

        [JsonProperty]
        public static string adminPrivateKey = String.Empty;

        [JsonProperty]
        public static string adminAddress = String.Empty;

        [JsonProperty]
        public static string userPrivateKey = String.Empty;

        [JsonProperty]
        public static string userAddress = String.Empty;

        [JsonProperty]
        public static bool hasNTD = true;

        [JsonProperty]
        public static bool isToken = true;

        [JsonProperty]
        public static float exRate = 1f;

        [JsonProperty]
        public static int nBoxValue = 1;
        [JsonProperty]
        public static int sBoxValue = 3;
        [JsonProperty]
        public static int gBoxValue = 5;
        
        [JsonProperty]
        public static int balanceBeforeDonate = 30;
    }
}