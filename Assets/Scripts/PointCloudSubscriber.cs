using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using System;
using System.Collections.Generic;

public class PointCloudSubscriber_Instanced : MonoBehaviour
{
    ROSConnection ros;
    public string pointCloudTopic = "/camera/camera/depth/color/points";

    [Header("Rendering Settings")]
    public Material instancedMaterial;  // GPU instancing 지원 셰이더 (아래 설명)
    public Mesh sphereMesh;
    [Range(0.001f, 0.05f)]
    public float pointSize = 0.01f;
    [Range(0f, 1f)]
    public float alpha = 1.0f;

    private int numPoints = 0;
    private bool cloudReady = false;
    private Matrix4x4[] matrices;
    private Vector4[] colors;
    private MaterialPropertyBlock mpb;

    const int BATCH_SIZE = 1023; // GPU instancing 최대 단위

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PointCloud2Msg>(pointCloudTopic, OnPointCloud);
        // sphereMesh가 비어 있으면 Unity 기본 Sphere Mesh 자동 할당
        if (sphereMesh == null)
            sphereMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx"); // ✅ Unity 내장 Sphere Mesh

        mpb = new MaterialPropertyBlock();
    }

    void OnPointCloud(PointCloud2Msg msg)
    {
        int width = (int)msg.width;
        int height = (int)msg.height;
        int pointStep = (int)msg.point_step;
        numPoints = width * height;

        if (numPoints == 0 || msg.data == null) return;

        matrices = new Matrix4x4[numPoints];
        colors = new Vector4[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            int baseIndex = i * pointStep;

            float x = BitConverter.ToSingle(msg.data, baseIndex + 0);
            float y = BitConverter.ToSingle(msg.data, baseIndex + 4);
            float z = BitConverter.ToSingle(msg.data, baseIndex + 8);

            // ROS -> Unity 변환
            Vector3 pos = new Vector3(x, -y, z);

            // RGB 추출
            float rgb = BitConverter.ToSingle(msg.data, baseIndex + 16);
            byte[] colorBytes = BitConverter.GetBytes(rgb);
            byte r = colorBytes[0];
            byte g = colorBytes[1];
            byte b = colorBytes[2];

            // 개별 포인트 transform
            matrices[i] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * pointSize);
            colors[i] = new Vector4(r / 255f, g / 255f, b / 255f, alpha);
        }

        cloudReady = true;
    }

    void Update()
    {
        if (!cloudReady || matrices == null) return;

        // GPU 인스턴싱 드로우
        for (int i = 0; i < numPoints; i += BATCH_SIZE)
        {
            int batchCount = Mathf.Min(BATCH_SIZE, numPoints - i);
            mpb.SetVectorArray("_Color", colors); // 셰이더 속성에 색상 배열 전달
            Graphics.DrawMeshInstanced(
                sphereMesh,
                0,
                instancedMaterial,
                new ArraySegment<Matrix4x4>(matrices, i, batchCount).ToArray(),
                batchCount,
                mpb,
                UnityEngine.Rendering.ShadowCastingMode.Off,
                false
            );
        }
    }
}
