using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour
{
    // Name = SceneName in Unity
    [SerializeField] private string normalSceneName = "NormalVersionScene";
    [SerializeField] private string dynamicSceneName = "DynamicNPCScene";
    [SerializeField] private TMP_Text popupText;   // TextMeshPro
    // Iteration global merken
    public static int selectedIteration = 1;

    public void StartNormalVersion()
    {
        SceneManager.LoadScene(normalSceneName);
    }

    // Diese Methode rufen die Iterations-Buttons auf
    public void SelectIteration(int iteration)
    {
        selectedIteration = iteration;
        Debug.Log("Iteration gewählt: " + iteration);
        popupText.text = "Iteration " + iteration + " gewählt!";
        // Text setzen
        popupText.text = "Iteration " + iteration + " gewählt!";
        popupText.gameObject.SetActive(true);

        // Nach 2 Sekunden wieder verstecken
        CancelInvoke(nameof(HidePopup));
        Invoke(nameof(HidePopup), 1f);
    }
    
    private void HidePopup()
    {
        popupText.gameObject.SetActive(false);
    }
    public void StartDynamicVersion()
    {
        SceneManager.LoadScene(dynamicSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}


