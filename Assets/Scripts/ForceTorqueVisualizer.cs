using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class ForceTorqueVisualizer : MonoBehaviour
{
    [Header("ROS Settings")]
    public string wrenchTopic = "/aft_sensor1/wrench";
    ROSConnection ros;

    [Header("Arrow Parts")]
    public Transform arrowBody;   // Cylinder
    public Transform arrowHead;   // Cone

    [Header("Visual Settings")]
    public float scaleFactor = 0.1f;   // 힘 → 길이 변환
    public float thicknessFactor = 0.02f; // 힘 → 굵기 변환
    public float smooth = 8f;          // 시각적 보간(부드럽게 변화)

    private Vector3 targetForce;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<WrenchStampedMsg>(wrenchTopic, OnWrenchReceived);
    }

    void OnWrenchReceived(WrenchStampedMsg msg)
    {
        // ROS → Unity 좌표계 변환 (필요시)
        targetForce = new Vector3(
            (float)msg.wrench.force.x,
            (float)msg.wrench.force.y,
            (float)msg.wrench.force.z
        );
    }

    void Update()
    {
        // 보간된 힘 벡터
        Vector3 force = Vector3.Lerp(Vector3.zero, targetForce, Time.deltaTime * smooth);
        float magnitude = force.magnitude;

        if (magnitude < 1e-3f)
        {
            arrowBody.localScale = Vector3.zero;
            arrowHead.localScale = Vector3.zero;
            return;
        }

        // 방향
        transform.rotation = Quaternion.LookRotation(force.normalized, Vector3.up);

        // 길이 (Z 방향)
        float arrowLength = magnitude * scaleFactor;

        // 굵기 (힘 크기에 비례)
        float thickness = Mathf.Clamp(magnitude * thicknessFactor, 0.005f, 0.05f);

        // Body 길이: 전체 화살 길이의 80%
        arrowBody.localScale = new Vector3(thickness, arrowLength * 0.4f, thickness);
        arrowBody.localPosition = new Vector3(0, 0, arrowLength * 0.4f);

        // Head 위치 & 크기
        arrowHead.localScale = new Vector3(thickness * 1.5f, arrowLength * 0.2f, thickness * 1.5f);
        arrowHead.localPosition = new Vector3(0, 0, arrowLength * 0.9f);
    }
}
