using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : ScriptableObject
{
	BoardManager manager = BoardManager.getInstance();
	ArrayList fluidsToAdd_ = new ArrayList();
	public int limit_;
	public int volume_ = 0;
	public int player_ = 0;
	public int x_, y_;
	public bool isSpawner_ = false;
	GameObject label_;

	public Tile(int x, int y, int player, int limit, bool isSpawner)
	{
		isSpawner_ = isSpawner;
		limit_ = limit;
		player_ = player;
		x_ = x;
		y_ = y;
	}

	public void addVolumeStart(int player, int volume)
	{
		KeyValuePair<int, int> pair = new KeyValuePair<int, int> (player, volume);
		fluidsToAdd_.Add (pair);
	}

	public void addVolumeFin(bool isEnforced = false)
	{
		if (fluidsToAdd_.Count == 0)
			return;
		Dictionary<int, int> map = new Dictionary<int, int> ();
		for (int i = 0; i < fluidsToAdd_.Count; i++) {
			KeyValuePair<int, int> pair = (KeyValuePair<int, int>)fluidsToAdd_ [i];
			if (!map.ContainsKey (pair.Key)) {
				map.Add (pair.Key, pair.Value);
			}
			else map [pair.Key] += pair.Value;
		}
		int largest = -9999;
		int largestPlayer = 0;
		int secondLargest = -9999;

		foreach (KeyValuePair<int, int> pair in map) {
			if (map [pair.Key] > largest) {
				secondLargest = largest;
				largest = map [pair.Key];
				largestPlayer = pair.Key;
			}
		}
		int player = largestPlayer;
		int volume = secondLargest > 0 ? largest - secondLargest : largest;

		//Debug.Log  ("player: " + player + " value: " + volume);
		if (player_ == player) {
			volume_ += volume;
		}
		else {
			volume_ -= volume;
			if (volume_ < 0) {
				player_ = (player_ == 0) ? player : 0;
				volume_ *= -1;
			}
		}
		if (!isEnforced
			&& volume_ > (int)((double)limit_ *2))
			volume_ = (int)((double)limit_ *2);
		
		fluidsToAdd_.Clear();
	}

	public bool shouldSpread(Tile other)
	{
		return player_ != 0
			&& ((   player_ == other.player_
				&& other.fillFactor () < 1 
				&& fillFactor() > other.fillFactor() * manager.resistance_)
			||  (  player_ != other.player_
				&&	limit_ > 0
				&& other.limit_ > 0
				&& fillFactor () > manager.fillFactorThreshold_
				&& (fillFactor () > other.fillFactor ()
					|| (player_ != other.player_ && other.player_ != 0))));
	}

	public void spread(ArrayList neighbours)
	{
		int volumeLost = 0;
		int neighboursToSpreadTo = 0;
		foreach (Tile t in neighbours) {
			if (shouldSpread (t))
				neighboursToSpreadTo++;
		}

		foreach (Tile t in neighbours) {
			if (shouldSpread(t))
			{
				if (t.player_ == player_) {
					float volumeToMove = 
						Mathf.Min(t.limit_*2 - t.volume_,
							Mathf.Max(0, ((float)volume_ * (fillFactor () - t.fillFactor ()) / 4 / neighboursToSpreadTo )));
					if (volumeToMove <= 0)
						continue;
					t.addVolumeStart (player_, (int) volumeToMove +1);
					volumeLost += (int) volumeToMove +1;
				} else {
					float volumeToMove = Mathf.Max(0, ((float)volume_ * manager.spreadFactor_ / neighboursToSpreadTo));
					if (volumeToMove <= 0)
						continue;
					t.addVolumeStart (player_, (int) volumeToMove +1);
					volumeLost += (int) volumeToMove +1;
				}
			}
		}
		if (volumeLost != 0)
			addVolumeStart (player_, volumeLost*-1);
	}

	public float fillFactor()
	{
		if (limit_ == 0)
			return 2;
		return (float)volume_ / (float)limit_;
	}
}