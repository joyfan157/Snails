namespace Snails.Systems;

public class ScoreManager
{
    public int Score { get; private set; }

    public void AddScore(int points)
    {
        Score += points;
    }
}
