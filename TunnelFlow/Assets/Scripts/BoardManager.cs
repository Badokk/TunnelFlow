using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {
	[SerializeField]
	[Range(5, 100)]
	public int rows_;
	[SerializeField]
	[Range(5, 100)]
	public int columns_;
	[SerializeField]
	[Range(0, 1)]
	public float spreadFactor_ = 0.06f;
	[SerializeField]
	[Range(0, 2)]
	public float resistance_ = 1.0f;
	[SerializeField]
	[Range(0, 1)]
	public float fillFactorThreshold_;

	[SerializeField]
	[Range(1, 9)]
	public int noPlayers_;
	[SerializeField]
	[Range(1, 100)]
	public int wellsPerPlayer_;
	[SerializeField]
	[Range(0, 500)]
	public int neutralWells_;

	[SerializeField]
	[Range(50, 1000)]
	public int wellCapacity_;
	[SerializeField]
	[Range(50, 1000)]
	public int tileCapacity_;
	[SerializeField]
	[Range(1, 1000)]
	public int wellIncome_;
	[SerializeField]
	[Range(0, 100)]
	public int wallChance_;

	[SerializeField]
	[Range(0, 30)]
	public int turnsBetweenReinforcements_;
	int turnsTillReinforcements_;
	[SerializeField]
	[Range(0.1f, 1)]
	public float turnLength_;
	float update = 0;

	[SerializeField]
	GameObject cubePrefab;
	[SerializeField]
	public Player [] player;
	public Tile[,] board;
	public static BoardManager instance_;
	BoardDrawer drawer;
	bool pause_ = false;

	// Use this for initialization
	void Start () {
		instance_ = this;
		board = new Tile[rows_, columns_];
		FillCubeGrid();
		drawer = BoardDrawer.getInstance ();
		drawer.init (rows_, columns_, cubePrefab);
	}

	public static BoardManager getInstance () {
		return instance_;
	}

	void Update()
	{
		if (Input.GetKeyDown ("p"))
			pause_ = !pause_;
		if (Input.GetKeyDown ("r"))
			Reset ();
		if (Input.GetKeyDown ("s"))
			SoftReset ();
		if (Input.GetKeyDown ("l"))
			drawer.ToggleLabels ();
	}

	void Reset()
	{
		drawer.teardown ();
		board = new Tile[rows_, columns_];
		FillCubeGrid ();
		drawer.init (rows_, columns_, cubePrefab);
	}

	void SoftReset()
	{
		drawer.teardown ();
		for (int i = 0; i < rows_; i++) {
			for (int j = 0; j < columns_; j++) {
				board [i, j].clear();
			}
		}
		drawer.init (rows_, columns_, cubePrefab);
	}

	void FixedUpdate() {
		bool auto = true;
		if (auto && !pause_) {
			//Debug.Log ("Last frame took " + Time.deltaTime);
			update -= Time.deltaTime;
			if (update <= 0) {
				update = turnLength_;
				PassTurn ();
			}
		} else {
			if (Input.GetKeyDown ("space"))
				PassTurn ();
		}
	}

	public void Cleeck(int x, int y)
	{
		//board [x, y].addVolumeStart (2, 1000);
		//board [x, y].addVolumeFin (true);
	}

	void FillCubeGrid () {
		//Random.InitState (0);
		for (int i = 0; i < rows_; i++) {
			for (int j = 0; j < columns_; j++) {
				board [i, j] = new Tile (i, j, Player.neutral, Random.Range(0, 100) < wallChance_, false);
			}
		}

		for (int i = 1; i < noPlayers_ * wellsPerPlayer_ + 1; i++) {
			int x = Random.Range (0, rows_);
			int y = Random.Range (0, columns_);
			if (board[x,y].isSpawner_)
			{
				i--;
				continue;
			}
			board [x, y] = new Tile (x, y, player[(i%noPlayers_)], false, true);
		}
		for (int i = 1; i < neutralWells_ + 1; i++) {
			int x = Random.Range (0, rows_);
			int y = Random.Range (0, columns_);
			if (board[x,y].isSpawner_)
			{
				i--;
				continue;
			}
			board [x, y] = new Tile (x, y, Player.neutral, false, true);
		}
	}

	ArrayList getNeighbours(int x, int y)
	{
		ArrayList result = new ArrayList();

		if (x > 0)
			result.Add (board [x - 1, y]);
		if (x < rows_ - 1)
			result.Add (board [x + 1, y]);
	
		if (y > 0)
			result.Add (board [x, y - 1]);
		if (y < columns_ -1)
			result.Add (board [x, y + 1]);
		
		return result;
	}

	void ReinforceWells()
	{
		for (int p = 0; p < noPlayers_; p++) {
			player [p].turnsTillReinforcement_--;
			if (player [p].turnsTillReinforcement_ < 0) {
				player [p].turnsTillReinforcement_ = player [p].turnsBetweenReinforcements_;
				for (int i = 0; i < rows_; i++) {
					for (int j = 0; j < columns_; j++) {
						if (board [i, j].isSpawner_ && board [i, j].player_ == player[p]) {
							board [i, j].addVolumeStart (board [i, j].player_, player[p].wellIncome_);
						}
					}
				}
			}
		}
		Player.neutral.turnsTillReinforcement_--;
		if (Player.neutral.turnsTillReinforcement_ < 0) {
			Player.neutral.turnsTillReinforcement_ = Player.neutral.turnsBetweenReinforcements_;

			//Debug.Log ("Neutral well increase");
			for (int i = 0; i < rows_; i++) {
				for (int j = 0; j < columns_; j++) {
					if (board [i, j].isSpawner_ && board [i, j].player_ == Player.neutral) {
						board [i, j].addVolumeStart (board [i, j].player_, Player.neutral.wellIncome_);
					}
				}
			}
		}
	}
		

	void PassTurn()
	{
		//Debug.Log ("Pass turn");
		ReinforceWells ();
		for(int i = 0; i < rows_; i++)
		{
			for(int j = 0; j < columns_; j++)
			{
				ArrayList neighbours = getNeighbours (i, j);
				board [i, j].spread (neighbours);
			}
		}
		for(int i = 0; i < rows_; i++)
		{
			for(int j = 0; j < columns_; j++)
			{
				board [i, j].addVolumeFin ();
			}
		}
		drawer.update (board);
	}
}



public class MyCube  {
	public GameObject cube;
}