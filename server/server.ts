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

  constructor(name: string, id: number, socketId: string) {
    this.name = name;
    this.id = id;
    this.socketId = socketId;
  }
}

var user1: UserProps;
var user2: UserProps;

/******************************************************************************/

/**
 * Ready to listen for others (imagine that it is a function for one connection)
 * TODO: switch to socket.id use
 */
io.on('connection', (socket) => {
  console.log('a user connected');

  socket.on('hello', (msg) => { // listening for event `hello`
    console.log("we received a message: " + msg);
    msg = JSON.parse(msg);

    // if user is first user, setup
    if (user1 === undefined || user1.id === msg.id) { 
      user1 = new UserProps(msg.name, msg.id, socket.id);
      socket.emit("initiate", 'you are user 1!');

    // if user is second user, setup and allow for starting game by telling clients
    } else if (user2 === undefined || user2.id === msg.id) {
      user2 = new UserProps(msg.name, msg.id, socket.id);
      socket.emit("initiate", 'you are user 2!');

      socket.emit("gameplay", user1 );
      socket.to(user1.socketId).emit("gameplay", user2 ); // sends message to specific client based on clientId

    // for excess users, cya 
    } else {
      socket.emit("initiate", 'no space for u :( byebye try again later');
    }
  });

  // can have other socket events ons
  socket.on('gameplay', () => {
  })

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
