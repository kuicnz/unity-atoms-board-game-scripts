using UnityEngine;

public class Player
{
    public string colour;
    public int gridsOwned;
    public Player nextPlayer;
    public Material material;

    public Player(string colour, int gridsOwned, Player nextPlayer)
    {
        this.colour = colour;
        this.gridsOwned = gridsOwned;
        this.nextPlayer = nextPlayer;
    }
}
