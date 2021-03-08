///// CONSTANTS
// server endpoints
export const IO_CONNECTED_EVENT = 'connection';
export const CLIENT_INITIATE_EVENT = 'hello';
export const SUBMIT_TURN_EVENT = 'submittingTurn';
export const END_GAME_REQUEST_EVENT = 'endGameRequest';
export const SOCKET_DISCONNECT_EVENT = 'disconnect';
// client endpoints
export const PAIRING_EVENT = 'pairing';
export const GAMEPLAY_START_EVENT = 'gameplay';
export const ERROR_EVENT = 'client error';
export const GAME_ENDED_EVENT = 'endGameConfirmation';
export const RECEIVE_TURN_EVENT = 'receiveTurn';
// server response to client
export const PAIRING_RESPONSE = {"response": 'currently pairing'};
export const OPPONENT_DISCONNECTED_RESPONSE = {"response": 'you win! opponent has disconnected'};
export const NO_PARTICULAR_RESPONSE = {};
// error responses
export const INVALID_USER_DATA_ERR = {"error": 'invalid user data'};
export const INVALID_TURN_DATA_ERR = {"error": 'invalid turn data'};
export const COULD_NOT_FIND_USER_IN_GAME_ERR = {"error": 'could not find user in game; have we paired a game for you yet?'};

// timeout intervals
export const CHECK_TIMEOUT_INTERVAL = 60000; // 60 seconds
export const CONNECTION_TIMEOUT_INTERVAL = 45000; // 45 seconds