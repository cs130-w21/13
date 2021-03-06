/**
 * This file just starts the server in server.ts.
 */

const { http, io } = require("./src/server");

/******************************************************************************/

/**
 * Listen on port 3000
 */
const port = process.env.PORT || 3000;
http.listen(port, () => {
  console.log('Connected at ' + port);
}); 

export {}  // suppress TS import checker
