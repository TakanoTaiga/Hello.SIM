using ROS2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TfPublisher : MonoBehaviour
{
    public GameObject[] tf_gameobjects;

    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<tf2_msgs.msg.TFMessage> tf_pub;

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
                ros2Node = ros2Unity.CreateNode("HelloDotSimTF");
                tf_pub = ros2Node.CreatePublisher<tf2_msgs.msg.TFMessage>("/tf");
            }

            var tf = new tf2_msgs.msg.TFMessage();
            tf.Transforms = new geometry_msgs.msg.TransformStamped[tf_gameobjects.Length];
            for(var i = 0; i < tf_gameobjects.Length; i++)
            {
                tf.Transforms[i] = new geometry_msgs.msg.TransformStamped();
                tf.Transforms[i].Header.Frame_id = "map";
                tf.Transforms[i].Child_frame_id = tf_gameobjects[i].name;
                tf.Transforms[i].Transform = GlobalObjTramsformToROS2(tf_gameobjects[i]);
            }
            tf_pub.Publish(tf);
        }
    }
    geometry_msgs.msg.Transform GlobalObjTramsformToROS2(GameObject obj)
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
