using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float timeTilDestruction = 30f;
    // Start is called before the first frame update
    void Awake()
    {
        Destroy(this, timeTilDestruction);
    }
}
