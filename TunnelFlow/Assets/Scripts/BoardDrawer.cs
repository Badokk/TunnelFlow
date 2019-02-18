using System;
using UnityEngine;

public class BoardDrawer : MonoBehaviour
{
	GameObject tilePrefab_;
	GameObject[,] tiles_;
	GameObject[,] labels_;
	BoardManager manager_;
	public static BoardDrawer instance_;
	int rows_;
	int columns_;
	bool viewLabels_;

	public BoardDrawer ()
	{
		instance_ = this;
		viewLabels_ = false;
	}

	public void ToggleLabels ()
	{
		if (viewLabels_)
			deleteLabels ();
		else
			createLabels ();
	}

	public static BoardDrawer getInstance () {
		return instance_;
	}

	public void init(int rows, int columns, GameObject prefab)
	{
		tilePrefab_ = prefab;
		tiles_ = new GameObject[rows, columns];
		labels_ = new GameObject[rows, columns];
		rows_ = rows;
		columns_ = columns;
		manager_ = BoardManager.getInstance();

		for (int i = 0; i < tiles_.GetLength (0); i++) {
			for (int j = 0; j < tiles_.GetLength (1); j++) {
				tiles_ [i, j] = Instantiate(
					tilePrefab_,
					new Vector3(i-rows_/2, j-columns_/2, rows_/3),
					Quaternion.identity) as GameObject;
				if (manager_.board [i, j].isSpawner_)
					tiles_ [i, j].transform.Translate (0, 0, -0.15f);
				tiles_ [i, j].AddComponent<TileIdentification>();
				tiles_ [i, j].GetComponent<TileIdentification>().setValues(i, j);
			}
		}
		createLabels ();
	}

	void createLabels()
	{
		for (int i = 0; i < tiles_.GetLength (0); i++) {
			for (int j = 0; j < tiles_.GetLength (1); j++) {
				labels_ [i, j] = new GameObject();
				labels_ [i, j].transform.SetParent (tiles_ [i, j].transform, false);
				labels_ [i, j].transform.Translate(0, 0, -0.5f);
				labels_ [i, j].AddComponent<TextMesh> ();
				labels_ [i, j].GetComponent<TextMesh>().color = Color.black;
				labels_ [i, j].GetComponent<TextMesh>().characterSize = 0.2f;
			}
		}
		viewLabels_ = true;
	}

	void deleteLabels()
	{
		viewLabels_ = false;
		for (int i = 0; i < tiles_.GetLength (0); i++) {
			for (int j = 0; j < tiles_.GetLength (1); j++) {
				Destroy (labels_ [i, j]);
			}
		}
	}

	public void update(Tile[,] tiles)
	{
		for (int i = 0; i < tiles_.GetLength (0); i++) {
			for (int j = 0; j < tiles_.GetLength (1); j++) {
				float limit = tiles[i, j].limit_;
				float volume = tiles[i, j].volume_;
				float player = tiles[i, j].player_;

				if (limit == 0)
					tiles_ [i, j].GetComponent<MeshRenderer> ().material.color = Color.black;

				if (player == 1)
					tiles_ [i, j].GetComponent<MeshRenderer> ().material.color = new Color(1, 1 - (float)volume/limit, 1 - (float)volume/limit);
				if (player == 2)
					tiles_ [i, j].GetComponent<MeshRenderer> ().material.color = new Color(1 - (float)volume/limit, 1, 1 - (float)volume/limit);
				if (player == 3)
					tiles_ [i, j].GetComponent<MeshRenderer> ().material.color = new Color(1 - (float)volume/limit, 1 - (float)volume/limit, 1);
				if (player == 4)
					tiles_ [i, j].GetComponent<MeshRenderer> ().material.color = new Color(1 - (float)volume/limit, 1, 1);
				if (player == 5)
					tiles_ [i, j].GetComponent<MeshRenderer> ().material.color = new Color(1, 1 - (float)volume/limit, 1);
				if (player == 6)
					tiles_ [i, j].GetComponent<MeshRenderer> ().material.color = new Color(1, 1, 1 - (float)volume/limit);

				if (viewLabels_) labels_[i, j].GetComponent<TextMesh>().text = "" + volume;
			}
		}
	}

	public void teardown()
	{
		for (int i = 0; i < tiles_.GetLength (0); i++) {
			for (int j = 0; j < tiles_.GetLength (1); j++) {
				Destroy(tiles_ [i, j]);
				Destroy(labels_ [i, j]);
			}
		}
	}
}

