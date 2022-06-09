using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GestureDetector : MonoBehaviour
{
    private Touch touch;
    private Vector2 beginTouchPosition = Vector2.zero;
    private Vector2 endTouchPosition = Vector2.zero;

    private bool sendingSwipesIsEnabled;
    public SwipeData LastSwipe { get; set;  }
    public static event Action<SwipeData> OnSwipe = delegate { };
    public static event Action<SwipeData> OnReleaseSwipe = delegate { };

    private void Start()
    {
        Input.multiTouchEnabled = false;
    }


    void Update()
    {
        if(Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    beginTouchPosition = touch.position;
                    sendingSwipesIsEnabled = true;
                    break;
                case TouchPhase.Moved:
                    endTouchPosition = touch.position;
                    if (!ShouldDiscardSwipe(beginTouchPosition) && sendingSwipesIsEnabled)
                    {
                        Direction direction = DetectSwipe();
                        var data = ToData(direction);
                        sendingSwipesIsEnabled = false;
                        OnSwipe?.Invoke(data);
                    }
                    break;
                case TouchPhase.Ended:
                    endTouchPosition = touch.position;
                    if (!ShouldDiscardSwipe(beginTouchPosition))
                    {
                        Direction direction = DetectSwipe();
                        var data = ToData(direction);
                        OnReleaseSwipe?.Invoke(data);
                    }
                    break;
            }
        }
    }

    private Direction DetectSwipe()
    {
        float horizontalDistance = Mathf.Abs(endTouchPosition.x - beginTouchPosition.x);
        float verticalDistance = Mathf.Abs(endTouchPosition.y - beginTouchPosition.y);
        Direction direction = new Direction();

        if((horizontalDistance > 0.5F) || (verticalDistance > 0.5F))
        {
            if (horizontalDistance > verticalDistance)
                direction = beginTouchPosition.x - endTouchPosition.x > 0 ? Direction.Left : Direction.Right;
            else
                direction = beginTouchPosition.y - endTouchPosition.y > 0 ? Direction.Down : Direction.Up;
        }
        return direction;
    }

    private SwipeData ToData(Direction direction)
    {
        SwipeData data = new SwipeData()
        {
            Direction = direction,
            BeginTouchPosition = beginTouchPosition,
            EndTouchPosition = endTouchPosition
        };
        return data;
    }

    private bool ShouldDiscardSwipe(Vector2 touchPos)
    {   
        PointerEventData touch = new PointerEventData(EventSystem.current);
        touch.position = touchPos;
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(touch, hits);
        return (hits.Count > 0); // discard swipe if an UI element is beneath
    }

    public struct SwipeData
    {
        public Direction Direction;
        public Vector2 BeginTouchPosition;
        public Vector2 EndTouchPosition;
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
