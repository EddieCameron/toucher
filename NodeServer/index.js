/* toucher
 * -------------
 * index.js
 * creates rooms to manage message sending between socket.io clients
 * auto room creation and resizing in a lazy fashion
 */

var app = require('express')(),
    server = require('http').createServer(app),
    io = require('socket.io').listen(server),
    port = 8080,
    url  = 'http://localhost:' + port + '/';
	uuid = require( 'node-uuid' );
	
/* We can access nodejitsu enviroment variables from process.env */
/* Note: the SUBDOMAIN variable will always be defined for a nodejitsu app */
if(process.env.SUBDOMAIN){
  url = 'http://' + process.env.SUBDOMAIN + '.jit.su/';
}

var minPerRoom = 3;
var maxPerRoom = 10;

function addToEmptiestRoom( socket ) {
	// Find emptiest room with enough people in it
	var quietestRoom = -1;
	var numInRoom = 0;
	for ( var roomID in io.sockets.manager.rooms ) {
		if ( roomID != "" && io.sockets.manager.rooms.hasOwnProperty( roomID ) ) {
		numInRoom = io.sockets.manager.rooms[roomID].length;
			if ( quietestRoom == -1 || numInRoom < numInQuietest ) {
				quietestRoom = roomID;
				numInQuietest = numInRoom;
			}
		}
	}
	
	if ( quietestRoom == -1 ) {
		quietestRoom = uuid.v4(); // uuid makes a unique code
	}
	
	// add to quietest room
	socket.join( quietestRoom );
	console.log( "added " + socket.id + " to room: " + quietestRoom );
	numInRoom++;
	if ( numInRoom > maxPerRoom ) {
		var toLeave = [];
		var toleaveIndex = 0;
		// get set of clients to put in new room
		for ( var otherSocket in io.sockets.clients( 'quietestRoom' ) ) {
			if ( toLeave.length >= minPerRoom )
				break;
			toLeave.push( otherSocket );
		}
		
		// make new room for these clients
		var newRoom = uuid.v4();
		for ( var i = 0; i < toLeave.length; i++ ) {
			toLeave[i].leave( quietestRoom );
			toLeave[i].join( newRoom );
		}
	}
}
 
server.listen(port);
console.log("Express server listening on port " + port);
console.log(url);

app.get('/', function (req, res) {
  res.sendfile(__dirname + '/index.html');
});

io.sockets.on( 'connection', function( socket ) {
	console.log( 'Client connected' );
	
	socket.on( 'login', function( from, msg ) {
		console.log( from + " logged in" );
			
		addToEmptiestRoom( socket );
	} );
	
	//
	socket.on( 'message', function( msg ) {
		// find room
		var clientRooms = io.sockets.manager.roomClients[socket.id]
		var clientRoom = -1;
		for ( var i = 0; i < clientRooms.length; i++ ) {
			if ( clientRooms[i] != "" )
				clientRoom = clientRooms[i];
		}
		
		console.log( "Sending " + msg + " to room: " + clientRoom );
		socket.broadcast.to( clientRoom ).emit( 'message', msg );
	} );
	
	socket.on( 'disconnect', function() {
		console.log( 'Client disconnected' );
		
		// find room
		var clientRooms = io.sockets.manager.roomClients[socket.id]
		var clientRoom = -1;
		for ( var i = 0; i < clientRooms.length; i++ ) {
			if ( clientRooms[i] != "" )
				clientRoom = clientRooms[i];
		}
		
		var otherClients = io.sockets.clients( clientRoom );
		console.log( socket.id + " leaving room of size " + otherClients.length );
		if ( otherClients.length - 1 < minPerRoom ) {
			// if will be less than min in this room, put in another room
			console.log( "Dissolving room" );
			for ( var i = 0; i < otherClients.length; i++ ) {
				if ( otherClients[i] != socket ) {
					otherClients[i].leave( clientRoom );
					addToEmptiestRoom( otherClients[i] );
				}
			}
		}
	} );
} );