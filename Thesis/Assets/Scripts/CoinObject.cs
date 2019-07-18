using UnityEngine;
using UnityEngine.Events;

namespace Thesis
{
    [RequireComponent(typeof(Collider2D))]
    public class CoinObject : MonoBehaviour
    {
        public UnityEvent OnEnter, OnExit;

        ILogManager log;

        private void Start()
        {
            log = GameCore.Log;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnEnter.Invoke();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            OnExit.Invoke();
        }

        public void GetCoin(int value)
        {
            log.Write("GetCoinTrigger," + value);
            GameCore.GiveReward(value);
        }

        public void OpenScene(string sceneName)
        {
            GameCore.OpenUIScene(sceneName);
        }
    }
}