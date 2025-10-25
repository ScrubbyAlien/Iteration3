using System.Collections;
using System.Linq;
using UnityEngine;

public static class NewtonianMechanics
{
    public static float CalculateDistance(float initialVelocity, float acceleration, float time) {
        float vt = initialVelocity * time;
        float half_at2 = 0.5f * acceleration * time * time;
        return vt + half_at2;
    }

    public static float CalculateInitialVelocity(float distance, float acceleration, float time) {
        float d_over_t = distance / time;
        float halfgt = 0.5f * acceleration * time;
        return d_over_t - halfgt;
    }

}