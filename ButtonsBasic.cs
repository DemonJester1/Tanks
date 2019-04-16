using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsBasic : MonoBehaviour {

	public void MainMenu_Play ()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void MainMenu_ExitGame ()
    {
        Application.Quit();
    }
}
