using ROS2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public ArticulationBody moto_joint_1;
    public ArticulationBody moto_joint_2;
    public ArticulationBody moto_joint_3;
    public ArticulationBody moto_joint_4;
    public GameObject robot_body;


    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private ISubscription<std_msgs.msg.Float32> motor1_sub;
    private ISubscription<std_msgs.msg.Float32> motor2_sub;
    private ISubscription<std_msgs.msg.Float32> motor3_sub;
    private ISubscription<std_msgs.msg.Float32> motor4_sub;
    private IPublisher<tf2_msgs.msg.TFMessage> tf_pub;

    private float[] targetSpeed = new float[4];

    void Start()
    {
        ros2Unity = GetComponent<ROS2UnityComponent>();
    }

    void Update()
    {
        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode("HelloDotSim");
                motor1_sub = ros2Node.CreateSubscription<std_msgs.msg.Float32>("hellosim/motor_1", msg => { targetSpeed[0] = msg.Data; });
                motor2_sub = ros2Node.CreateSubscription<std_msgs.msg.Float32>("hellosim/motor_2", msg => { targetSpeed[1] = msg.Data; });
                motor3_sub = ros2Node.CreateSubscription<std_msgs.msg.Float32>("hellosim/motor_3", msg => { targetSpeed[2] = msg.Data; });
                motor4_sub = ros2Node.CreateSubscription<std_msgs.msg.Float32>("hellosim/motor_4", msg => { targetSpeed[3] = msg.Data; });
                tf_pub = ros2Node.CreatePublisher<tf2_msgs.msg.TFMessage>("/tf");
            }

            moto_joint_1.SetDriveTargetVelocity(ArticulationDriveAxis.X, checkError(targetSpeed[0]) * 200 * Random.Range(0.9f, 1.1f));
            moto_joint_2.SetDriveTargetVelocity(ArticulationDriveAxis.X, checkError(targetSpeed[1]) * 200 * Random.Range(0.9f, 1.1f)); 
            moto_joint_3.SetDriveTargetVelocity(ArticulationDriveAxis.X, checkError(targetSpeed[2]) * 200 * Random.Range(0.9f, 1.1f));
            moto_joint_4.SetDriveTargetVelocity(ArticulationDriveAxis.X, checkError(targetSpeed[3]) * 200 * Random.Range(0.9f, 1.1f));

            // publish tf
            var pose_robot = new geometry_msgs.msg.TransformStamped();
            pose_robot.Header.Frame_id = "map";
            pose_robot.Child_frame_id = "base_link";
            pose_robot.Transform = globalObjTramsformToROS2(robot_body);

            var tf = new tf2_msgs.msg.TFMessage();
            tf.Transforms = new geometry_msgs.msg.TransformStamped[1]{pose_robot};
            tf_pub.Publish(tf);
        }
    }

    float checkError(float input)
    {
        if(input > 1.0){return (float)1.0;}
        if (input < -1.0){return (float)1.0;}

        return (float)input;
    }

    geometry_msgs.msg.Transform globalObjTramsformToROS2(GameObject obj)
    {
        var transform_rt = new geometry_msgs.msg.Transform();
        transform_rt.Translation.X = (float)(obj.transform.position.x);
        transform_rt.Translation.Y = (float)(obj.transform.position.z);
        transform_rt.Translation.Z = (float)(obj.transform.position.y);
        transform_rt.Rotation.X = (float)(obj.transform.rotation.x);
        transform_rt.Rotation.Y = (float)(obj.transform.rotation.z);
        transform_rt.Rotation.Z = (float)(obj.transform.rotation.w);
        transform_rt.Rotation.W = (float)(obj.transform.rotation.y);
        return transform_rt;
    }
}
