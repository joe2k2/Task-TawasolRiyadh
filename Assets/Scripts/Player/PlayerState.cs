using UnityEngine;
using System;

[Serializable]
public struct PlayerState
{
    public float timestamp;
    public Vector3 position;
    public Vector3 velocity;
    public bool isGrounded;
    public bool isJumping;

    public PlayerState(float time, Vector3 pos, Vector3 vel, bool grounded, bool jumping)
    {
        timestamp = time;
        position = pos;
        velocity = vel;
        isGrounded = grounded;
        isJumping = jumping;
    }
}
