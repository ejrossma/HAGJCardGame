using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayScene : MonoBehaviour
{
    public void loadPlay() {
        SceneManager.LoadScene("PlayScene", LoadSceneMode.Single);
    }
}
