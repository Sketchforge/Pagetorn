using UnityEngine.SceneManagement;

public class WinGame : PlayerInteractable
{
    public override void Interact()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
