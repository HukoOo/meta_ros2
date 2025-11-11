using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasRenderer))] // UI 객체임을 암시 (선택)
public class RosManager : MonoBehaviour
{
    private ROSConnection ros;

    [Header("Defaults")]
    public string defaultIP = "192.168.2.14";
    public string defaultPort = "10000";

    [Header("UI Refs")]
    [SerializeField] private RawImage statusLamp;         // 상태 표시용 램프
    [SerializeField] private TMP_InputField ipInputField; // IP 입력칸 (선택)
    [SerializeField] private TMP_Text portText;           // 포트 표시/입력 (선택)

    // 선택: 온스크린 키패드
    public Numpad numpad;

    private void Awake()
    {
        // 인스펙터에 안 꽂혀 있으면 같은 오브젝트에서 찾아보기 (보조)
        if (!statusLamp) statusLamp = GetComponent<RawImage>();
        if (!ipInputField) ipInputField = GetComponent<TMP_InputField>();
    }

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        string ip = defaultIP;
        int port = int.Parse(defaultPort);

        // (원하면 PlayerPrefs에서 불러오기)
        if (PlayerPrefs.HasKey("ip"))   ip   = PlayerPrefs.GetString("ip");
        if (PlayerPrefs.HasKey("port")) port = PlayerPrefs.GetInt("port");

        ros.RosIPAddress = ip;
        ros.RosPort = port;
        ros.Connect();

        // UI 반영
        if (ipInputField) ipInputField.text = ip;
        if (portText)     portText.text     = port.ToString();

        // numpad 이벤트 연결(있다면)
        if (numpad != null)
            numpad.OnButtonPressed += OnNumPadButtonPressed;
    }

    void OnDestroy()
    {
        if (numpad != null)
            numpad.OnButtonPressed -= OnNumPadButtonPressed;
    }

    void Update()
    {
        // 안전 가드
        if (ros == null || statusLamp == null) return;

        statusLamp.color = ros.HasConnectionError ? Color.red : Color.green;
    }

    // TMP_InputField의 OnEndEdit에 연결해도 됨
    public void InputIPFinished(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) return;

        ros.RosIPAddress = ip.Trim();
        PlayerPrefs.SetString("ip", ros.RosIPAddress);
        PlayerPrefs.Save();

        Reconnect();
        Debug.Log("[ROS] Set IP to " + ros.RosIPAddress);
    }

    public void InputPortFinished(string portStr)
    {
        if (int.TryParse(portStr, out var port))
        {
            port = Mathf.Clamp(port, 1, 65535);
            ros.RosPort = port;
            PlayerPrefs.SetInt("port", ros.RosPort);
            PlayerPrefs.Save();
            Reconnect();
        }
    }

    private void Reconnect()
    {
        ros.Disconnect();
        ros.Connect();
    }

    // 예시: numpad에서 0~9, '.'(-1), backspace(-2) 받는 경우
    private void OnNumPadButtonPressed(int num)
    {
        if (!ipInputField) return;

        string text = ipInputField.text ?? string.Empty;

        if (num == -2) // backspace
        {
            if (text.Length > 0) text = text.Substring(0, text.Length - 1);
        }
        else if (num == -1) // '.'
        {
            text += ".";
        }
        else if (num == 10) // connect
        {
            Reconnect();
        }
        else // 0~9
        {
            text += num.ToString();
        }

        ipInputField.text = text;
    }
}
