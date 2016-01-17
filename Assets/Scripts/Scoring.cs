using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scoring : MonoBehaviour {

	public int baseScore = 2;
	public float difficultyStep;

	private int score;
	private Text scoreText;
	private GameLoop gl;


	// Use this for initialization
	void Start () {
		score = 0;
		scoreText = GameObject.Find ("ScoreText").GetComponent<Text> ();
		gl = GetComponent<GameLoop> ();
	}

	// Update is called once per frame
	public void IncreaseScore (int nRows) {
		float difficulty = gl.GetDifficulty ();
		int nextScore = Mathf.RoundToInt((difficulty - 1f) * 5000f);
		while (score > nextScore) {
			gl.IncreaseDifficulty(difficultyStep);
			difficulty = gl.GetDifficulty ();
			nextScore = Mathf.RoundToInt((difficulty - 1f) * 5000f);
		}

		score += Mathf.RoundToInt((Mathf.Pow (baseScore, nRows)) * 10);
		scoreText.text = score.ToString ();
	}
}
