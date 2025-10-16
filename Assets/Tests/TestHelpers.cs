using System.Collections;
using UnityEngine;

public static class TestHelpers
{
    public static IEnumerator WaitForNumberOfFixedUpdates(int updates) {
        for (int i = 0; i < updates; i++) {
            yield return new WaitForFixedUpdate();
        }
    }
    
    public static IEnumerator WaitForNumberOfFrames(int frames) {
        for (int i = 0; i < frames; i++) {
            yield return null;
        }
    }

    public static float CalculateDistance(float initialVelocity, float acceleration, float time) {
        float vt = initialVelocity * time;
        float half_at2 = 0.5f * acceleration * time * time;
        return vt + half_at2;
    }
}