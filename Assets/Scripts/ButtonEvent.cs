using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
public class ButtonEvent : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshPro _text;

    [Header("Events")]
    public UnityEvent onPressed;   // 누르기 시작
    public UnityEvent onReleased;  // 버튼 위에서 손을 뗌
    public UnityEvent onCanceled;  // 누른 채로 버튼 영역 밖으로 나감

    private int activePointerId = int.MinValue;
    private bool pressed;

    public void OnPointerDown(PointerEventData e)
    {
        // 이미 다른 손이 누르고 있으면 무시
        if (pressed) return;
        pressed = true;
        activePointerId = e.pointerId;
        onPressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (!pressed || e.pointerId != activePointerId) return;
        pressed = false;
        activePointerId = int.MinValue;
        onReleased?.Invoke();
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!pressed || e.pointerId != activePointerId) return;
        pressed = false;
        activePointerId = int.MinValue;
        onCanceled?.Invoke();
    }

    public void OnButtonPressed()
    {
        _text.text = "Pressed";
    }

    public void OnButtonReleased()
    {
        _text.text = "Released";
    }

    public void OnButtonCanceled()
    {
        _text.text = "Canceled";
    }
}
