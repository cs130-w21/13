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
    clientSocket1.emit('hello', '{"name":"client1","id":1,"playerOrder":0}');
    clientSocket1.on('initiate', (msg) => {
      console.log("" + msg);
      assert.equal(msg, '1');
      clientSocket2.emit('hello', '{"name":"client2","id":2,"playerOrder":0}');
      clientSocket2.on('initiate', (msg) => {
        console.log("" + msg);
        assert.equal(msg, '2');
        done();
      });
    });
  });
  

  /**
   * Test if messages are sent
   */

  /** 
  it("test-command-sharing", (done) => {
    clientSocket1.emit('hello', '{"name":"client1","id":1,"playerOrder":0}');
    clientSocket1.on('initiate', (msg) => {
      console.log("" + msg);
      assert.equal(msg, '1');
      clientSocket2.emit('hello', '{"name":"client2","id":2,"playerOrder":0}');
      clientSocket2.on('initiate', (msg) => {
        console.log("" + msg);
        assert.equal(msg, '2');
        clientSocket1.emit('submittingTurn', '{"commandString":"FBFB","playerId":1}');
        clientSocket2.emit('submittingTurn', '{"commandString":"LRLR","playerId":2}');
        console.log("sent both turns");
        clientSocket1.on('receiveTurn', (data) => {
          console.log("P1 receiving " + data);
          assert.equal(data, "LRLR");
          clientSocket2.on('receiveTurn', (data) => {
            console.log("P2 receiving " + data);
            assert.equal(data, "FBFB");
            done();
          });
        });
      });
    });
  });
  */
});
  /******************************************************************************/

  export { }  // suppress TS import checker
