using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadWinLossScene : MonoBehaviour
{
    public void loadResults() {
        SceneManager.LoadScene("WinLossScene", LoadSceneMode.Additive);
    }
}
