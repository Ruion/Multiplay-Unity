using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class JoystickMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Transform player;
    private RectTransform joystickPad;
    private RectTransform joystickCircle;
    private CharacterController cc;

    public float moveSpeed = 1;
    public float rotateSpeed = 5;
    private Vector3 moveVector;

    // Start is called before the first frame update
    private void OnEnable()
    {
        joystickPad = transform.parent.GetComponent<RectTransform>();
        cc = player.GetComponent<CharacterController>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector2.ClampMagnitude(eventData.position - (Vector2)joystickPad.position, joystickPad.rect.width * 0.5f);

        moveVector = new Vector3(transform.localPosition.x, 0, transform.localPosition.y).normalized;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine("Move");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopCoroutine("Move");
        transform.localPosition = Vector3.zero;
        moveVector = Vector3.zero;
    }

    private IEnumerator Move()
    {
        while (true)
        {
            cc.Move(moveVector * moveSpeed * Time.deltaTime);

            if (moveVector != Vector3.zero)
                player.rotation = Quaternion.Slerp(player.rotation, Quaternion.LookRotation(moveVector), rotateSpeed * Time.deltaTime);

            yield return null;
        }
    }
}