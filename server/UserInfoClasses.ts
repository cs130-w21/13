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
  playerOrder!: number;
  randomSeed!: number;

  constructor(name: string, id: number, socketId: string, playerOrder: number, randomSeed: number) {
    this.name = name;
    this.id = id;
    this.socketId = socketId;
    this.commands = null;
    this.playerOrder = playerOrder;
    this.randomSeed = randomSeed;
  }
  exportClientRequiredUserInfo() : ClientRequiredUserInfo {
    return new ClientRequiredUserInfo(this.name, this.playerOrder, this.randomSeed);
  }
  exportClientRequiredTurnInfo() : SocketTurnInfo {
    return new SocketTurnInfo(this.id, this.commands);
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
  playerOrder!: number;
  randomSeed!: number;
  constructor(name: string, playerOrder: number, randomSeed: number) {
    this.name = name;
    this.playerOrder = playerOrder;
    this.randomSeed = randomSeed;
  }
}

/******************************************************************************/
// CLASSES RECEIVED FROM CLIENT
export class ClientSentUserInfo {
  name!: string;
  id!: number;
  isInvalid() : boolean {
    return (this.name !== undefined) && (this.id !== undefined);
  }
}

/******************************************************************************/
// CLASSES SENT AND RECEIVED FROM CLIENT

export class SocketTurnInfo {
  id!: number;
  commands!: string | null;
  isInvalid() : boolean {
    return (this.id !== undefined) && (this.commands !== undefined);
  }
  constructor(id: number, commands: string | null) {
    this.id = id;
    this.commands = commands;
  }
}