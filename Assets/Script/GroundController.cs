using UnityEngine;

public class GroundController : MonoBehaviour
{
    [SerializeField] private Vector3 _groundSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _groundSpeed * Time.deltaTime;
    }
}
