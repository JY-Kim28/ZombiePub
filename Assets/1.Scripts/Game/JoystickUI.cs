using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform joystickBG;
    [SerializeField] RectTransform joystickBar;


    bool touched;
    Vector2 pressPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        touched = true;
        pressPos = eventData.position;

        joystickBG.position = pressPos;
        joystickBG.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touched = false;

        joystickBG.gameObject.SetActive(false);

        Game.Player.MoveStop();
    }

    void Update()
    {
        if (touched)
        {
            //Vector2 touchPos = Input.GetTouch(0).position;
            Vector2 touchPos = TouchUtils.GetTouchPoint();

            Vector3 fromCenter = touchPos - pressPos;
            if (fromCenter.x < -150) fromCenter.x = -150;
            else if (fromCenter.x > 150) fromCenter.x = 150;

            if (fromCenter.y < -150) fromCenter.y = -150;
            else if (fromCenter.y > 150) fromCenter.y = 150;

            joystickBar.anchoredPosition = fromCenter;


            Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, fromCenter);

            Game.Player.Move(quaternion);

        }
    }

}
