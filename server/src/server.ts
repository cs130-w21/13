/**
 * This is the main file for the hacman server. Currently, the server is designed
 * mostly to facilitate communication between the clients (ie. not to enforce
 * game state or to manage client actions, etc.), so see the Unity side of the
 * project for the actual game logic.
 */

const app = require('express')();
const http = require('http').createServer(app);
const io = require('socket.io')(http);
const MutexPromise = require('mutex-promise'); // in NodeJS, in browser this is not needed

import UserInfo, { SocketTurnInfo, ClientSentUserInfo } from './UserInfoClasses';
import * as CONSTANTS from './ServerConstants';

var mutex = new MutexPromise('very-unique-key');

/******************************************************************************/

/** global monotonic counter */
var user_id = 0;

/**
 * if client hits GET endpoint "localhost:3000/user_id",
 * then let user have a unique id
 */
app.get('/user_id', (_, res) => {
  user_id++;
  res.end(user_id.toString());
});

/******************************************************************************/

// cleanse every minute, check if anyone hasn't pinged in 1 minute
let usersSearchingForGame: UserInfo[] = [];
let idMap: Map<number, UserInfo> = new Map();

/******************************************************************************/
// attempts to add player to game. if already exists, return empty list
// if no other players, return [client] and note waiting for game
// if game found, return [user1, user2]
function attemptAddPlayerToGame(info: ClientSentUserInfo, socketId: string): UserInfo[] {
  if (idMap.get(info.id)) {
    idMap.get(info.id)?.pinged()
    return [];
  }
  if (usersSearchingForGame.length === 0) {
    let temp: UserInfo = new UserInfo(info.name, info.id, socketId, Math.random());
    usersSearchingForGame.push(temp);
    idMap.set(info.id, temp);
    return [temp];
  }
  let user1: UserInfo = usersSearchingForGame.shift()!;
  user1.setOpponentId(info.id);
  let user2 = new UserInfo(info.name, info.id, socketId, user1.randomSeed);
  user2.setOpponentId(user1.id);
  user1.setPlayerNumber(1);
  user2.setPlayerNumber(2);
  idMap.set(user1.id, user1);
  idMap.set(user2.id, user2);
  return [user1, user2];
}

function getGamePlayers(id: number): UserInfo[] {
  let curUser = idMap.get(id);
  if (!curUser || !curUser.opponentId) {
    return [];
  }
  let curOpponent = idMap.get(curUser.opponentId);
  if (!curOpponent) {
    idMap.delete(id);
    return [curUser];
  }
  return [curUser, curOpponent];
}

// io.attach(http, {
//   pingInterval: 300,
//   pingTimeout: 30000,
//   cookie: false
// });

/**
 * Server waits for both players to connect, and sends a "gameplay" event to both
 *  once 2 players have connected
 * Additional players will receive the error event "initiate"
 */
