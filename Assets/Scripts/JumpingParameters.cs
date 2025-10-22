using System;
using UnityEngine;

[Serializable]
public struct JumpingParameters
{
    // https://www.youtube.com/watch?v=IOe1aGY6hXA&list=PLGKTwK4yOQ_dpXp3FCPumQAwf-6fjERKe

    // public const float FloatPointErrorCompensator = 1.000846f;
    
    [Min(0)]
    public float height;
    public float heightx2 => height * 2;
    [Min(0.01f)]
    public float timeUp;
    public float timeUpSqr => timeUp * timeUp;
    public static bool logErrors = true;

    public JumpingParameters(float h, float tu) {
        height = h;
        timeUp = tu;
    }
    
    public JumpValues CalculateJumpValues() {
        if (timeUp == 0) {
            if (logErrors) {
                Debug.LogWarning("Jumping time is zero, aborting jumping calculations to avoid division by zero.");
            }
            return default;
        }
        float jumpVelocity = (heightx2) / timeUp;
        float upGravity = (-heightx2) / (timeUpSqr);
        return new JumpValues(jumpVelocity, upGravity);
    }

    public struct JumpValues
    {
        public float velocity;
        public float gravity;

        public JumpValues(float v = 0, float u = -1) {
            velocity = v;
            gravity = u;
        }
        
    }
}