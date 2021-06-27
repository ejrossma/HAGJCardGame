using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadRulesScene : MonoBehaviour
{
    public void loadRules() {
        SceneManager.LoadScene("RulesScene", LoadSceneMode.Single);
    }
}
