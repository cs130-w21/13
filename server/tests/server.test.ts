/**
 * This is the test file corresponding to ../server.ts.
 */

import { ClientSentTurnInfo } from "../src/UserInfoClasses";

const { http, io, intervalCheck } = require("../src/server");
const Client = require("socket.io-client");
const assert = require("chai").assert;
const { promisify } = require('util')
const sleep = promisify(setTimeout)

const url = 'http://localhost:3000';

/******************************************************************************/
describe("test-client-pairing-socket-events", () => {
  let clientSocket1, clientSocket2;

  /**
   * Initialize server, client sockets
   */
  beforeEach((done) => {
    http.listen(3000, () => {
      clientSocket1 = new Client(url);
      clientSocket1.on("connect", () => {
        clientSocket2 = new Client(url);
        clientSocket2.on("connect", done);
      });
    });
  });

  /**
   * Teardown sockets
   */
  afterEach(() => {
    io.close();
    clientSocket1.close();
    clientSocket2.close();
    clearInterval(intervalCheck);
  });
  
  /**
   * Tests whether two clients can connect to the server.
   */
  it("hello-test-two-clients", async () => {
    clientSocket1.emit('hello', '{"name":"client1","id":1,"playerNumber":0}');
    await clientSocket1.on('pairing', (msg) => {
      assert.equal(msg, 'currently pairing');
    });
    clientSocket2.emit('hello', '{"name":"client2","id":2,"playerNumber":0}');
    await clientSocket2.on('pairing', (msg) => {
      assert.equal(msg, 'currently pairing');
    });
  });

  //   /**
  //    * Tests whether two clients can be paired
  //    */
    it("hello-test-pair-two-clients", async () => {
      clientSocket1.emit('hello', '{"name":"client1","id":1,"playerNumber":0}');
      clientSocket1.on('pairing', (msg) => { });
      clientSocket2.emit('hello', '{"name":"client2","id":2,"playerNumber":0}');
      clientSocket2.on('pairing', (msg) => { });
      await clientSocket1.on('gameplay', (msg) => {
        assert.equal(msg.name, "client2");
        assert.equal(msg.playerNumber, 2);
        assert.equal(msg.commands, undefined);
      });
      await clientSocket2.on('gameplay', (msg) => {
        assert.equal(msg.name, "client1");
        assert.equal(msg.playerNumber, 1);
        assert.equal(msg.commands, undefined);
      });
    });
});

/******************************************************************************/
describe("test-gameplay-socket-events", () => {
  let clientSocket1, clientSocket2;

  /**
   * Initialize server, client sockets
   */
  beforeEach((done) => {
    http.listen(3000, () => {
      clientSocket1 = new Client(url);
      clientSocket1.on("connect", () => {
        clientSocket2 = new Client(url);
        clientSocket2.on("connect", () => {
          clientSocket1.emit('hello', '{"name":"client1","id":1}');
          clientSocket2.emit('hello', '{"name":"client2","id":2}');
          done();
        });
      });
    });
  });

  /**
   * Teardown sockets
   */
  afterEach(() => {
    io.close();
    clientSocket1.close();
    clientSocket2.close();
    clearInterval(intervalCheck);
  });

  /**
   * Tests whether server sends moves after both clients send their moves
   */
  it("gameplay-test-send-info", async () => {
    const player1Turn = new ClientSentTurnInfo(1, "LELELEAP");
    const player2Turn = new ClientSentTurnInfo(2, "DUE VLA");
    clientSocket1.emit('submittingTurn', JSON.stringify(player1Turn));
    clientSocket2.emit('submittingTurn', JSON.stringify(player2Turn));
    await clientSocket1.on('receiveTurn', (msg) => {
      assert.equal(msg, player1Turn.toString());
    });
    await clientSocket2.on('receiveTurn', (msg) => {
      assert.equal(msg, player2Turn.toString());
    });
  });

  /**
 * Tests whether server sends new moves after both clients send their moves
 */
  it("gameplay-test-send-multiple-info", async () => {
    clientSocket1.emit('submittingTurn', '{"id":1,"commands":"LELELEAP"}');
    clientSocket2.emit('submittingTurn', '{"id":2,"commands":"DUE VLA"}');
    await clientSocket1.on('receiveTurn', () => { });
    await clientSocket2.on('receiveTurn', () => { });
    clientSocket1.emit('submittingTurn', '{"id":1,"commands":"OS"}');
    clientSocket2.emit('submittingTurn', '{"id":2,"commands":"MEOW"}');
    await clientSocket1.on('receiveTurn', (msg) => {
      assert.equal(msg, '{"id":2,"commands":"MEOW"}');
    });
    await clientSocket2.on('receiveTurn', (msg) => {
      assert.equal(msg, '{"id":1,"commands":"OS"}');
    });
  });

  /**
  * Test end game
  */
  it("gameplay-game-end", async () => {
    clientSocket1.emit('submittingTurn', '{"id":1,"commands":"LELELEAP"}');
    clientSocket2.emit('submittingTurn', '{"id":2,"commands":"DUE VLA"}');
    clientSocket1.emit('submittingTurn', '{"id":1,"commands":"OS"}');
    clientSocket2.emit('submittingTurn', '{"id":2,"commands":"MEOW"}');
    clientSocket1.emit('endGameRequest', '{"name":"client1","id":1}');
    clientSocket2.emit('endGameRequest', '{"name":"client2","id":2}');
    await clientSocket1.on('endGameConfirmation', (msg) => {
      assert.equal(msg, '');
    });
    await clientSocket2.on('endGameConfirmation', (msg) => {
      assert.equal(msg, '');
    });
  });
});

/******************************************************************************/
export { }  // suppress TS import checker