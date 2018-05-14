import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { ChangeEvent } from 'react';
import WebsocketService from '../services/ChatWebsocketService';
import * as Toastr from 'toastr';
import { v4 as Guid } from 'uuid';
import { ChatMessage } from '../models/ChatMessage';

interface ChatState {
    nick: string,
    currentMessage: string,
    messages: ChatMessage[]
}

export class Chat extends React.Component<RouteComponentProps<{}>, ChatState> {

    private refChatNickNameInput: HTMLInputElement;
    
    constructor() {
        super();
        this.state = {
            nick: '',
            currentMessage: '',
            messages: []
        };

        Toastr.info('Chat loaded!');
        console.log('Ctor Chat');
    }

    handleStartBtnClick = () => {
        this.setState({
            nick: this.refChatNickNameInput.value
        });
    }

    handleOnSubmit = (event: any) => {
        event.preventDefault();

        let msg = {
            message: this.state.currentMessage,
            id: Guid(),
            nick: this.state.nick
        };

        WebsocketService.sendMessage(msg);
    }

    handleMessageOnChange = (input: ChangeEvent<HTMLInputElement>) => {
        this.setState({
            currentMessage: input.currentTarget.value
        })
    }

    componentWillUnmount() {
        WebsocketService.offRegisterMessageAdded((chatMessage: ChatMessage) => {
            alert(chatMessage.message);
        });
        WebsocketService.stopService();
    }

    componentWillMount() {
        WebsocketService.startService();
        WebsocketService.onRegisterMessageAdded((chatMessage: ChatMessage) => {
            this.state.messages.push(chatMessage);
            this.setState(prevState => {  });

            //alert(chatMessage.message);
        });
    }

    public render() {

        return <div className="card bg-info">
            <div className="card-header">
                Chat <strong>{this.state.nick}</strong>
            </div>
            <div className="card-body">
                
                {this.state.nick == '' &&
                    <div className="input-group">
                        <input type="text" className="form-control"
                            ref={(input) => this.refChatNickNameInput = input as HTMLInputElement}
                            placeholder="Enter chat username" />
                        <div className="input-group-append">
                            <button className="btn btn-success" onClick={this.handleStartBtnClick}
                                type="submit">Start</button>
                        </div>
                    </div>
                }
                
                {this.state.nick != '' && 
                    <ul>
                    {this.state.messages.map((msg) =>
                        <li key={msg.id}><p><strong>{msg.nick}</strong> :{msg.message}</p></li>
                    )}
                </ul>
                    }
            </div>

            {this.state.nick != '' && 
                <div className="card-footer">
                    <form onSubmit={this.handleOnSubmit}>
                        <div className="input-group">
                            <input type="text"
                                onChange={this.handleMessageOnChange}
                                className="form-control"
                                placeholder="Message"
                                disabled={this.state.nick == ''} />
                            <div className="input-group-append">
                                <button className="btn btn-outline-secondary"
                                    type="submit" disabled={this.state.nick == ''}>Send</button>
                            </div>
                        </div>

                    </form>
                </div>
            }
        </div>;
    }
}