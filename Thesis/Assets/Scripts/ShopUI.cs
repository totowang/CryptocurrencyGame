using Thesis;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public Text NBoxValueText;
    public Text SBoxValueText;
    public Text GBoxValueText;
    public Text NBoxNTDText;
    public Text SBoxNTDText;
    public Text GBoxNTDText;

    [SerializeField]
    GameObject[] ntdObjects;

    WeaponBoxType weaponBoxType = WeaponBoxType.Normal;

    private void Start()
    {
        NBoxValueText.text = DataConfig.nBoxValue.ToString();
        SBoxValueText.text = DataConfig.sBoxValue.ToString();
        GBoxValueText.text = DataConfig.gBoxValue.ToString();

        if (!DataConfig.hasNTD && ntdObjects.Length > 1)
        {
            foreach (var item in ntdObjects)
            {
                item.SetActive(false);
            }
        }
        if (DataConfig.hasNTD)
        {
            NBoxNTDText.text = (DataConfig.nBoxValue * DataConfig.exRate).ToString("g5");
            SBoxNTDText.text = (DataConfig.sBoxValue * DataConfig.exRate).ToString("g5");
            GBoxNTDText.text = (DataConfig.gBoxValue * DataConfig.exRate).ToString("g5");
        }
    }

    public void ExitPause()
    {
        GameCore.Unpause();
    }
    
    public void SelectWeaponBox(int type)
    {
        weaponBoxType = (WeaponBoxType)type;
    }

    public void BuyWeapon()
    {
        GameCore.BuyWeaponBox(weaponBoxType);
    }
}
