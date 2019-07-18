using UnityEngine;
using UnityEngine.Events;

namespace Thesis
{
    [RequireComponent(typeof(Collider2D))]
    public class ShopObject : MonoBehaviour
    {
        public UnityEvent OnEnter, OnExit;

        ILogManager log;

        private void Start()
        {
            log = GameCore.Log;
            if (GameCore.BoughtWeapon)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnEnter.Invoke();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            OnExit.Invoke();
        }
        
        public void OpenScene(string sceneName)
        {
            log.Write("OpenShop");
            GameCore.OpenUIScene(sceneName);
        }
    }
}