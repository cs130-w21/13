/**
 * This file just starts the server in server.ts.
 */

const { http, io } = require("./server.ts");

/******************************************************************************/

/**
 * Listen on port 3000
 */
const port = process.env.PORT || 3000;
http.listen(port, () => {
  console.log('Connected at ' + port);
}); 

export {}  // suppress TS import checker
