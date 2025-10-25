using System;
using UnityEngine;

public class BeatDrawer : MonoBehaviour
{
    [SerializeField]
    private bool hide;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float bpm;
    [SerializeField]
    private float lengthOfLevelInSeconds;
    [SerializeField]
    private float offset;
    
    
    private void OnDrawGizmos() {
        if (hide) return;
        Gizmos.color = Color.red;
        float beatsPerSecond = bpm / 60f;
        float unitsPerBeat = speed / beatsPerSecond;
        float accruedLength = 0;
        while (accruedLength < lengthOfLevelInSeconds * speed) {
            Gizmos.DrawLine(new Vector3(offset + accruedLength, 100f, 0f), new Vector3(offset + accruedLength, -100f));
            accruedLength += unitsPerBeat;
        }
    }
}