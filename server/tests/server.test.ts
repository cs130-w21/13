/**
 * This is the test file corresponding to ../server.ts.
 */

const { http, io } = require("../server.ts");
const Client = require("socket.io-client");
const assert = require("chai").assert;

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
  });

  /**
   * Tests whether two clients can connect to the server.
   */
  it("hello-test-two-clients", (done) => {
    clientSocket1.emit('hello', '{"name":"client1","id":1,"playerOrder":0}');
    clientSocket1.on('pairing', (msg) => {
      assert.equal(msg, '1');
      clientSocket2.emit('hello', '{"name":"client2","id":2,"playerOrder":0}');
      clientSocket2.on('pairing', (msg) => {
        assert.equal(msg, '2');
        done();
      });
    });
  });

  /**
   * Tests whether two clients can be paired
   */
  it("hello-test-pair-two-clients", (done) => {
    clientSocket1.emit('hello', '{"name":"client1","id":1,"playerOrder":0}');
    clientSocket1.on('pairing', (msg) => { });
    clientSocket2.emit('hello', '{"name":"client2","id":2,"playerOrder":0}');
    clientSocket2.on('pairing', (msg) => { });
    clientSocket1.on('gameplay', (msg) => {
      assert.equal(msg.name, "client2");
      assert.equal(msg.id, 2);
      assert.equal(msg.commands, "null");
    });
    clientSocket2.on('gameplay', (msg) => {
      assert.equal(msg.name, "client1");
      assert.equal(msg.id, 1);
      assert.equal(msg.commands, "null");
    });
    done();
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
  });

  /**
   * Tests whether server sends moves after both clients send their moves
   */
  it("gameplay-test-send-info", (done) => {
    clientSocket1.emit('submittingTurn', '{"id":1,"commands":"LELELEAP"}');
    clientSocket2.emit('submittingTurn', '{"id":2,"commands":"DUE VLA"}');
    clientSocket1.on('receiveTurn', (msg) => {
      assert.equal(msg, '{"id":2,"commands":"DUE VLA"}');
      clientSocket2.on('receiveTurn', (msg) => {
        assert.equal(msg, '{"id":1,"commands":"LELELEAP"}');
        done();
      });
    });
    done();
  });

  /**
 * Tests whether server sends new moves after both clients send their moves
 */
  it("gameplay-test-send-multiple-info", (done) => {
    clientSocket1.emit('submittingTurn', '{"id":1,"commands":"LELELEAP"}');
    clientSocket2.emit('submittingTurn', '{"id":2,"commands":"DUE VLA"}');
    clientSocket1.emit('submittingTurn', '{"id":1,"commands":"OS"}');
    clientSocket2.emit('submittingTurn', '{"id":2,"commands":"MEOW"}');
    clientSocket1.on('receiveTurn', (msg) => {
      assert.equal(msg, '{"id":2,"commands":"MEOW"}');
      clientSocket2.on('receiveTurn', (msg) => {
        assert.equal(msg, '{"id":1,"commands":"OS"}');
        done();
      });
    });
    done();
  });

  /**
* Test end game
*/
  it("gameplay-game-end", (done) => {
    clientSocket1.emit('submittingTurn', '{"id":1,"commands":"LELELEAP"}');
    clientSocket2.emit('submittingTurn', '{"id":2,"commands":"DUE VLA"}');
    clientSocket1.emit('submittingTurn', '{"id":1,"commands":"OS"}');
    clientSocket2.emit('submittingTurn', '{"id":2,"commands":"MEOW"}');
    clientSocket1.on('receiveTurn', (msg) => {
      assert.equal(msg, '{"id":2,"commands":"MEOW"}');
      clientSocket2.on('receiveTurn', (msg) => {
        assert.equal(msg, '{"id":1,"commands":"OS"}');
        done();
      });
    });
    done();
  });
});

/******************************************************************************/
export { }  // suppress TS import checker