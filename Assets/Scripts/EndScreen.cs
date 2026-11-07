using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BacktoMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    
}