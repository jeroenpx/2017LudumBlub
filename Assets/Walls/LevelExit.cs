using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour {

    [SerializeField]
    private string nextLevelName;

	[SerializeField]
	private bool doStartGame = false;

	[SerializeField]
	private bool doEndGame = false;

	[SerializeField]
	private bool isForTimeLapse = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (nextLevelName != null && collision.gameObject.name.StartsWith("octopus"))
        {
			MessagePortalActive();
        }
    }

	private void MessagePortalActive(){
		if (doStartGame) {
			ScoreKeeper.StartGame ();
		}
		ScoreKeeper.BeforeNextLevel ();
		if (isForTimeLapse) {
			ScoreKeeper.MakeTimeLapse ();
		}
		if (doEndGame) {
			ScoreKeeper.EndGame ();
		}
		SceneManager.LoadScene(nextLevelName);
	}
}
