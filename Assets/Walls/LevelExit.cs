using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour {

    [SerializeField]
    private string nextLevelName;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (nextLevelName != null && collision.gameObject.name.StartsWith("octopus"))
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }
}
