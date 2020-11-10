public class PlayerLeaderboardData
{
	public string Name { private set; get; }
	public void SetName(string name)
	{
		Name = name;
	}

	public int HighestLevel { private set; get; }
	public void SetHighestLevel(int highestLevel)
	{
		HighestLevel = highestLevel;
	}
}
