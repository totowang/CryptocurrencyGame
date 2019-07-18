using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Thesis
{
    public class ThesisSceneController : MonoBehaviour
    {
        public UnityEvent OnStart;
        public UnityEvent OnDonated;

        private void Start()
        {
            GameCore.CoinManager.DonateSubmitButton.onClick.AddListener(()=>OnDonated.Invoke());
            OnStart.Invoke();
        }

        public void TransferTo(string sceneName)
        {
            GameCore.TransitionTo(sceneName);
            GameCore.Log.Write("TransitionTo," + sceneName);
        }

        public void OpenCoinUI()
        {
            GameCore.CoinManager.Display(true);
        }

        public void OpenDonateUI(bool active)
        {
            GameCore.CoinManager.OpenDonateUI(active);
        }

        public void OpenDonateUI(float sec)
        {
            StartCoroutine(OpenDonateUIDelay(sec));
        }

        private IEnumerator OpenDonateUIDelay(float sec)
        {
            yield return new WaitForSeconds(sec);
            OpenDonateUI(true);
        }

        public void Ending()
        {
            GameCore.Log.Write("The End");
        }

        private void OnApplicationQuit()
        {
            GameCore.Log.Write("OnApplicationQuit");
        }

        private void OnApplicationPause(bool pause)
        {
            GameCore.Log.Write("OnApplicationPause," + pause);
        }
    }
}