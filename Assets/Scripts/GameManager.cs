using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image[] images;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateLives(int lives)
    {
        switch (lives)
        {
            case 0:
                images[0].enabled = false;
                images[1].enabled = false;
                images[2].enabled = false;
                break;
            case 1:
                images[0].enabled = true;
                images[1].enabled = false;
                images[2].enabled = false;
                break;
            case 2:
                images[0].enabled = true;
                images[1].enabled = true;
                images[2].enabled = false;
                break;
            case 3:
                images[0].enabled = true;
                images[1].enabled = true;
                images[2].enabled = true;
                break;
        }
    }
}
