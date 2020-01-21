// Import package
var mongodb = require('mongodb');
var socketio = require('socket.io');
//var ObjectID = mongodb.ObjectID;
var express = require('express')

// Create Express Service
var app = express();

// Create MongoDB Client
var MongoClient = mongodb.MongoClient;

// Connection URL
var url = 'mongodb://localhost:27017'

var nicknames = [];     // List of client nicknames
var ids = [];           // List of client id
var clients = [];       // List of client socket id
//var gamerooms = [{"game_id": 1,"game_name" : "game1","players": [1,2]},{"game_id": 2,"game_name" : "game2","players": [1]},{"game_id": 3,"game_name" : "game3","players": [1]}];
var gamerooms = [];

var gameroomId = 0;
var clientId = 0;

MongoClient.connect(url, { useUnifiedTopology: true }, function (err, client) {
    if (err) console.log('Unable to connection to the mongoDB server.Error', err);
    else {
        // Start Web Server
        var server = app.listen(80, () => {
            console.log('Connected to MongoDB Server , WebService running on port 80');
        })

        // Set Socket Server
        var io = socketio.listen(server);

        io.sockets.on('connection', function (socket) {
            console.log('Socket ID : ' + socket.id + ', Connect');
            socket.emit('connected');
            //clients.push(socket.id);

            socket.on('login', function (data) {
                clientId++;
                console.log('user ' + data.userName + ' with ID ' + clientId + ' and socket id ' + socket.id + ' connected to server');
                //ids.push(clientId.toString());
                //clients.push(socket.id);
                //nicknames.push(data.userName);
                socket.emit('res_login', { 'client_id': clientId });
            });

            socket.on('update_sid', function (data) {
                ids.push(data.client_id);
                clients.push(socket.id);
                nicknames.push(data.nickname);
            });

            socket.on('disconnect', function () {
                console.log('Socket ID : ' + socket.id + ', Disconnect')

                for (i = 0; i < clients.length; i++) {
                    if (clients[i] === socket.id) {
                        clients.splice(i, 1);
                        ids.splice(i, 1);
                        nicknames.splice(i, 1);
                        break;
                    }
                }
                console.log('current client list: ' + clients)

            })

            socket.on('rm_list', function (data) {
                console.log('Socket ID : ' + socket.id + ', removed')
                for (i = 0; i < clients.length; i++) {
                    if (clients[i] === socket.id) {
                        clients.splice(i, 1);
                        ids.splice(i, 1);
                        nicknames.splice(i, 1);
                        break;
                    }
                }
                socket.emit('removed', data);
                console.log('current client list: ' + clients)
            })

            socket.on('create_game', function (data) {
                gameroomId += 1;
                room = {
                    "game_id": gameroomId,
                    "game_name": data.game_name,
                    "players": [data.client_id],
                    "ready": 0
                }
                console.log('game ' + data.game_name + ' with id ' + gameroomId + ' created.');
                gamerooms.push(room);
                socket.emit('confirm_join', { 'game_id': gameroomId });
            });

            socket.on('remove_game', function (data) {
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        for (j = 0; j < gamerooms[i].players.length; j++) {
                            console.log("abc");
                            console.log(ids.indexOf(gamerooms[i].players[j]));
                            console.log(clients[ids.indexOf(gamerooms[i].players[j])]);
                            io.to(clients[ids.indexOf(gamerooms[i].players[j])]).emit('exit');
                        }
                        gamerooms.splice(i, 1);
                    }
                }
            });

            socket.on('join_game', function (data) {
                console.log('join_game called with game_id ' + data.game_id + ' and client_id ' + data.client_id);
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        if (gamerooms[i].players.length == 2) {
                            console.log("gameroom " + gamerooms[i].game_id + 'is already full.');
                            socket.emit('game_full');
                            break;
                        }

                        // Send data to other clients
                        for (j = 0; j < gamerooms[i].players.length; j++) {
                            console.log("send signal");
                            sendData = {
                                'name': nicknames[ids.indexOf(data.client_id)]
                            };
                            console.log("|" + data.client_id + "|");
                            console.log("|" + ids[1] + "|");
                            console.log(ids);
                            console.log(nicknames[ids.indexOf(data.client_id)]);
                            io.to(clients[ids.indexOf(gamerooms[i].players[j])]).emit('opponent_join', sendData);
                        }

                        gamerooms[i].players.push(data.client_id);
                        console.log('player ' + data.client_id + " joined game " + data.game_id);
                        console.log('current players: ' + gamerooms[i].players);

                        names = [];

                        for (j = 0; j < gamerooms[i].players.length; j++) {
                            ind = clients.indexOf(gamerooms[i].players[j]);
                            names.push(nicknames[ind]);
                        }

                        sendData = {
                            'game_id': gamerooms[i].game_id,
                            'nicknames': names
                        };
                        socket.emit('confirm_join', sendData);
                        break;
                    }
                    else {
                        console.log('game_id is different (' + gamerooms[i].game_id + '!=' + data.game_id + ')');
                        console.log(typeof (gamerooms[i].game_id));
                        console.log(typeof (data.game_id));
                    }
                }
            });

            socket.on('exit_game', function (data) {
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        var players = gamerooms[i].players;
                        players.splice(players.indexOf(data.client_id), 1);
                        for (j = 0; j < players.length; j++) {
                            io.to(clients[players.indexOf(players[j])]).emit('opponent_exit');
                        }
                    }
                }
                socket.emit('exit');
            });

            socket.on('reqGamerooms', function (data) {
                console.log('reqGamerooms called');
                resList = [];

                for (i = 0; i < gamerooms.length; i++) {
                    resList.push({
                        "game_id": gamerooms[i].game_id,
                        "game_name": gamerooms[i].game_name,
                        "player_num": gamerooms[i].players.length
                    });
                }
                socket.emit('sendGamerooms', { 'res': resList });
            });

            socket.on('require_game_info', function (data) {
                console.log('require_game_info called');
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        player_names = [];
                        console.log(gamerooms[i].players);
                        for (j = 0; j < gamerooms[i].players.length; j++) {
                            console.log(gamerooms[i].players[j]);
                            console.log(typeof (gamerooms[i].players[j]));
                            player_names.push(nicknames[ids.indexOf(gamerooms[i].players[j])]);
                        }
                        console.log(player_names);
                        socket.emit('res_game_info', {
                            "game_id": gamerooms[i].game_id,
                            "game_name": gamerooms[i].game_name,
                            "player_names": player_names
                        });
                        break;
                    }
                }
            });

            socket.on('start_game', function (data) {
                console.log("Game " + data.game_id + " started");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        for (j = 0; j < gamerooms[i].players.length; j++) {
                            io.to(clients[ids.indexOf(gamerooms[i].players[j])]).emit('start_game');
                        }
                        break;
                    }
                }
            });

            socket.on('game_ready', function (data) {
                console.log("Game " + data.game_id + ": player " + data.playerId + " moved by (" + data.x + "," + data.y + "," + data.z + ") and rotated by (0," + data.ry + ",0)");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        gamerooms[i].ready++;
                        console.log(gamerooms[i].ready + " people are ready in game " + gamerooms[i].game_id);

                        for (j = 0; j < gamerooms[i].players.length; j++) {
                            io.to(clients[ids.indexOf(gamerooms[i].players[j])]).emit('game_ready', { 'ready': gamerooms[i].ready });
                        }
                        break;
                    }
                }
            });

            socket.on('spawn_chickens', function (data) {
                console.log("Game " + data.game_id + ": spawn chickens");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit spawn_chicken to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('spawn_chickens', data);
                        }
                    }
                }
            });

            socket.on('respawn_chickens', function (data) {
                console.log("Game " + data.game_id + ": respawn chickens");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit respawn_chicken to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('respawn_chickens', data);
                        }
                    }
                }
            });

            socket.on('move', function (data) {
                //console.log("Game "+data.game_id+": player "+data.playerId+" moved by ("+data.x+","+data.y+","+data.z+") and rotated by (0,"+data.ry+",0)");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            //console.log('emit move to player '+players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('move', data);
                        }
                    }
                }
            });

            socket.on('fix_position', function (data) {
                //console.log("Game "+data.game_id+": player "+data.playerId+" is in position ("+data.x+","+data.y+","+data.z+")"+" and EulerAngle (0,"+data.ry+",0)");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            //console.log('emit player_idle to player '+players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('fix_position', data);
                        }
                    }
                }
            });

            socket.on('attack', function (data) {
                console.log("Game " + data.game_id + ": player " + data.playerId + " attacked.");
                console.log("" + data.x + " " + data.y + " " + data.z + " " + data.vx + " " + data.vy + " " + data.vz);
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit attack to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('attack', data);
                        }
                    }
                }
            });

            socket.on('catch', function (data) {
                console.log("Game " + data.game_id + ": player " + data.playerId + " catched.");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit catch to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('catch', data);
                        }
                    }
                }
            });

            socket.on('player_hit', function (data) {
                console.log("Game " + data.game_id + ": player " + data.playerId + " hit.");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit player_hit to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('player_hit', data);
                        }
                    }
                }
            });

            socket.on('chicken_hit', function (data) {
                console.log("Game " + data.game_id + ": player " + data.playerId + " caught chicken " + data.chickenId);
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit chicken_hit to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('chicken_hit', data);
                        }
                    }
                }
            });

            socket.on('container_hit', function (data) {
                console.log("Game " + data.game_id + ": player " + data.playerId + " hit.");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit container_hit to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('container_hit', data);
                        }
                    }
                }
            });

            socket.on('dead', function (data) {
                console.log("dead");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit dead to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('dead', data);
                        }
                    }
                }
            });

            socket.on('container_restore', function (data) {
                console.log("Game " + data.game_id + ": player " + data.playerId + " container restored.");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit container_restore to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('container_restore', data);
                        }
                    }
                }
            });

            socket.on('player_walk', function (data) {
                console.log("Game " + data.game_id + ": player " + data.playerId + " started to walk.");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit player_walk to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('player_walk', data);
                        }
                    }
                }
            });

            socket.on('player_idle', function (data) {
                console.log("Game " + data.game_id + ": player " + data.playerId + " went idle.");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit player_idle to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('player_idle', data);
                        }
                    }
                }
            });

            socket.on('game_over', function (data) {
                console.log("Game over");
                for (i = 0; i < gamerooms.length; i++) {
                    if (gamerooms[i].game_id.toString() === data.game_id) {
                        players = gamerooms[i].players;
                        for (j = 0; j < players.length; j++) {
                            console.log('emit game_over to player ' + players[j]);
                            io.to(clients[ids.indexOf(players[j])]).emit('game_over', data);
                        }
                    }
                }
            });
        });

        // Debugging functions
        app.post('/clients', (request, response, next) => {
            console.log("Clients: " + clients);
            response.json("");
        });

        app.post('/nicknames', (request, response, next) => {
            console.log("Nicknames: " + nicknames);
            response.json("");
        });

        app.post('/ids', (request, response, next) => {
            console.log("Ids: " + ids);
            response.json("");
        });

        app.post('/gamerooms', (request, response, next) => {
            console.log("Gamerooms:");
            for (i = 0; i < gamerooms.length; i++) {
                console.log("game_id: " + gamerooms[i].game_id + ", game_name: " + gamerooms[i].game_name + ", players: [" + gamerooms[i].players + "]");
            }
            response.json("");
        });

        app.post('/all', (request, response, next) => {
            console.log("Clients: " + clients);
            console.log("Nicknames: " + nicknames);
            console.log("Ids: " + ids);
            console.log("Gamerooms:");
            for (i = 0; i < gamerooms.length; i++) {
                console.log("game_id: " + gamerooms[i].game_id + ", game_name: " + gamerooms[i].game_name + ", players: [" + gamerooms[i].players + "]");
            }
            response.json("");
        });
    }
})