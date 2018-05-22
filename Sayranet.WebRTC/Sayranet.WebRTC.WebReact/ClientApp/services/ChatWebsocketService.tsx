import * as signalR from '@aspnet/signalr';
import { ChatMessage } from '../models/ChatMessage';

class ChatWebsocketService {
    private _connection: signalR.HubConnection;
    private _url: string = `http://test1.mstachyra.hostingasp.pl/chat`;

    constructor() {
        console.log('Ctor ChatWebsocketService');
    }

    onRegisterMessageAdded(messageAdded: (chatMessage: ChatMessage) => void) {
        // get nre chat message from the server
        this._connection.on('send', (chatMessage: ChatMessage) => {
            messageAdded(chatMessage);
        });
    }

    offRegisterMessageAdded(messageAdded: (chatMessage: ChatMessage) => void) {
        // get nre chat message from the server
        this._connection.off('send', (chatMessage: ChatMessage) => {
            messageAdded(chatMessage);
        });
    }

    sendMessage(message: ChatMessage) {
        // send the chat message to the server
        this._connection.invoke('SendMessage', message);
    }

    startService() {

        // create Connection
        this._connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(this._url, signalR.HttpTransportType.WebSockets)
            .build();
        // start connection
        this._connection.start().catch(err => console.error(err, 'red'));
    }

    stopService() {
        this._connection.stop();
    }
}

const WebsocketService = new ChatWebsocketService();

export default WebsocketService;