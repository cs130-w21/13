/**
 * This is the main file for the hacman server. Currently, the server is designed
 * mostly to facilitate communication between the clients (ie. not to enforce
 * game state or to manage client actions, etc.), so see the Unity side of the
 * project for the actual game logic.
 */

const app = require('express')();
const http = require('http').createServer(app);
const io = require('socket.io')(http);
import UserInfo, { SocketTurnInfo, ClientSentUserInfo } from './UserInfoClasses';

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

let user1: UserInfo;
let user2: UserInfo;

/******************************************************************************/

/**
 * Server waits for both players to connect, and sends a "gameplay" event to both
 *  once 2 players have connected
 * Additional players will receive the error event "initiate"
 */
io.on('connection', (socket) => {
  console.log('a user connected');

  // The "hello" event is how a user initially connects to the server
  socket.on('hello', (data: string) => {
    let msg: ClientSentUserInfo = JSON.parse(data);
    console.log(msg);
    if (!msg.isInvalid) {
      // if user is first user, setup
      if (user1 === undefined || user1.id === msg.id) {
        user1 = new UserInfo(msg.name, msg.id, socket.id, 1, Math.random());
        // Returns 1 to indicate player 1
        socket.emit("pairing", 1);

        // if user is second user, setup and allow for starting game by telling clients
      } else if (user2 === undefined || user2.id === msg.id) {
        user2 = new UserInfo(msg.name, msg.id, socket.id, 2, user1.randomSeed);
        // Returns 2 to indicate player 2
        socket.emit("pairing", 2);
        socket.to(user1.socketId).emit("gameplay", user2.exportClientRequiredUserInfo()); // sends message to specific client based on clientId
        socket.emit("gameplay", user1.exportClientRequiredUserInfo());

        // for excess users, cya 
      } else {
        socket.emit("pairing", 'There are already 2 players. Please try again later.');
      }
    } else {
      socket.emit("invalid", "invalid user data")
    }
  });

  /**
  * Receives turn info from users, and sends turn info to other player once both
  *   players have submitted
  * TODO: CHECK IF GAME STARTED IN MULTIGAME
  */
  socket.on('submittingTurn', (data: string) => {
    let msg: SocketTurnInfo = JSON.parse(data);
    if (!msg.isInvalid) {
      // Check which user has submitted and change state accordingly
      if (user1.id == msg.id) {
        user1.commands = msg.commands;
        console.log("Received commands from player 1: " + user1.commands);
      }
      else if (user2.id == msg.id) {
        user2.commands = msg.commands;
        console.log("Received commands from player 2: " + user2.commands);
      }

      // If we've received both sets of commands, send back to players
      if (user1.commands !== null && user2.commands !== null) {
        socket.to(user1.socketId).emit("receiveTurn", user2.exportClientRequiredTurnInfo());
        socket.to(user2.socketId).emit("receiveTurn", user1.exportClientRequiredTurnInfo());
        console.log("Sending commands to both players")
        user1.commands = null;
        user2.commands = null;
      }
    } else {
      socket.emit("invalid", "invalid turn data")
    }
  });

  /**
  * When server receives a end game signal from either client, send both clients
  *   an end game message
  * TODO: CHECK IF GAME STARTED IN MULTIGAME
  */
  socket.on('endGameRequest', () => {
    console.log('Server received an end game request');
    socket.to(user1.socketId).emit("endGameConfirmation", "");
    socket.to(user2.socketId).emit("endGameConfirmation", "");
    // TODO: for multigame, open these back up
    // user1 = undefined;
    // user2 = undefined;
  });

  // this method like socket will fire when user disconnects
  socket.on('disconnect', () => {
    console.log('user disconnected');
  });
});

/******************************************************************************/

module.exports = {
  http: http,
  io: io,
};
