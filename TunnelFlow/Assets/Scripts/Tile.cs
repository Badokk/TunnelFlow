using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : ScriptableObject
{
	ArrayList fluidsToAdd_ = new ArrayList();
	public int limit_;
	public int volume_ = 0;
	public Player player_;
	public int x_, y_;
	public bool isSpawner_ = false;
	public bool isWall_;
	bool isOriginalSpawner_;
	Player originalPlayer_;
	GameObject label_;

	public Tile(int x, int y, Player player, bool isWall, bool isSpawner)
	{
		//Debug.Log (player.color_);
		isSpawner_ = isSpawner;
		isWall_ = isWall;
		player_ = player;
		limit_ = isWall ?
			0
			: (isSpawner_ ? player_.wellCapacity_ : player_.tileCapacity_);
		originalPlayer_ = player;
		x_ = x;
		y_ = y;
		isOriginalSpawner_ = originalPlayer_ != Player.neutral && isSpawner_;
	}

	public void clear()
	{
		volume_ = 0;
		if (!isOriginalSpawner_)
			player_ = Player.neutral;
		else
			player_ = originalPlayer_;
		if (!isWall_) {
			if (isSpawner_)
				limit_ = player_.wellCapacity_;
			else
				limit_ = player_.tileCapacity_;
		}
	}

	public void addVolumeStart(Player player, int volume)
	{
		KeyValuePair<Player, int> pair = new KeyValuePair<Player, int> (player, volume);
		fluidsToAdd_.Add (pair);
	}

	public void addVolumeFin(bool isEnforced = false)
	{
		if (fluidsToAdd_.Count == 0)
			return;
		Dictionary<Player, int> map = new Dictionary<Player, int> ();
		for (int i = 0; i < fluidsToAdd_.Count; i++) {
			KeyValuePair<Player, int> pair = (KeyValuePair<Player, int>)fluidsToAdd_ [i];
			if (!map.ContainsKey (pair.Key)) {
				map.Add (pair.Key, pair.Value);
			}
			else map [pair.Key] += pair.Value;
		}
		int largest = -99999;
		Player largestPlayer = Player.neutral;
		int secondLargest = -99999;

		foreach (KeyValuePair<Player, int> pair in map) {
			if (map [pair.Key] > largest) {
				secondLargest = largest;
				largest = map [pair.Key];
				largestPlayer = pair.Key;
			}
		}
		Player player = largestPlayer;
		int volume = secondLargest > 0 ? largest - secondLargest : largest;

		//Debug.Log  ("player: " + player + " value: " + volume);
		if (player_ == player) {
			volume_ += volume;
			//Debug.Log ("Adding " + volume + ", total: " + volume_ + " limit: " + limit_);
		}
		else {
			volume_ -= volume;
			if (volume_ < 0) {
				player_ = player;
				limit_ = isSpawner_ ? player.wellCapacity_ : player.tileCapacity_;
				volume_ *= -1;
			}
		}
		if (!isEnforced
			&& volume_ > (int)((double)limit_ *4))
			volume_ = (int)((double)limit_ *4);
		
		fluidsToAdd_.Clear();
	}

	public bool shouldSpread(Tile other)
	{
		return (
			!isWall_
			&& !other.isWall_
			&& (player_ == other.player_
				&& other.fillFactor () < 1 
				&& fillFactor() > other.fillFactor() * player_.resistance_)
			||  (  player_ != other.player_
				&& other.limit_ > 0
				&& fillFactor () > player_.fillFactorThreshold_
				&& (fillFactor () > other.fillFactor ()
					|| (player_ != other.player_ && other.player_ != Player.neutral))));
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
					float volumeToMove = Mathf.Min(volume_/neighboursToSpreadTo/4,
						Mathf.Max(0, ((float)volume_ * (fillFactor () - t.fillFactor ()) / neighboursToSpreadTo )));
					if (volumeToMove <= 0)
						continue;
					t.addVolumeStart (player_, (int) volumeToMove);
					volumeLost += (int) volumeToMove;
				} else {
					float volumeToMove = Mathf.Min(volume_/neighboursToSpreadTo,
						Mathf.Max(0, ((float)volume_ * player_.spreadFactor_ / neighboursToSpreadTo)));
					if (volumeToMove <= 0)
						continue;
					t.addVolumeStart (player_, (int) volumeToMove);
					volumeLost += (int) volumeToMove;
				}
			}
		}
		if (volumeLost != 0)
			addVolumeStart (player_, volumeLost*-1);
		if (volumeLost > volume_) {
			Debug.Log ("Something's fucked up " + volume_ + " " + volumeLost);
		}
	}

	public float fillFactor()
	{
		if (limit_ == 0)
			return 2;
		return (float)volume_ / (float)limit_;
	}
}