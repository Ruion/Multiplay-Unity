using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    private void OnEnable()
    {
        if (!photonView.IsMine) return;
        FloatingJoystickTouchArea joystick = FindObjectOfType<FloatingJoystickTouchArea>();
        joystick.player = transform;
        joystick.cc = GetComponent<CharacterController>();
    }
}