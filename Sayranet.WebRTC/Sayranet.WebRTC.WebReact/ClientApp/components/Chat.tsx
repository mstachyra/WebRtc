import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { ChangeEvent } from 'react';
import WebsocketService from '../services/ChatWebsocketService'

interface ChatState {
    isClicked: boolean,
    currentMessage: string
}

export class Chat extends React.Component<RouteComponentProps<{}>, ChatState> {
    constructor() {
        super();
        this.state = {
            isClicked: true,
            currentMessage: ''
        };

        WebsocketService.registerMessageAdded((message: string) => {
            alert(message);
        });
    }

    handleClick = () => {
        alert(this.state.isClicked);
    }

    handleOnSubmit = (event: any) => {
        event.preventDefault();
        WebsocketService.sendMessage(this.state.currentMessage);
        //alert(this.state.currentMessage);
    }

    handleMessageOnChange = (input: ChangeEvent<HTMLInputElement>) => {
        this.setState({
            currentMessage: input.currentTarget.value
        })
    }

    public render() {
        return <div className="card">
            <div className="card-header">
                Chat
            </div>
            <div className="card-body">
            </div>
            <div className="card-footer">
                <form onSubmit={this.handleOnSubmit}>

                    <div className="input-group mb-3">
                        <input type="text"
                            onChange={this.handleMessageOnChange}
                            className="form-control"
                            placeholder="Recipient's username"
                            aria-label="Recipient's username"
                            id="msg" />
                        <div className="input-group-append">
                            <button className="btn btn-outline-secondary"
                                type="submit">Button</button>
                        </div>
                    </div>

                </form>
            </div>
        </div>;
    }
}