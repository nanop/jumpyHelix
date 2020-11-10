using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorController : MonoBehaviour {

    [Header("Rotator Config")]
    [SerializeField] private float rotatingSpeed = 0.5f;

    private Touch touch;
    private Quaternion rotationY;

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    rotationY = Quaternion.Euler(0f, -touch.deltaPosition.x * rotatingSpeed, 0f);
                    transform.rotation = rotationY * transform.rotation;
                }
            }
            //if (Input.GetMouseButton(0) && GameManager.Instance.IsFinishedFading)
            //{
            //    float x = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
            //    if (x <= 0.5f) //Touch left
            //    {
            //        transform.eulerAngles += Vector3.up * rotatingSpeed * Time.deltaTime;
            //    }
            //    else //Touch right
            //    {
            //        transform.eulerAngles += Vector3.down * rotatingSpeed * Time.deltaTime;
            //    }
            //}
        }
    }

}
