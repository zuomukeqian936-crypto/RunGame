using UnityEngine;

public class DestroyTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.root.gameObject.CompareTag("Ground"))
        {
            Debug.Log(other.gameObject.transform.root.tag);
            Destroy(other.gameObject.transform.root.gameObject);
            Debug.Log("削除しました");
        }
    }
}
