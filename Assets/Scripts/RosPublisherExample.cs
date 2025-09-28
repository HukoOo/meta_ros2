using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry; // PoseStamped

/// <summary>
///
/// </summary>
public class RosPublisherExample : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "pos_rot";

    // The game object
    public GameObject cube;
    // Publish the cube's position and rotation every N seconds
    public float publishMessageFrequency = 0.5f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topicName);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            cube.transform.rotation = Random.rotation;

            PoseStampedMsg cubePos = new PoseStampedMsg();
            cubePos.pose.position.x = cube.transform.position.x;
            cubePos.pose.position.y = cube.transform.position.y;
            cubePos.pose.position.z = cube.transform.position.z;
            cubePos.pose.orientation.x = cube.transform.rotation.x;
            cubePos.pose.orientation.y = cube.transform.rotation.y;
            cubePos.pose.orientation.z = cube.transform.rotation.z;
            cubePos.pose.orientation.w = cube.transform.rotation.w;
            // Finally send the message to server_endpoint.py running in ROS
            ros.Publish(topicName, cubePos);

            timeElapsed = 0;
        }
    }
}