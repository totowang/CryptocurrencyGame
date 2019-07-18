using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ConvertToText : MonoBehaviour
{
    private Text text;
    private int intN = 0;
    private float floatN = 0f;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void Add(int number)
    {
        intN += number;
        text.text = intN.ToString();
    }

    public void Add(float number)
    {
        floatN += number;
        text.text = floatN.ToString();
    }

    public void UpdateText(int number)
    {
        intN = number;
        text.text = intN.ToString();
    }

    public void UpdateText(float number)
    {
        floatN = number;
        text.text = floatN.ToString();
    }
}
