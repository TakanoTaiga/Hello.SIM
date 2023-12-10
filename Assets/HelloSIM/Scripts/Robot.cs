using ROS2;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public ArticulationBody moto_joint_1;
    public ArticulationBody moto_joint_2;
    public ArticulationBody moto_joint_3;
    public ArticulationBody moto_joint_4;
    public GameObject robot_body;
    public TextMeshProUGUI ros2ui;
    public TextMeshProUGUI motorui;
    public TextMeshProUGUI topiclist;
    public TMP_InputField input_namespace;


    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private ISubscription<std_msgs.msg.Float32> motor1_sub;
    private ISubscription<std_msgs.msg.Float32> motor2_sub;
    private ISubscription<std_msgs.msg.Float32> motor3_sub;
    private ISubscription<std_msgs.msg.Float32> motor4_sub;
    private IPublisher<tf2_msgs.msg.TFMessage> tf_pub;
    private IPublisher<geometry_msgs.msg.TransformStamped> robo_tf_pub;
    private string robo_namespace = "a1";

    private float[] targetSpeed = new float[4];

    void Start()
    {
        ros2Unity = GetComponent<ROS2UnityComponent>();
        topiclist.SetText(
            "Sub Topic List\r\n" +
            robo_namespace + "/hellosim/motor_1\r\n" +
            robo_namespace + "/hellosim/motor_2\r\n" +
            robo_namespace + "/hellosim/motor_3\r\n" +
            robo_namespace + "/hellosim/motor_4"
            );
    }

    void Update()
    {
        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode(robo_namespace + "HelloDotSim");
                motor1_sub = ros2Node.CreateSubscription<std_msgs.msg.Float32>(robo_namespace + "/hellosim/motor_1", msg => { targetSpeed[0] = msg.Data; });
                motor2_sub = ros2Node.CreateSubscription<std_msgs.msg.Float32>(robo_namespace + "/hellosim/motor_2", msg => { targetSpeed[1] = msg.Data; });
                motor3_sub = ros2Node.CreateSubscription<std_msgs.msg.Float32>(robo_namespace + "/hellosim/motor_3", msg => { targetSpeed[2] = msg.Data; });
                motor4_sub = ros2Node.CreateSubscription<std_msgs.msg.Float32>(robo_namespace + "/hellosim/motor_4", msg => { targetSpeed[3] = msg.Data; });
                tf_pub = ros2Node.CreatePublisher<tf2_msgs.msg.TFMessage>("/tf");
                robo_tf_pub = ros2Node.CreatePublisher<geometry_msgs.msg.TransformStamped>(robo_namespace + "/hellosim/robo_tf");
            }

            ros2ui.SetText(
                "hellosim/motor_1: " + targetSpeed[0].ToString("0.0") +
                "\r\nhellosim/motor_2: " + targetSpeed[1].ToString("0.0") +
                "\r\nhellosim/motor_3: " + targetSpeed[2].ToString("0.0") +
                "\r\nhellosim/motor_4: " + targetSpeed[3].ToString("0.0")
                );

            moto_joint_1.SetDriveTargetVelocity(ArticulationDriveAxis.X, CheckError(targetSpeed[0]) * 200 * Random.Range(0.9f, 1.1f));
            moto_joint_2.SetDriveTargetVelocity(ArticulationDriveAxis.X, CheckError(targetSpeed[1]) * 200 * Random.Range(0.9f, 1.1f)); 
            moto_joint_3.SetDriveTargetVelocity(ArticulationDriveAxis.X, CheckError(targetSpeed[2]) * 200 * Random.Range(0.9f, 1.1f));
            moto_joint_4.SetDriveTargetVelocity(ArticulationDriveAxis.X, CheckError(targetSpeed[3]) * 200 * Random.Range(0.9f, 1.1f));

            motorui.SetText(
                "Robot Pos(z-up)\r\n" +
                "x: " + robot_body.transform.position.x.ToString("f3") + "\r\n" +
                "y: " + robot_body.transform.position.z.ToString("f3") + "\r\n" +
                "z: " + robot_body.transform.position.y.ToString("f3") + "\r\n" +
                "z: " + robot_body.transform.eulerAngles.y.ToString("f3") + "‹\r\n"
                );

            // publish tf
            var pose_robot = new geometry_msgs.msg.TransformStamped();
            pose_robot.Header.Frame_id = "map";
            pose_robot.Child_frame_id = robo_namespace + "_base_link";
            pose_robot.Transform = GlobalObjTramsformToROS2(robot_body);
            robo_tf_pub.Publish(pose_robot);

            var tf = new tf2_msgs.msg.TFMessage
            {
                Transforms = new geometry_msgs.msg.TransformStamped[1] { pose_robot }
            };
            tf_pub.Publish(tf);
        }
    }

    float CheckError(float input)
    {
        if(input > 1.0){return (float)1.0;}
        if (input < -1.0){return (float)-1.0;}

        return (float)input;
    }

    geometry_msgs.msg.Transform GlobalObjTramsformToROS2(GameObject obj)
    {
        var transform_rt = new geometry_msgs.msg.Transform();
        transform_rt.Translation.X = obj.transform.position.x;
        transform_rt.Translation.Y = obj.transform.position.z;
        transform_rt.Translation.Z = obj.transform.position.y;
        transform_rt.Rotation.X = obj.transform.rotation.x;
        transform_rt.Rotation.Y = obj.transform.rotation.z;
        transform_rt.Rotation.Z = obj.transform.rotation.w;
        transform_rt.Rotation.W = obj.transform.rotation.y;
        return transform_rt;
    }

    public void changeNamespace()
    {
        robo_namespace = input_namespace.text;
        if(ros2Node.node.Name == robo_namespace + "HelloDotSim") { return; }
        ros2Node.RemoveSubscription<std_msgs.msg.Float32>(motor1_sub);
        ros2Node.RemoveSubscription<std_msgs.msg.Float32>(motor2_sub);
        ros2Node.RemoveSubscription<std_msgs.msg.Float32>(motor3_sub);
        ros2Node.RemoveSubscription<std_msgs.msg.Float32>(motor4_sub);
        ros2Node.RemovePublisher<geometry_msgs.msg.TransformStamped>(robo_tf_pub);
        ros2Node = null;
        topiclist.SetText(
            "Sub Topic List\r\n" +
            robo_namespace + "/hellosim/motor_1\r\n" +
            robo_namespace + "/hellosim/motor_2\r\n" +
            robo_namespace + "/hellosim/motor_3\r\n" +
            robo_namespace + "/hellosim/motor_4"
            );
    }
}
