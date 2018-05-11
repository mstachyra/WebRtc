import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface ChatState {
}

export class Chat extends React.Component<RouteComponentProps<{}>, ChatState> {
    constructor(props: any) {
        super(props);
    }

    public render() {
        return <div className="card">
            <div className="card-header">
                Chat
            </div>
            <div className="card-body">
            </div>
            <div className="card-footer">
                <div className="input-group mb-3">
                    <input type="text" className="form-control" placeholder="Recipient's username" aria-label="Recipient's username" aria-describedby="basic-addon2" />
                    <div className="input-group-append">
                        <button className="btn btn-outline-secondary" type="button">Button</button>
                    </div>
                </div>
            </div>
        </div>;
    }
}