/**
 * This file just starts the server in server.ts.
 */

const { http, io } = require("./server.ts");

/******************************************************************************/

/**
 * Listen on port 3000
 */
http.listen(3000, () => {
  console.log('Connected at 3000');
});

export {}  // suppress TS import checker