io.on(CONSTANTS.IO_CONNECTED_EVENT, (socket) => {
  console.log('a user connected');

  // The "hello" event is how a user initially connects to the server
  socket.on(CONSTANTS.CLIENT_INITIATE_EVENT, async (data: string) => {

    let clientUserInfo: ClientSentUserInfo = JSON.parse(data);
    console.log(clientUserInfo);
    if (!clientUserInfo.isInvalid) {
      let gamePlayers: UserInfo[] = await mutex.promise()
        .then(function (mutex) {
          mutex.lock();
          let gamePlayers: UserInfo[] = attemptAddPlayerToGame(clientUserInfo, socket.id);
          mutex.unlock();
          return gamePlayers;
        });
      if (gamePlayers.length !== 0) {
        socket.emit(CONSTANTS.PAIRING_EVENT, CONSTANTS.PAIRING_STRING); // CHANGED TODO
      }
      if (gamePlayers.length === 2) {
        console.log("STARTTTT")
        console.log(gamePlayers)

        let user1: UserInfo = gamePlayers[0];
        let user2: UserInfo = gamePlayers[1];
        socket.to(user1.socketId).emit(CONSTANTS.GAMEPLAY_START_EVENT, user2.exportClientRequiredUserInfo());
        socket.emit(CONSTANTS.GAMEPLAY_START_EVENT, user1.exportClientRequiredUserInfo());
      }
    } else {
      socket.emit(CONSTANTS.ERROR_EVENT, CONSTANTS.INVALID_USER_DATA)
    }
  });

  /**
  * Receives turn info from users, and sends turn info to other player once both
  *   players have submitted
  */
  socket.on(CONSTANTS.SUBMIT_TURN_EVENT, async (data: string) => {
    let socketTurnInfo: SocketTurnInfo = JSON.parse(data);
    if (!socketTurnInfo.isInvalid) {
      await mutex.promise()
        .then(function (mutex) {
          mutex.lock();
          let players: UserInfo[] = getGamePlayers(socketTurnInfo.id);
          if (players.length === 0) {
            socket.emit(CONSTANTS.ERROR_EVENT, CONSTANTS.COULD_NOT_FIND_USER_IN_GAME);
          } else if (players.length === 1) {
            socket.emit(CONSTANTS.GAME_ENDED_EVENT, CONSTANTS.OPPONENT_DISCONNECTED);
          } else if (players.length === 2) {
            let player: UserInfo = players[0];
            let opponent: UserInfo = players[1];
            player.commands = socketTurnInfo.commands;
            player.pinged();
            console.log("Received commands from player %s: %s", player.playerNumber, player.commands);

            // If we've received both sets of commands, send back to players
            if (player.commands !== null && opponent.commands !== null) {
              socket.emit(CONSTANTS.RECEIVE_TURN_EVENT, opponent.exportClientRequiredTurnInfo());
              socket.to(opponent.socketId).emit(CONSTANTS.RECEIVE_TURN_EVENT, player.exportClientRequiredTurnInfo());
              console.log("Sending commands to both players")
              player.commands = null;
              opponent.commands = null;
              idMap.set(opponent.id, opponent);
            }
            idMap.set(player.id, player);
          }
          mutex.unlock();
        });
    } else {
      socket.emit(CONSTANTS.ERROR_EVENT, CONSTANTS.INVALID_TURN_DATA)
    }
  });

  /**
  * When server receives a end game signal from either client, send both clients
  *   an end game message
  */
  socket.on(CONSTANTS.END_GAME_REQUEST_EVENT, async (data: string) => {//// TODO FIX CLIENT
    console.log('Server received an end game request');
    let clientUserInfo: ClientSentUserInfo = JSON.parse(data);
    let players: UserInfo[] = await mutex.promise()
      .then(function (mutex) {
        mutex.lock();
        let players: UserInfo[] = getGamePlayers(clientUserInfo.id);
        players.forEach(userInfo => idMap.delete(userInfo.id));
        mutex.unlock();
        return players;
      });
    if (players.length === 0) {
      socket.emit(CONSTANTS.ERROR_EVENT, CONSTANTS.COULD_NOT_FIND_USER_IN_GAME);
    } else if (players.length === 1) {
      socket.emit(CONSTANTS.GAME_ENDED_EVENT, CONSTANTS.OPPONENT_DISCONNECTED);
    } else {
      socket.to(players[0].socketId).emit(CONSTANTS.GAME_ENDED_EVENT, CONSTANTS.NO_PARTICULAR_RESPONSE);
      socket.to(players[1].socketId).emit(CONSTANTS.GAME_ENDED_EVENT, CONSTANTS.NO_PARTICULAR_RESPONSE);
    }
  });

  socket.on(CONSTANTS.SOCKET_DISCONNECT_EVENT, () => {
    console.log('user disconnected');
  });
});

/******************************************************************************/

module.exports = {
  http: http,
  io: io,
};
