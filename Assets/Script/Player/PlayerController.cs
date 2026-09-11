using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed = 5f;       // 前進スピード
    [SerializeField] private float _laneChangeSpeed = 15f;   // レーン移動の速さ
    [SerializeField] private float[] _laneXPositions = { -8f, -5f, -2f }; // 左, 真ん中, 右のX座標

    private Rigidbody _rigidbody;
    private PlayerPosition _currentPosition = PlayerPosition.Middle;
    private bool _isPressed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();    
    }

    void Update()
    {
        HandleLaneInput();
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
            if (_currentPosition > PlayerPosition.Left)
            {
                _currentPosition--;
                _isPressed = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 右に移動できる場合（Left -> Middle、Middle -> Right）
            if (_currentPosition < PlayerPosition.Right)
            {
                _currentPosition++;
                _isPressed = true;
            }
        }
    }

    /// <summary>
    /// プレイヤー前進処理
    /// </summary>
    private void MovePlayer()
    {
        if(_isPressed == true)
        {
            // 1. 目標のX座標をenumのインデックスから取得
            float targetX = _laneXPositions[(int)_currentPosition];

            // 2. 現在のX座標から目標のX座標へ滑らかに補間移動
            float currentX = _rigidbody.position.x;
            float newX = Mathf.MoveTowards(currentX, targetX, _laneChangeSpeed * Time.fixedDeltaTime);

            // 3. 前進（Z方向）とレーン移動（X方向）を合わせた移動ベクトルを作成
            Vector3 moveVelocity = new Vector3((newX - currentX) / Time.fixedDeltaTime, _rigidbody.linearVelocity.y, _forwardSpeed);

            _rigidbody.linearVelocity = moveVelocity;

            if(Mathf.Approximately(targetX,newX))
            {
                ResetPress();
            }
        }
        else
        {
            _rigidbody.linearVelocity = new Vector3(0f, 0f, _forwardSpeed);
        }   
    }

    /// <summary>
    /// スピード加算処理
    /// </summary>
    /// <param name="applyForce">加算率</param>
    public void ApplyForce(float applyForce = 1.1f)
    {
        _forwardSpeed *= applyForce;
    }

    /// <summary>
    /// 方向移動完了処理
    /// </summary>
    private void ResetPress()
    {
        _isPressed = false;
    }
}
