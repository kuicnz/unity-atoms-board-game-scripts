using UnityEngine;

public class Tile
{
    public Player Owner;
    public int atomCount;
    public GameObject grid;

    public Tile(Player Owner, int atomCount)
    {
        this.Owner = Owner;
        this.atomCount = atomCount;
    }
}
