using System;
using UnityEngine;

public class TileIdentification : MonoBehaviour
{
	public int x_;
	public int y_;

	public TileIdentification (int x, int y)
	{
		x_ = x;
		y_ = y;
	}

	public void setValues(int x, int y)
	{
		x_ = x;
		y_ = y;
	}
}


