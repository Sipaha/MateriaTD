using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class SCamera : MonoBehaviour {

    const float LockedDelta = 0.3f;

    public Grid MapGrid;
    public SCursor Cursor; 

    private float _leftLimit;
    private float _rightLimit;
    private float _topLimit;
    private float _bottomLimit;
    private Vector2 _cameraHalfSize;

    private Vector3 _initialCameraPosition;
    private Vector3 _initialTouch0Position;
    private Vector3 _initialTouch1Position;
    private Vector3 _initialMidPointScreen;
    private float _initialOrthographicSize;
    private bool _zoom;

    private Vector3 _deltaAccum;
    private Vector3 _downPos;
    private bool _dragged = false;
    private bool _locked = true;

    public float MinSize = 3f;
    public float MaxSize = 1000f;

    private EventSystem _eventSystem;

    private Camera _camera;
    private int _pixelW, _pixelH;
    private float _aspect;
    private float _defaultOrthoSize;

    void Awake () {
        _camera = GetComponent<Camera>();

	    _topLimit = 0;
        _leftLimit = 0;
        _bottomLimit = -MapGrid.Size.Y * MapGrid.CellSize.y;
        _rightLimit = MapGrid.Size.X * MapGrid.CellSize.x;

        _pixelW = Screen.width;
        _pixelH = Screen.height;
        _aspect = _pixelW / (float)_pixelH;
        _defaultOrthoSize = 3.5f * _pixelH / (2*Screen.dpi);
        UpdateMaxOrthoSize();
        SetOrthoSize(_defaultOrthoSize);
        SetPos(new Vector3(0,0,-10));

        _eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
    }

    void SetOrthoSize(float size)
    {
        float correctedSize = CheckOrthoSize(size);
        _camera.orthographicSize = correctedSize;
        _cameraHalfSize = new Vector2(correctedSize * _aspect, correctedSize);
    }

    float CheckOrthoSize(float size)
    {
        if (size > MaxSize) return MaxSize;
        return size < MinSize ? MinSize : size;
    }

    void UpdateOrthoSize()
    {
        SetOrthoSize(_camera.orthographicSize);
    }
       
    void SetPos(Vector3 pos)
    {
        transform.position = CheckPosBounds(pos);
    }

    void UpdatePos()
    {
        SetPos(transform.position);
    }

    Vector3 CheckPosBounds(Vector3 pos)
    {
        float left = _leftLimit + _cameraHalfSize.x;
        float right = _rightLimit - _cameraHalfSize.x;
        float top = _topLimit - _cameraHalfSize.y;
        float bottom = _bottomLimit + _cameraHalfSize.y;
        if (pos.x < left)
        {
            pos.x = left;
        }
        else if (pos.x > right)
        {
            pos.x = right;
        }
        if (pos.y > top)
        {
            pos.y = top;
        }
        else if (pos.y < bottom)
        {
            pos.y = bottom;
        }
        return pos;
    }

    void UpdateMaxOrthoSize()
    {
        float widthSizeLimit = (MapGrid.Size.X * MapGrid.CellSize.x) / (2f * _aspect);
        float heightSizeLimit = (MapGrid.Size.Y * MapGrid.CellSize.y) / 2f;
        MaxSize = Mathf.Min(widthSizeLimit, heightSizeLimit);
    }
        
    void Update ()
    {
        if (_pixelW != _camera.pixelWidth || _pixelH != _camera.pixelHeight)
        {
            _aspect = _camera.pixelWidth / (float)_camera.pixelHeight;
            UpdateMaxOrthoSize();
            UpdateOrthoSize();
            UpdatePos();
            _pixelW = Screen.width;
            _pixelH = Screen.height;
        }

        if (_eventSystem.currentSelectedGameObject) return; 

        if (Input.GetMouseButton(0) && Input.touchCount <= 1)
        {
            _zoom = false;

            if (!_dragged)
            {
                _downPos = Input.mousePosition;
                _deltaAccum = new Vector3();
                _locked = true;
                _dragged = true;
            }
            else
            {
                Vector3 pointerPos = Input.mousePosition;
                Vector3 worldDownPos = _camera.ScreenToWorldPoint(_downPos);
                Vector3 worldPointerPos = _camera.ScreenToWorldPoint(pointerPos);
                Vector3 delta = worldDownPos - worldPointerPos;
                if (_locked)
                {
                    _deltaAccum += delta;
                    if (Mathf.Abs(_deltaAccum.x) + Mathf.Abs(_deltaAccum.y) > LockedDelta)
                    {
                        _locked = false;
                        delta = _deltaAccum;
                    }
                }
                if(!_locked) SetPos(transform.position + delta);
                _downPos = pointerPos;
            }
        } 
        else if(_dragged)
        {
            _dragged = false;
            if (_locked)
            {
                Vector2 worldDownPos = _camera.ScreenToWorldPoint(_downPos);
                Cursor.SetPosition(worldDownPos);
            }
        }
        
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (!_zoom)
            {
                _initialTouch0Position = touch0.position;
                _initialTouch1Position = touch1.position;
                _initialCameraPosition = transform.position;
                _initialOrthographicSize = _camera.orthographicSize;
                _initialMidPointScreen = (touch0.position + touch1.position) / 2;

                _zoom = true;
            }
            else
            {
                transform.position = _initialCameraPosition;
                SetOrthoSize(_initialOrthographicSize);

                float scaleFactor = GetScaleFactor(touch0.position,
                                                   touch1.position,
                                                   _initialTouch0Position,
                                                   _initialTouch1Position);

                Vector2 currentMidPoint = (touch0.position + touch1.position) / 2;
                Vector3 initialMidPointWorldBeforeZoom = GetComponent<Camera>().ScreenToWorldPoint(_initialMidPointScreen);

                SetOrthoSize(_initialOrthographicSize / scaleFactor);

                Vector3 initialMidPointWorldAfterZoom = GetComponent<Camera>().ScreenToWorldPoint(_initialMidPointScreen);
                Vector2 initialMidPointDelta = initialMidPointWorldBeforeZoom - initialMidPointWorldAfterZoom;

                Vector2 oldAndNewMidPointDelta = GetComponent<Camera>().ScreenToWorldPoint(currentMidPoint) -
                                                 GetComponent<Camera>().ScreenToWorldPoint(_initialMidPointScreen);

                Vector3 newPos = _initialCameraPosition;
                newPos.x -= oldAndNewMidPointDelta.x - initialMidPointDelta.x;
                newPos.y -= oldAndNewMidPointDelta.y - initialMidPointDelta.y;

                SetPos(newPos);
            }
        }
        else
        {
            _zoom = false;
        }
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0)
        {
            SetOrthoSize(_camera.orthographicSize + 0.3f);
            SetPos(transform.position);
        }
        if (scroll > 0)
        {
            SetOrthoSize(_camera.orthographicSize - 0.3f);
            SetPos(transform.position);
        }
    }
    
    static bool IsTouching(Touch touch)
    {
        return touch.phase == TouchPhase.Began ||
                touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary;
    }
    
    public static float GetScaleFactor(Vector2 position1, Vector2 position2, Vector2 oldPosition1, Vector2 oldPosition2)
    {
        float distance = Vector2.Distance(position1, position2);
        float oldDistance = Vector2.Distance(oldPosition1, oldPosition2);

        if (oldDistance == 0 || distance == 0)
        {
            return 1.0f;
        }

        return distance / oldDistance;
    }
}
