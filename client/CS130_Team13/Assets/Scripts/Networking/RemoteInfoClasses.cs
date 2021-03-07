
[System.Serializable]
public class UserInfo
{
    public string name;
    public int id;
    public int playerNumber;
    public double randomSeed;

    public UserInfo(string name, int id)
    {
        this.name = name;
        this.id = id;
    }
}

[System.Serializable]
public class TurnInfo
{
    public string commands;
    public int id;

    public TurnInfo(string commands, int id)
    {
        this.commands = commands;
        this.id = id;
    }
}