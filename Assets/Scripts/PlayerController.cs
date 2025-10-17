using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GeometryBody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private GeometryBody body;
    
    [SerializeField]
    private JumpingParameters jumpingParameters;
    public JumpingParameters jumpingParams => jumpingParameters;

    private JumpingParameters.JumpValues values;

    private bool onGround;
    
    private void Awake() {
        body = GetComponent<GeometryBody>();
        InputSystem.actions.FindAction("Player/Jump").performed += Jump;
    }

    private void Start() {
        values = jumpingParameters.CalculateJumpValues();
        body.SetGravity(values.gravityDown);
        body.SetXVelocity(speed);
        body.OnCollide += (_) => onGround = true;
    }

    public void Jump(InputAction.CallbackContext context) {
        body.IgnoreCollisionsFor1Frame();
        onGround = false;
        body.SetYVelocity(values.velocity);
        body.SetGravity(values.gravityUp);
        StartCoroutine(SetDownGravity(jumpingParameters.timeUp));
    }

    private IEnumerator SetDownGravity(float delay) {
        yield return new WaitForSeconds(delay);
        body.SetGravity(values.gravityDown);
    }
    
    public void SetJumpingParameters(JumpingParameters newParameters) {
        jumpingParameters = newParameters;
        values = jumpingParameters.CalculateJumpValues();
    }
}