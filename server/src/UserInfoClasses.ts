/******************************************************************************/
// CLASSES SAVED BY SERVER 
/**
 * Manages the metadata for each client during a game
 */
export default class UserInfo {
  name!: string;
  id!: number;
  socketId!: string;
  private commands!: string | null;
  playerNumber!: number;
  randomSeed!: number;
  opponentId!: number;
  commandsUpdated!: string;
  closed!: boolean;
  closeTime!: Date | null;
  constructor(name: string, id: number, socketId: string, randomSeed: number) {
    this.name = name;
    this.id = id;
    this.socketId = socketId;
    this.commands = null;
    this.randomSeed = randomSeed;
    this.closed = false;
  }
  exportClientRequiredUserInfo(): ClientRequiredUserInfo {
    return new ClientRequiredUserInfo(this.name, this.playerNumber, this.randomSeed);
  }
  exportClientRequiredTurnInfo(): TurnInfo {
    return new TurnInfo(this.id, this.commands!, this.commandsUpdated);
  }
  setPlayerNumber(playerNumber: number): void {
    this.playerNumber = playerNumber;
  }
  setOpponentId(opponentId: number): void {
    this.opponentId = opponentId;
  }
  setCommands(commands: string, commandsUpdated: string): void {
    this.commands = commands;
    this.commandsUpdated = commandsUpdated;
  }
  close(): void {
    this.closed = true;
    this.closeTime = new Date();
  }
  open(): void {
    this.closed = false;
    this.closeTime = null;
  }
}

/******************************************************************************/
// CLASSES SENT BY SERVER 
/**
 * User info that is relevant to the client. 
 * SENT on game start
 */
export class ClientRequiredUserInfo {
  name!: string;
  playerNumber!: number;
  randomSeed!: number;
  constructor(name: string, playerNumber: number, randomSeed: number) {
    this.name = name;
    this.playerNumber = playerNumber;
    this.randomSeed = randomSeed;
  }
}

/******************************************************************************/
// CLASSES RECEIVED FROM CLIENT
export class ClientSentUserInfo {
  name!: string;
  id!: number;
  isInvalid(): boolean {
    return !this.name || !this.id;
  }
}

/******************************************************************************/
// TWO WAY CLASSES
export class TurnInfo {
  id!: number;
  commands!: string;
  commandsUpdated!: string;
  constructor(id: number, commands: string, commandsUpdated: string) {
    this.id = id;
    this.commands = commands;
    this.commandsUpdated = commandsUpdated;
  }
  isInvalid(): boolean {
    return !this.id || !this.commands || !this.commandsUpdated;
  }
}