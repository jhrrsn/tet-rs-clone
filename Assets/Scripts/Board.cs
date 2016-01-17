using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

	public int width = 10;
	public int height = 24;
	public GameObject cell;
	public AudioClip[] clips;

	private GameObject[,] board;
	private Transform boardHolder;
	private Scoring score;
	private GameLoop gl;
	private AudioSource boardSFX;

	// Use this for initialization
	void Awake () {
		InitialiseBoard ();
	}

	void Start() {
		score = GetComponent<Scoring> ();
		gl = GetComponent<GameLoop> ();
		boardSFX = GetComponent<AudioSource> ();
	}

	void InitialiseBoard() {
		board = new GameObject[width, height];
		boardHolder = new GameObject ("Board").transform;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				board[i,j] = Instantiate(cell, new Vector2(i,j), Quaternion.identity) as GameObject;
				board[i,j].transform.SetParent(boardHolder);
				board[i,j].SetActive(false);
//				board[i,j].GetComponent<SpriteRenderer>().enabled = false;
			}
		}
	}

	void CheckRows () {
		int completeRows = 0;
		for (int j = 0; j < height; j++) {
			if (board[0,j].activeSelf && IsFull (j)) {
				ClearRow (j);
				completeRows++;
				j--;
			}
		}
		if (completeRows > 0) {
			boardSFX.PlayOneShot(clips[0]);
			score.IncreaseScore(completeRows);
		}
	}

	bool IsFull(int row) {
		for (int i = 0; i < width; i++) {
			if (!board[i,row].activeSelf) {
				return false;
			}
		}
		return true;
	}

	void ClearRow(int row) {
		for (int i = 0; i < width; i++) {
			board [i, row].SetActive (false);
		}
		MoveRowsDown (row + 1);
	}

	void MoveRowsDown (int startRow) {
		for (int j = startRow; j < height; j++) {
			bool emptyRow = true;
			for (int i = 0; i < width; i++) {
				if (board [i, j].activeSelf)
					emptyRow = false;
				board [i, j-1].SetActive (board [i, j].activeSelf);
				if (board [i, j].activeSelf) {
					Color cellColor = board[i,j].GetComponent<SpriteRenderer>().color;
					board[i,j-1].GetComponent<SpriteRenderer>().color = cellColor;
				}
				board [i, j].SetActive (false);
			}
			if (emptyRow) 
				break; 
		}
	}


	public int GetWidth() {
		return width;
	}

	public int GetHeight() {
		return height;
	}

	public bool GetCellState(int x, int y) {
		return board [x, y].activeSelf;
	}

	public void BlockToBoard(Transform t, Color c) {
		foreach (Transform child in t) {
			int x = Mathf.RoundToInt(child.position.x);
			int y = Mathf.RoundToInt(child.position.y);
			if (y > height-5) {
				boardSFX.PlayOneShot(clips[1]);
				gl.GameOver();
				break;
			}
			c.a = 0.8f;
			board[x,y].GetComponent<SpriteRenderer>().color = c;
			board[x,y].SetActive(true);
		}
		score.IncreaseScore(0);
		CheckRows ();
	}
}
