using System;
public class UserInfo
{
  public string name;
  public int id;
  public int playerNumber;
  public double randomSeed;
  public string commands;
  public GameManager.GameState state;
  public string commandsUpdated;
  public UserInfo(string name, int id)
  {
    this.name = name;
    this.id = id;
  }
  public ServerRequiredUserConnectionInfo exportConnectToServerRequiredInfo() {
    if (state == GameManager.GameState.AwaitingOpponentCommands) {
      return new ServerRequiredUserConnectionInfo(name, id, commands);
    }
    return new ServerRequiredUserConnectionInfo(name, id);
  }
  public TurnInfo exportTurnInfo() {
    return new TurnInfo(commands, id);
  }
}

[System.Serializable]
public class ServerRequiredUserConnectionInfo
{
  public string name;
  public int id;
  public string commands;
  public ServerRequiredUserConnectionInfo(string name, int id) {
    this.name = name; 
    this.id = id;
  }
  public ServerRequiredUserConnectionInfo(string name, int id, string commands) {
    this.name = name; 
    this.id = id;
    this.commands = commands;
  }
}

[System.Serializable]
public class TurnInfo
{
  public string commands;
  public int id;
  public string commandsUpdated;
  public TurnInfo(string commands, int id)
  {
    this.commands = commands;
    this.id = id;
  }
}