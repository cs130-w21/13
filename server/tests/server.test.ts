/**
 * This is the test file corresponding to ../server.ts.
 */

const { http, io } = require("../server.ts");
const Client = require("socket.io-client");
const assert = require("chai").assert;

/******************************************************************************/

describe("server-socket-test", () => {
  let clientSocket1, clientSocket2;
  let url = 'http://localhost:3000';

  /**
   * Initialize server, client sockets
   */
  before((done) => {
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
  after(() => {
    io.close();
    clientSocket1.close();
    clientSocket2.close();
  });

  /**
   * Tests whether two clients can connect to the server.
   */
  it("hello-test-two-clients", (done) => {
    clientSocket1.emit('hello', '{"name":"client1","id":1}');
    clientSocket1.on('initiate', (msg) => {
      assert.equal(msg, 'you are user 1!');
      clientSocket2.emit('hello', '{"name":"client2","id":2}');
      clientSocket2.on('initiate', (msg) => {
        assert.equal(msg, 'you are user 2!');
        done();
      });
    });
  });
});

/******************************************************************************/

export {}  // suppress TS import checker
