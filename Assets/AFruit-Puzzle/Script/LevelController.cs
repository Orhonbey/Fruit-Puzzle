using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public GameObject level1, levelFail;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("levelKey"))
        {
            if (PlayerPrefs.GetInt("levelKey")==0)
            {
                level1.gameObject.SetActive(true);
                levelFail.gameObject.SetActive(false);
            }
            else
            {
                level1.gameObject.SetActive(false);
                levelFail.gameObject.SetActive(true);
            }
        }
    }
    public void LevelFailOpen()
    {
        PlayerPrefs.SetInt("levelKey", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LevelOneOpen()
    {
        PlayerPrefs.SetInt("levelKey", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
