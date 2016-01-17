using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public float moveRate;
	public AudioClip[] clips;

	private Board board;
	private int boardWidth;
	private GameLoop gameLoop;
	private Color blockColor;
	private float nextMove;
	private AudioSource blockSFX;

	void Start () {
		GameObject gc = GameObject.FindGameObjectWithTag ("GameController");
		board = gc.GetComponent<Board> ();
		gameLoop = gc.GetComponent<GameLoop> ();

		boardWidth = board.GetWidth ();

		blockColor = transform.GetChild(0).GetComponent<SpriteRenderer> ().color;

		nextMove = Time.time + moveRate;

		blockSFX = GetComponent<AudioSource> ();
	}

	void Update () {
		if (Time.time > nextMove) {
			float h = Input.GetAxis ("Horizontal");

			Vector2 undoPosition = transform.position;

			if (h > 0) {
				nextMove = Time.time + moveRate;
				Vector2 newPos = transform.position;
				newPos.x += 1;
				transform.position = newPos;
				// If new position is invalid, undo move.
				if (!CheckPositionValid()) {
					transform.position = undoPosition;
				} else {
					blockSFX.PlayOneShot(clips[1], 0.1f);
				}
			} else if (h < 0) {
				nextMove = Time.time + moveRate;
				Vector2 newPos = transform.position;
				newPos.x -= 1;
				transform.position = newPos;
				// If new position is invalid, undo move.
				if (!CheckPositionValid()) {
					transform.position = undoPosition;
				} else {
					blockSFX.PlayOneShot(clips[1], 0.1f);
				}
			}
		}

//		if (Input.GetButtonDown ("Left")) {
//			Vector2 undoPosition = transform.position;
//			Vector2 newPos = transform.position;
//			newPos.x -= 1;
//			transform.position = newPos;
//
//			if (!CheckPositionValid()) {
//				transform.position = undoPosition;
//			} else {
//				blockSFX.PlayOneShot(clips[1], 0.1f);
//			}
//		} else if (Input.GetButtonDown ("Right")) {
//			Vector2 undoPosition = transform.position;
//			Vector2 newPos = transform.position;
//			newPos.x += 1;
//			transform.position = newPos;
//			
//			if (!CheckPositionValid()) {
//				transform.position = undoPosition;
//			} else {
//				blockSFX.PlayOneShot(clips[1], 0.1f);
//			}
//		}

		if (Input.GetButtonDown ("Rotate")) {
			Quaternion undoRotation = transform.rotation;
			transform.Rotate(new Vector3(0, 0, 90));

			// If new position is invalid, undo move.
			if (!CheckPositionValid()) {
				Vector2 undoPosition = transform.position;

				if (transform.position.x < boardWidth/2) {
					Vector2 newPos = transform.position;
					newPos.x += 1;
					transform.position = newPos;
					if (!CheckPositionValid()) {
						transform.rotation = undoRotation;
						transform.position = undoPosition;
					} else {
						blockSFX.PlayOneShot(clips[0]);
					}
				} else {
					Vector2 newPos = transform.position;
					newPos.x -= 1;
					transform.position = newPos;
					if (!CheckPositionValid()) {
						transform.rotation = undoRotation;
						transform.position = undoPosition;
					} else {
						blockSFX.PlayOneShot(clips[0]);
					}
				}
			} else {
				blockSFX.PlayOneShot(clips[0]);
			}
		}
	}

	bool CheckEmptyBelow() {
		foreach (Transform child in transform) {
			int x = Mathf.RoundToInt(child.position.x);
			int y = Mathf.RoundToInt(child.position.y);
			if (y == 0) {
				return false;
			} else if (board.GetCellState(x, y-1)) {
				return false;
			} 
		}
		return true;
	}

	bool CheckPositionValid() {
		foreach (Transform child in transform) {
			int x = Mathf.RoundToInt(child.position.x);
			int y = Mathf.RoundToInt(child.position.y);
			if (x < 0 || x > boardWidth-1) {
				return false;
			} else if (board.GetCellState(x, y)) {
				return false;
			}
		}

		return true;
	}

	public void Fall () {
		if (CheckEmptyBelow ()) {
			Vector2 newPos = transform.position;
			newPos.y -= 1;
			transform.position = newPos;
		} else {
			blockSFX.PlayOneShot(clips[2]);
			board.BlockToBoard(transform, blockColor);
			gameLoop.SpawnBlock();
			foreach (Transform t in transform) {
				t.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			}
			Destroy (gameObject,0.5f);
		}
	}
}
