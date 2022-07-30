using System;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public static CanvasController Singleton;
    
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _hud;
    [SerializeField] private InventoryManager _inventoryManager;
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
        _pauseMenu.SetActive(true);
        _inventoryManager.SetActive(false);
        _hud.SetActive(false);
        PauseMenuOpen = true;
        PlayerManager.Actions.CheckState();
    }

    public void OpenInventory()
    {
        _pauseMenu.SetActive(false);
        _inventoryManager.SetActive(true);
        _hud.SetActive(false);
        InventoryOpen = true;
        PlayerManager.Actions.CheckState();
    }

    public void CloseMenu()
    {
        _pauseMenu.SetActive(false);
        _inventoryManager.SetActive(false);
        _hud.SetActive(true);
        PauseMenuOpen = false;
        InventoryOpen = false;
        PlayerManager.Actions.CheckState();
    }
}