// Import package
var mongodb = require('mongodb');
var mongoose = require('mongoose');
var socketio = require('socket.io');
//var ObjectID = mongodb.ObjectID;
var express = require('express')
var bodyParser = require('body-parser')

// Create Express Service
var app = express();
app.use(bodyParser.json({ limit: '50mb' }));
app.use(bodyParser.urlencoded({ limit: '50mb', extended: true }));
app.use(express.json({ limit: '50mb' }));
app.use(express.urlencoded({ limit: '50mb', extended: true }));


// Create MongoDB Client
var MongoClient = mongodb.MongoClient;

// Connection URL
var url = 'mongodb://localhost:27017'

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
            var db = client.db('minecraft');

            socket.emit('connected');

            // Link socket event handlers
            socket.on('requireList', function (data) {
                console.log("requireList");
                db.collection('maps').findOne({}, function (error, res) {
                    if (error) console.log("error in requireList: " + error);
                    socket.emit('sendList', { 'worldList': res.names });
                })
            })

            socket.on('download', function (data) {
                console.log("rdownload");
                var worldName = data.worldName;

                db.collection('maps').findOne({}, function (error, res) {
                    if (error) console.log("error in download: " + error);

                    var names = res.names;
                    var data = res.data;

                    var result = {
                        'worldName': worldName,
                        'worldData': data[names.indexOf(worldName)]
                    }

                    socket.emit('sendWorld', result);
                })
            })

            socket.on('upload', function (data) {
                console.log("upload");
                var worldName = data.worldName;
                var worldData = data.worldData;

                db.collection('maps').findOne({}, function (error, res) {
                    if (error) console.log("error in download: " + error);

                    var names = res.names;
                    var data = res.data;

                    var index = names.indexOf(worldName);
                    if (index == -1) {
                        names.push(worldName);
                        data.push(worldData);
                    }
                    else {
                        data[index] = worldData;
                    }

                    db.collection('maps').updateOne({}, { $set: { 'names': names, 'data': data } }, function (err, res) {
                        console.log('updated');
                    });
                });
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
    }
})