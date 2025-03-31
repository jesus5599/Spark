using UnityEngine;

public class ChainLink : MonoBehaviour
{
    public Rigidbody connectedBody; // Asigna en el inspector el eslabón anterior

    void Start()
    {
        HingeJoint joint = gameObject.AddComponent<HingeJoint>();
        joint.connectedBody = connectedBody;
        joint.useLimits = true;
        JointLimits limits = new JointLimits();
        limits.min = -45;
        limits.max = 45;
        joint.limits = limits;
    }
}
