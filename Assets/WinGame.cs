using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : PlayerInteractable
{
    public override void Interact()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
