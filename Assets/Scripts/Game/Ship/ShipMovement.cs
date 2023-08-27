using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls ship based on ShipInput
/// </summary>
public class ShipMovement : MonoBehaviour
{

    [SerializeField] private float speed, rotateSpeed;
    [SerializeField] private Rigidbody2D rigidBody;
    public Rigidbody2D RigidBody { get => rigidBody;  }
    [SerializeField] private float acceleration, angularAcceleration;
    public float Acceleration { get => acceleration; }
    public float AngularAcceleration { get => angularAcceleration; }

    [SerializeField] private float accelerationDecay = 0.01f;

    [SerializeField] private AudioSource engineAudio;

    /// <summary>
    /// Sets up ShipInput.
    /// </summary>
    /// <param name="controller">Master controller of ship.</param>
    public void Init(ShipController controller)
    {
        controller.Input.SetAcceleration += SetAccelerate;
        controller.Input.SetTrigger += Trigger;
        controller.Input.SetAngularAcceleration += SetAngularAcceleration;

        MainCoroutine.OnMainUpdate += Accelerate;
        MainCoroutine.OnMainUpdate += AddTorque;
        GameController.OnPausedStateChange += PauseEngine;

        if (ArduinoInput.IsDisabled)
            accelerationDecay = 0;
    }

    private void OnDisable()
    {
        MainCoroutine.OnMainUpdate -= Accelerate;
        MainCoroutine.OnMainUpdate -= AddTorque;
        GameController.OnPausedStateChange -= PauseEngine;
    }

    public void SetAccelerate(float acceleration)
    {
        this.acceleration = acceleration * speed;
    }

    public void SetAngularAcceleration(float torque)
    {
        angularAcceleration = torque * rotateSpeed / speed;
    }



    public void Accelerate(float deltaTime)
    {
        if (acceleration > 0)
            acceleration -= accelerationDecay * deltaTime;
        else
            acceleration = 0;

        engineAudio.volume = Mathf.Min(acceleration / 100f, 1f);
        rigidBody.AddForce(acceleration * deltaTime * transform.up);
    }

    public void Trigger(bool down)
    {

    }

    public void AddTorque(float deltaTime)
    {
        rigidBody.AddTorque(angularAcceleration * Acceleration / deltaTime);
    }


    public void PauseEngine(bool paused)
    {
        if(paused)
            engineAudio.Pause();
        else
            engineAudio.UnPause();
    }

}
