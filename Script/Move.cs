public class Move
{
    public int x;
    public int y;
    public Move prev;
    public Move post;
    public Player OldOwner;

    public Move(int x, int y, Move prev, Move post, Player OldOwner)
    {
        this.x = x;
        this.y = y;
        this.prev = prev;
        this.post = post;
        this.OldOwner = OldOwner;
    }
}
