using System;
using UnityEngine;

[System.Serializable]
public class Player
{
	public static Player [] players;
	public static Player neutral = new Player (0.5f, 1.6f, 0.3f, 10, 50, 75, 5, Color.gray);

	public float spreadFactor_;
	public float resistance_;
	public float fillFactorThreshold_;
	public int turnsBetweenReinforcements_;
	public int turnsTillReinforcement_;
	public int wellCapacity_;
	public int tileCapacity_;
	public int wellIncome_;
	public Color color_;

	public Player ()
	{
		spreadFactor_ = 0.2f;
		resistance_ = 1;
		fillFactorThreshold_ = 0.1f;
		turnsBetweenReinforcements_ = 5;
		wellCapacity_ = 150;
		tileCapacity_ = 100;
		wellIncome_ = 30;
		color_ = Color.gray;
	}
	public Player (
		float spreadFactor,
		float resistance,
		float fillFactorThreshold,
		int turnsBetweenReinforcements,
		int wellCapacity,
		int tileCapacity,
		int wellIncome,
		Color color)
	{
		spreadFactor_ = spreadFactor;
		resistance_ = resistance;
		fillFactorThreshold_ = fillFactorThreshold;
		turnsBetweenReinforcements_ = turnsBetweenReinforcements;
		wellCapacity_ = wellCapacity;
		tileCapacity_ = tileCapacity;
		wellIncome_ = wellIncome;
		color_ = color;
	}

	//public Player[] init()
	//{}
}

