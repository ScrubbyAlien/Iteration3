using System.Collections;
using UnityEngine;

public class Cooldown
{
    private float cooldownLength;
    private float finishTime;
    private bool started;

    public Cooldown(float length) {
        cooldownLength = length;
    }
    public Cooldown() {
        cooldownLength = 0f;
    }

    public void Start() {
        // prevents restarting the cooldown while it is ongoing
        // if restarting is desired behaviour call Restart instead
        if (started) return;
        started = true;
        finishTime = Time.time + cooldownLength;
    }

    public void Stop() {
        started = false;
        finishTime = 0;
    }

    public void Restart() {
        Stop();
        Start();
    }
    
    public bool on {
        get {
            started = Time.time < finishTime;
            return started;
        }
    }
}