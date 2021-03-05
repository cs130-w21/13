"use strict";
/**
 * This is the test file corresponding to ../server.ts.
 */
Object.defineProperty(exports, "__esModule", { value: true });
const { http, io } = require("../server.ts");
const Client = require("socket.io-client");
const chai = require("chai");
const assert = require("chai").assert;
// import chaiHttp from 'chai-http';
// chai.use(chaiHttp);
/******************************************************************************/
const url = 'http://localhost:3000';
describe("test-GET-unique-id", () => {
    it('get-consecutive-ids', (done) => {
        chai.request(url)
            .get('/id')
            .end((_, res) => {
            res.should.have.status(200);
            assert(res.body, 1);
            done();
        });
    });
});
describe("test-client-pairing-sockets", () => {
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
        clientSocket1.on('initiate', (msg) => {
            assert.equal(msg, '1');
            clientSocket2.emit('hello', '{"name":"client2","id":2,"playerOrder":0}');
            clientSocket2.on('initiate', (msg) => {
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
        clientSocket1.on('initiate', (msg) => {
            assert.equal(msg, '1');
            clientSocket2.emit('hello', '{"name":"client2","id":2,"playerOrder":0}');
            clientSocket2.on('initiate', (msg) => {
                assert.equal(msg, '2');
                clientSocket1.on('gameplay', (msg) => {
                    assert.equal(msg.name, "client2");
                    assert.equal(msg.id, 2);
                    assert.equal(msg.commands, "*");
                });
                clientSocket2.on('gameplay', (msg) => {
                    assert.equal(msg.name, "client1");
                    assert.equal(msg.id, 1);
                    assert.equal(msg.commands, "*");
                });
                done();
            });
        });
    });
    /**
     * Test if messages are sent
     */
    // it("test-command-sharing", (done) => {
    //   clientSocket1.emit('hello', '{"name":"client1","id":1,"playerOrder":0}');
    //   clientSocket1.on('initiate', (msg) => {
    //     console.log("" + msg);
    //     assert.equal(msg, '1');
    //     clientSocket2.emit('hello', '{"name":"client2","id":2,"playerOrder":0}');
    //     clientSocket2.on('initiate', (msg) => {
    //       console.log("" + msg);
    //       assert.equal(msg, '2');
    //       clientSocket1.emit('submittingTurn', '{"commandString":"FBFB","playerId":1}');
    //       clientSocket2.emit('submittingTurn', '{"commandString":"LRLR","playerId":2}');
    //       console.log("sent both turns");
    //       clientSocket1.on('receiveTurn', (data) => {
    //         console.log("P1 receiving " + data);
    //         assert.equal(data, "LRLR");
    //         clientSocket2.on('receiveTurn', (data) => {
    //           console.log("P2 receiving " + data);
    //           assert.equal(data, "FBFB");
    //           done();
    //         });
    //       });
    //     });
    //   });
    // });
});
