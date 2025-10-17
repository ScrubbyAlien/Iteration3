using System;
using UnityEngine;

[Serializable]
public struct JumpingParameters
{
    // https://www.youtube.com/watch?v=IOe1aGY6hXA&list=PLGKTwK4yOQ_dpXp3FCPumQAwf-6fjERKe
        
    [Min(0)]
    public float height;
    public float heightx2 => height * 2f;
    [Min(0.1f)]
    public float timeUp;
    public float timeUpSqr => timeUp * timeUp;
    [Min(0.1f)]
    public float timeDown;
    public float timeDownSqr => timeDown * timeDown;
    public static bool logErrors = true;

    public JumpingParameters(float h, float tu, float td) {
        height = h;
        timeUp = tu;
        timeDown = td;
    }
    
    public JumpValues CalculateJumpValues() {
        if (timeUp == 0 || timeDown == 0) {
            if (logErrors) {
                Debug.LogWarning("Jumping time is zero, aborting jumping calculations to avoid division by zero.");
            }
            return default;
        }
        float jumpVelocity = (heightx2) / timeUp;
        float upGravity = (-heightx2) / (timeUpSqr);
        float downGravity = (-heightx2) / (timeDownSqr);
        return new JumpValues(jumpVelocity, upGravity, downGravity);
    }

    public struct JumpValues
    {
        public float velocity;
        public float gravityUp;
        public float gravityDown;

        public JumpValues(float v = 0, float u = -1, float d = -1) {
            velocity = v;
            gravityUp = u;
            gravityDown = d;
        }
        
    }
}