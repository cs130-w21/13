using System;
public class UserInfo
{
  public string name;
  public int id;
  public int playerNumber;
  public double randomSeed;
  private string commands;
  public GameManager.GameState state;
  public string commandsUpdated;
  public UserInfo(string name, int id)
  {
    this.name = name;
    this.id = id;
  }
  public ServerRequiredUserConnectionInfo exportConnectToServerRequiredInfo() {
    if (state == GameManager.GameState.AwaitingOpponentCommands) {
      return new ServerRequiredUserConnectionInfo(name, id, commands, commandsUpdated);
    }
    return new ServerRequiredUserConnectionInfo(name, id);
  }
  public void setCommands(string commands) {
    this.commands = commands;
    if (commands == null || commands == "null")
      this.commandsUpdated = null;
    else
      this.commandsUpdated = "date is " + System.DateTime.Now.ToString();
  }
  public void setCommands(string commands, string commandsUpdated) {
    this.commands = commands;
    this.commandsUpdated = commandsUpdated;
  }
  public string getCommands() {
    if (this.commandsUpdated == null)
      return null;
    
    return this.commands;
  }
  public TurnInfo exportTurnInfo() {
    return new TurnInfo(commands, id, commandsUpdated);
  }
}

[System.Serializable]
public class ServerRequiredUserConnectionInfo
{
  public string name;
  public int id;
  public string commands;
  public string commandsUpdated;
  public ServerRequiredUserConnectionInfo(string name, int id) {
    this.name = name; 
    this.id = id;
  }
  public ServerRequiredUserConnectionInfo(string name, int id, string commands, string commandsUpdated) {
    this.name = name; 
    this.id = id;
    this.commands = commands;
    this.commandsUpdated = commandsUpdated;
  }
}

[System.Serializable]
public class TurnInfo
{
  public string commands;
  public int id;
  public string commandsUpdated;
  public TurnInfo(string commands, int id, string commandsUpdated)
  {
    this.commands = commands;
    this.id = id;
    this.commandsUpdated = commandsUpdated;
  }
}