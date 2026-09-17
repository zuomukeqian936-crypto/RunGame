using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _roadPrefab;
    [SerializeField] private Vector3 _spawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            Instantiate(_roadPrefab, _spawnPosition, Quaternion.identity);
        }
    }

}
