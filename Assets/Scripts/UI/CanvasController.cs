using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public static CanvasController Singleton;
    
	[SerializeField] private Canvas _pauseMenu;
    [SerializeField] private Canvas _hud;
    [SerializeField] private GameObject _detailsMenu;
    [SerializeField] private TMPro.TMP_Text _nameText;
    [SerializeField] private TMPro.TMP_Text _descriptionText;
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private CraftingManager _craftingManager;
    [SerializeField] private ToolbarManager _toolbarManager;

    public static InventoryManager InventoryManager => Singleton._inventoryManager;
    public static ToolbarManager ToolbarManager => Singleton._toolbarManager;

    public bool PauseMenuOpen { get; private set; }
    public bool InventoryOpen { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        CloseMenu();
    }

    public void OpenPauseMenu()
    {
	    _pauseMenu.enabled = true;
        _inventoryManager.SetActive(false);
        _craftingManager.SetActive(false);
	    _hud.enabled = false;
        PauseMenuOpen = true;
        PlayerManager.Actions.CheckState();
    }

    public void OpenInventory()
    {
	    _pauseMenu.enabled = false;
        _inventoryManager.SetActive(true);
        _craftingManager.SetActive(false);
	    _hud.enabled = false;
        InventoryOpen = true;
        PlayerManager.Actions.CheckState();
    }

    public void OpenDetails(string name, string details)
	{
        _detailsMenu.SetActive(true);
        _nameText.text = name;
        _descriptionText.text = details;

	}
    public void CloseDetails()
	{
        _detailsMenu.SetActive(false);
    }

    public void OpenCrafting()
    {
	    _pauseMenu.enabled = false;
        _inventoryManager.SetActive(true);
        _craftingManager.SetActive(true);
	    _hud.enabled = false;
        InventoryOpen = true;
        PlayerManager.Actions.CheckState();
    }

    public void CloseMenu()
	{
		_pauseMenu.enabled = false;
        _inventoryManager.SetActive(false);
		_craftingManager.SetActive(false);
		_hud.enabled = true;
        PauseMenuOpen = false;
        InventoryOpen = false;
        PlayerManager.Actions.CheckState();
    }
}