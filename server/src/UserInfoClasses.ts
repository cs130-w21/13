/******************************************************************************/
// CLASSES SAVED BY SERVER 
/**
 * Manages the metadata for each client during a game
 */
export default class UserInfo {
  name!: string;
  id!: number;
  socketId!: string;
  commands!: string | null;
  playerNumber!: number;
  randomSeed!: number;
  opponentId!: number;
  date!: Date;
  constructor(name: string, id: number, socketId: string, randomSeed: number) {
    this.name = name;
    this.id = id;
    this.socketId = socketId;
    this.commands = null;
    this.randomSeed = randomSeed;
    this.pinged();
  }
  exportClientRequiredUserInfo() : ClientRequiredUserInfo {
    return new ClientRequiredUserInfo(this.name, this.playerNumber, this.randomSeed);
  }
  exportClientRequiredTurnInfo() : SocketTurnInfo {
    return new SocketTurnInfo(this.id, this.commands);
  }
  setPlayerNumber(playerNumber: number) : void {
    this.playerNumber = playerNumber;
  }
  setOpponentId(opponentId: number) : void {
    this.opponentId = opponentId;
  }
  pinged() : void {
    this.date = new Date();
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
  isInvalid() : boolean {
    return !this.name || !this.id;
  }
}

/******************************************************************************/
// CLASSES SENT BY SERVER AND RECEIVED FROM CLIENT

export class SocketTurnInfo {
  id!: number;
  commands!: string | null; // TODO UASDOIA UNDEFINED NULL PICKLES
  isInvalid() : boolean {
    return !this.id || !this.commands;
  }
  constructor(id: number, commands: string | null) {
    this.id = id;
    this.commands = commands;
  }
}