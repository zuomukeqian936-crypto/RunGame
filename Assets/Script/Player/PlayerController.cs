using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed = 5f;       // 前進スピード
    [SerializeField] private float _laneChangeSpeed = 15f;   // レーン移動の速さ
    [SerializeField] private float[] _laneXPositions = { -8f, -5f, -2f }; // 左, 真ん中, 右のX座標
    [SerializeField] private float _jumpPower = 3f;

    private Rigidbody _rigidbody;
    private PlayerPosition _currentPosition = PlayerPosition.Middle;
    private Vector3 _playerDirection;
    private bool _isRotate = false;
    private bool _isJump = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();    
    }

    void Update()
    {
        HandleLaneInput();
        HandleJumpInput();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
    }

    /// <summary>
    /// 入力によるレーン切り替え処理
    /// </summary>
    private void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // 左に移動できる場合（Middle -> Left、Right -> Middle）
            if (_currentPosition > PlayerPosition.Left && !_isRotate)
            { 
                _currentPosition--;
                _isRotate = true;
                PlayerRotate();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !_isRotate)
        {
            // 右に移動できる場合（Left -> Middle、Middle -> Right）
            if (_currentPosition < PlayerPosition.Right)
            {
                _currentPosition++;
                _isRotate = true;
                PlayerRotate();
            }
        }
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isJump)
        {
            _rigidbody.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            _isJump = true;
            Debug.Log("ジャンプしました");
        }
    }

    /// <summary>
    /// プレイヤー方向転換処理
    /// </summary>
    private void PlayerRotate()
    {
        // 1. 目標のX座標をenumのインデックスから取得
        float targetX = _laneXPositions[(int)_currentPosition];
        float currentX = _rigidbody.position.x;

        // 2. 現在のX座標から目標のX座標へ滑らかに補間移動
        _playerDirection = new Vector3(0f, targetX - currentX, 0f).normalized;
        transform.Rotate(_playerDirection);
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void MovePlayer()
    {
        // 1. 目標のX座標をenumのインデックスから取得
        float targetX = _laneXPositions[(int)_currentPosition];
        float currentX = _rigidbody.position.x;

        float newX = Mathf.MoveTowards(currentX, targetX, _laneChangeSpeed * Time.fixedDeltaTime);

        // 3. 前進（Z方向）とレーン移動（X方向）を合わせた移動ベクトルを作成
        Vector3 moveVelocity = new Vector3((newX - currentX) / Time.fixedDeltaTime, _rigidbody.linearVelocity.y, _forwardSpeed);

        _rigidbody.linearVelocity = moveVelocity;

        if (Mathf.Abs(targetX - currentX) < 0.1f && _isRotate)
        {
            transform.Rotate(Vector3.zero);
            _isRotate = false;
        }
    }


        //if(_isPressed == true)
        //{
        //    // 1. 目標のX座標をenumのインデックスから取得
        //    float targetX = _laneXPositions[(int)_currentPosition];

        //    // 2. 現在のX座標から目標のX座標へ滑らかに補間移動
        //    float currentX = _rigidbody.position.x;
        //    _playerDirection = new Vector3(0f, currentX - targetX, 0f).normalized;
        //    transform.Rotate(_playerDirection);

        //    float newX = Mathf.MoveTowards(currentX, targetX, _laneChangeSpeed * Time.fixedDeltaTime);

        //    // 3. 前進（Z方向）とレーン移動（X方向）を合わせた移動ベクトルを作成
        //    Vector3 moveVelocity = new Vector3((newX - currentX) / Time.fixedDeltaTime, _rigidbody.linearVelocity.y, _forwardSpeed);

        //    _rigidbody.linearVelocity = moveVelocity;

        //    if(Mathf.Approximately(targetX,newX))
        //    {
        //        ResetPress();
        //    }
        //}
        //else
        //{
        //    if(_playerDirection != Vector3.zero)
        //    {
        //        transform.Rotate(new Vector3(0f, 0f, 0f));
        //    }

        //    _rigidbody.linearVelocity = new Vector3(0f, 0f, _forwardSpeed);
        //}  

    /// <summary>
    /// 着地処理
    /// </summary>
    /// <param name="other">接触したオブジェクト</param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            _isJump = false;
        }
    }
}
