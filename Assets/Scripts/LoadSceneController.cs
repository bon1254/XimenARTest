using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
    public void LoadSceneGate()
    {
        SceneManager.LoadScene("SceneGate");
    }

    public void LoadSceneWall()
    {
        SceneManager.LoadScene("SceneWall");
    }

    public void LoadSceneTemple()
    {
        SceneManager.LoadScene("SceneTemple");
    }
}
