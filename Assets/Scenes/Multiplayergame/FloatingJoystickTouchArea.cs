using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystickTouchArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform joystickPad;
    [SerializeField] private RectTransform joystickCircle;

    public CharacterController cc;
    public Transform player;

    public float moveSpeed = 3;
    public float rotateSpeed = 5;
    private Vector3 moveVector;

    public void OnDrag(PointerEventData eventData)
    {
        joystickCircle.transform.localPosition = Vector2.ClampMagnitude(eventData.position - (Vector2)joystickPad.position, joystickPad.rect.width * 0.5f);

        moveVector = new Vector3(joystickCircle.transform.localPosition.x, 0, joystickCircle.transform.localPosition.y).normalized;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (player == null || cc == null) return;

        joystickPad.position = eventData.position;

        StartCoroutine("Move");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickCircle.transform.localPosition = Vector3.zero;
        moveVector = Vector3.zero;
        StopCoroutine("Move");
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