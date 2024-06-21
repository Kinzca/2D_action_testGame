public Transform target;//跟随的物体
    public Vector3 offset;//偏移量

    public float sensitivity = 2.0f;
    public float smoothing = 0.5f;

    private Vector2 mouseLook;
    private Vector2 smoothV;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;//隐藏鼠标
    }

    private void Update()
    {
        if (target)
        {
            transform.rotation = target.GetChild(2).rotation;
            transform.position = target.GetChild(2).position + offset;
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }

        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        mouseLook += new Vector2(target.position.x, mouseY);
        mouseLook = Vector2.ClampMagnitude(mouseLook, 90f);
        
        // 平滑视角旋转
        smoothV.y = Mathf.Lerp(smoothV.y, mouseLook.y, smoothing);

        // 应用视角旋转
        transform.rotation = Quaternion.Euler(-smoothV.y, smoothV.x, 0.0f);
    }