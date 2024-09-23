using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMOve : MonoBehaviour
{
    public void GoOXGame()
    {
        SceneManager.LoadScene("OX_Stage");
    }
    public void GoMatch()
    {
        SceneManager.LoadScene("Scene_K");

    }

    public void GoHome()
    {
        SceneManager.LoadScene("LJSCS");

    }

}
