using UnityEngine;
using System.Collections;

public class GameLoop : MonoBehaviour {

	public GameObject[] tetrominoes;
	public Vector2[] previewPositions;
	public float normalStep;
	public float dropStep;
	
	private Board board;
	private int boardWidth;
	private int boardHeight;
	private Vector2 boardCentre;

	private int nextBlockID;
	private GameObject previewBlock;
	private Block currentBlock;
	private int blockWidth;

	private float nextStep;
	private float timeStep;
	private float difficultyMod;

	private GameObject gameOverScreen;
	private bool gameOver;

	// Use this for initialization
	void Start () {
		gameOverScreen = GameObject.Find ("GameOverPanel");
		gameOverScreen.SetActive (false);
		gameOver = false;

		board = GetComponent<Board> ();
		boardWidth = board.GetWidth ();
		boardHeight = board.GetHeight ();
		boardCentre = new Vector2 (Mathf.FloorToInt (boardWidth / 2), Mathf.FloorToInt (boardHeight - 4));

		timeStep = normalStep;
		nextStep = Time.time + (timeStep/difficultyMod);
		difficultyMod = 1f;

		nextBlockID = Random.Range(0, tetrominoes.Length);
		SpawnBlock ();
	}

	void Update() {
		if (!gameOver && Time.time > nextStep) {
			nextStep = Time.time + (timeStep/difficultyMod);
			currentBlock.Fall();
		}
		if (Input.GetButtonDown ("Drop")) {
			timeStep = dropStep;
			nextStep = Time.time;
		} else if (Input.GetButtonUp ("Drop")) {
			timeStep = normalStep;
		}
		if (gameOver && Input.GetButtonDown("Restart")) {
			Application.LoadLevel(0);
		}
	}

	void PreviewBlock() {
		Destroy (previewBlock);
		previewBlock = Instantiate(tetrominoes[nextBlockID], previewPositions[nextBlockID], Quaternion.identity) as GameObject;
		previewBlock.GetComponent<Block> ().enabled = false;
	}
	
	void SetBlockWidth(Transform t) {
		int maxX = 0;
		foreach (Transform child in t) {
			if (child.transform.position.x > maxX) {
				maxX = (int) child.transform.position.x;
			}
		}
		blockWidth = maxX;
	}	
	
	public void SpawnBlock() {
		GameObject newBlock = Instantiate(tetrominoes[nextBlockID], Vector2.zero, Quaternion.identity) as GameObject;
		SetBlockWidth (newBlock.transform);
		Vector2 newBlockPos = boardCentre;
		newBlockPos.x -= Mathf.Round (blockWidth / 2);
		newBlock.transform.position = newBlockPos;
		currentBlock = newBlock.GetComponent<Block> ();
		timeStep = normalStep;
		nextStep = Time.time + timeStep;
		nextBlockID = Random.Range(0, tetrominoes.Length);
		PreviewBlock ();
	}

	public void IncreaseDifficulty(float i) {
		difficultyMod += i;
		Debug.Log (difficultyMod);
	}

	public float GetDifficulty() {
		return difficultyMod;
	}

	public void GameOver() {
		GameObject.Find ("BGM").GetComponent<AudioSource> ().pitch = 0.4f;
		gameOver = true;
		gameOverScreen.SetActive (true);
	}
}
