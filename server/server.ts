/**
 * This is the main file for the hacman server. Currently, the server is designed
 * mostly to facilitate communication between the clients (ie. not to enforce
 * game state or to manage client actions, etc.), so see the Unity side of the
 * project for the actual game logic.
 */

const app = require('express')();
const http = require('http').createServer(app);
const io = require('socket.io')(http);

/******************************************************************************/

/** global monotonic counter */
var user_id = 0;

/**
 * if client hits GET endpoint "localhost:3000/user_id",
 * then let user have a unique id
 */
app.get('/user_id', (req, res) => {
  user_id++;
  res.end(user_id.toString());
});

/******************************************************************************/

/**
 * Manages the metadata for each client during a game.
 */
class UserProps {
  name!: string;
  id!: number;
  socketId!: string;
  commands!: string;

  constructor(name: string, id: number, socketId: string) {
    this.name = name;
    this.id = id;
    this.socketId = socketId;
    this.commands = "*";
  }
}

let user1: UserProps;
let user2: UserProps;

/******************************************************************************/

/**
 * Server waits for both players to connect, and sends a "gameplay" event to both
 *  once 2 players have connected
 * Additional players will receive the error event "initiate"
 * Ready to listen for others (imagine that it is a function for one connection)
 * When initializing the game, also initialize state
 *  0 : no commands submitted, 1: P1 only submitted, 2: P2 only submitted, 3: both submitted
 */
let current_state;  
io.on('connection', (socket) => {
  console.log('a user connected');

  // The "hello" event is how a user initially connects to the server
  //  msg should contain a userInfo object
  socket.on('hello', (msg) => { // listening for event `hello`
    console.log("we received a message: " + msg);
    msg = JSON.parse(msg);

    // if user is first user, setup
    if (user1 === undefined || user1.id === msg.id) { 
      user1 = new UserProps(msg.name, msg.id, socket.id);
      // Returns 1 to indicate player 1
      socket.emit("initiate", 1);

    // if user is second user, setup and allow for starting game by telling clients
    } else if (user2 === undefined || user2.id === msg.id) {
      user2 = new UserProps(msg.name, msg.id, socket.id);
      // Returns 2 to indicate player 2
      socket.emit("initiate", 2);

      socket.emit("gameplay", user1 );
      socket.to(user1.socketId).emit("gameplay", user2 ); // sends message to specific client based on clientId
      current_state = 0;

    // for excess users, cya 
    } else {
      socket.emit("initiate", 'There are already 2 players. Please try again later.');
    }
  });

  /**
  * Receives turn info from users, and sends turn info to other player once both
  *   players have submitted
  * Recall that for current_state:
  *   0 : no commands submitted, 1: P1 only submitted, 2: P2 only submitted, 3: both submitted
  */
  socket.on('submittingTurn', (msg) => {
    //console.log("Server has received commands from a user");
    msg = JSON.parse(msg);

    // Check which user has submitted and change state accordingly
    if(user1.id == msg.playerId){
      user1.commands = msg.commandString;
      if(current_state == 0 || current_state == 2) {
        current_state++;
      }  
      console.log("Received commands from player 1");
      console.log("Current State: " + current_state);
    }
    else if(user2.id == msg.playerId){
      user2.commands = msg.commandString;
      if(current_state == 0 || current_state == 1) {
        current_state += 2;
      }  
      console.log("Received commands from player 2");
      console.log("Current State: " + current_state);
    }

    // If we've received both sets of commands, send back to players
    if(current_state == 3) {
      socket.to(user1.socketId).emit("receiveTurn", user2.commands);
      socket.to(user2.socketId).emit("receiveTurn", user1.commands);
      console.log("Sending commands to both players")
      current_state = 0;
    }
  });

  /**
  * When server receives a end game signal from either client, send both clients
  *   an end game message
  */
  socket.on('endGameRequest', () => { 
    console.log('Server received an end game request');
    socket.to(user1.socketId).emit("endGameConfirmation", "");
    socket.to(user2.socketId).emit("endGameConfirmation", "");
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
