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
}