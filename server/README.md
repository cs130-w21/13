# How to start up this server
`cd server` ==> make sure that path ends with `13\server`
`npm install`
`npm start`

# Multiplayer Development Instructions
1. In Unity, use [ParallelSync](https://github.com/VeriorPies/ParrelSync/tree/1.4.1) to run two different clients with Debug at once
a. If confused what it does, watch 4 minute video, https://www.youtube.com/watch?v=9r-hwXPJIMo&ab_channel=JasonWeimann, to understand
b. On top bar in Unity, go to ParallelSync > Clones Manager > Add new clone > Open in New Editor

## Server prep
2. run `npm install`
3. run `npm start` everytime you want to reset the game (will be changed in future)
a. In VSCode, you can [have a terminal](https://code.visualstudio.com/docs/editor/integrated-terminal) as part of the screen so that you dont have to have a thousand tabs
4. checkout index.ts for the socket code 
a. (may possibly add other files in future)
5. modify index.ts 
a. (or add new ts files and import them in the index.ts file!)

## Client Prep
6. checkout Asset/Scripts/RemoteController script for client socket info (and change if you'd like)
7. run scene "Sockets" in both Unity's and check out Console for debug info
a. Currently, RemoteController.cs is attached to the RemoteController GameObject

WARNING: when debugging, I highly highly recommend stopping both Unity clients AND the server between each test. Unity networking is kind of wonky + also if you don't stop it, your computer might become super slow (running 2 Unitys) + how server is set up right now.

Note: Unity clients often connect and disconnect (dw it's normal). We can live with it, or we can figure out fixes/optimizations in future.

# Unimportant records (no need to read lawl)
### Server init for typescript server (this server)
followed https://blog.logrocket.com/typescript-with-node-js-and-express/

### Server init for javascript server
`npm install express-generator -g`
`express server`