import * as signalR from '@aspnet/signalr';

class ChatWebsocketService {
    private _connection: signalR.HubConnection;

    constructor() {
        let url: string = `http://localhost:50780/chat`;

        // create Connection
        this._connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(url, signalR.HttpTransportType.WebSockets)
            .build();

        // start connection
        this._connection.start().catch(err => console.error(err, 'red'));
    }

    registerMessageAdded(messageAdded: (msg: string) => void) {
        // get nre chat message from the server
        this._connection.on('send', (message: string) => {
            messageAdded(message);
        });
    }

    sendMessage(message: string) {
        // send the chat message to the server
        this._connection.invoke('SendMessage', message);
    }
}

const WebsocketService = new ChatWebsocketService();

export default WebsocketService;