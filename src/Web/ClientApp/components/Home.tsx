import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export class Home extends React.Component<RouteComponentProps<{}>, {}> {

    private refGroupNameInput: HTMLInputElement;

    handleGoBtnClick = () => {
        if (this.refGroupNameInput.value == '') {
            return;
        }
        // Navigate with group name pram
        this.props.history.push('webrtc/' + this.refGroupNameInput.value);
    }

    public render() {
        return <div className="card">
            <div className="card-body">
                <blockquote className="blockquote mb-0">
                    <p>Example of applications using SignalR and WebRTC (simple group chat)</p>
                </blockquote>
                <div className="input-group mb-3">
                    <input type="text"
                        ref={(input) => this.refGroupNameInput = input as HTMLInputElement}
                        className="form-control"
                        placeholder="Enter group name"/>
                        <div className="input-group-append">
                        <button className="btn btn-outline-secondary"
                            type="button"
                            onClick={this.handleGoBtnClick}>Let's go</button>
                        </div>
                </div>
            </div>
        </div>;
    }
}
