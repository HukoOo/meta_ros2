using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshPro _text;

    public void OnPointerEnter()
    {
        _text.text = "Enter";
    }

    public void OnPointerExit()
    {
        _text.text = "Exit";
    }

    public void OnPointerDown()
    {
        _text.text = "Down";
    }
    public void OnPointerUp()
    {
        _text.text = "Up";
    }

    public void OnPointerClick()
    {
        _text.text = "Click";
    }
}
