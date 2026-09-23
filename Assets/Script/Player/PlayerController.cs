using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _laneChangeSpeed = 15f;   // レーン移動の速さ
    [SerializeField] private float[] _laneXPositions = { -8f, -5f, -2f }; // 左, 真ん中, 右のX座標
    [SerializeField] private float _jumpPower = 3f;
    [SerializeField] private float _maxTiltAngle = 15f;      // レーン移動時に体が傾く最大角度

    private Rigidbody _rigidbody;
    private Animator _animator;
    private PlayerPosition _currentPosition = PlayerPosition.Middle;
    private bool _isJump = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>(); // Animatorコンポーネントの取得
    }

    void Update()
    {
        HandleLaneInput();
        HandleJumpInput();
    }

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
            if (_currentPosition > PlayerPosition.Left)
            {
                _currentPosition--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_currentPosition < PlayerPosition.Right)
            {
                _currentPosition++;
            }
        }
    }

    /// <summary>
    /// ジャンプ処理（アニメーション連動）
    /// </summary>
    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isJump)
        {
            _rigidbody.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            _isJump = true;

            // Animatorにジャンプを伝える（トリガー名が "Jump" の場合）
            if (_animator != null)
            {
                _animator.SetTrigger("Jump");
            }

            Debug.Log("ジャンプしました");
        }
    }

    /// <summary>
    /// プレイヤーの移動と傾き処理
    /// </summary>
    private void MovePlayer()
    {
        float targetX = _laneXPositions[(int)_currentPosition];
        float currentX = _rigidbody.position.x;

        // X座標を滑らかに移動
        float newX = Mathf.MoveTowards(currentX, targetX, _laneChangeSpeed * Time.fixedDeltaTime);

        // 速度の設定（Z方向は動かさず、X方向の移動のみ制御）
        Vector3 moveVelocity = new Vector3((newX - currentX) / Time.fixedDeltaTime, _rigidbody.linearVelocity.y, 0f);
        _rigidbody.linearVelocity = moveVelocity;

        // 移動方向（左・右）に応じて体を斜めに傾ける演出
        float xDifference = targetX - currentX;
        float tiltAngle = Mathf.Clamp(-xDifference * _maxTiltAngle, -_maxTiltAngle, _maxTiltAngle);

        // 前方を向いた状態を基準に、Z軸（またはY軸）を中心に傾ける
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, tiltAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
    }

    /// <summary>
    /// 着地処理
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            _isJump = false;

            // 着地時にジャンプフラグやアニメーションを戻す場合ここで処理できます
        }
        //ゲームオーバー用のトリガー
        if (other.gameObject.CompareTag("Snake"))
        {
            if (MainGameManager.Instance != null)
            {
                MainGameManager.Instance.OnSnakeHit();
            }
        }
    }
}
