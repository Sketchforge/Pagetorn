using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    
    public void SetActive(bool active) => _canvas.enabled = active;
}
