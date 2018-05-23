import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import webRtcAdapter from 'webrtc-adapter';
import * as signalR from '@aspnet/signalr';

interface WebRtcState {
    messages: string[]
}

interface WebRtcParams {
    groupName: string
}

export class WebRtc extends React.Component<RouteComponentProps<WebRtcParams>, WebRtcState> {

    private connection: signalR.HubConnection;
    
    private url: string = `http://test1.mstachyra.hostingasp.pl/chat`;
    //private url: string = `http://localhost:50780/chat`;
    private refChatInput: HTMLInputElement;
    private refChatAreaDiv: HTMLDivElement;
    private video: HTMLVideoElement;

    // Put variables in global scope to make them available to the browser console.
    private constraints: MediaStreamConstraints = {
        audio: false,
        video: true
    };
    private errors: string[];

    constructor() {
        super();
        this.connection = null;
        this.state = {
            messages: ['Welcome in app']
        };

        this.handleSuccess.bind(this);
        this.handleError.bind(this);

        //navigator
        //    .mediaDevices
        //    .getUserMedia(this.constraints)
        //    .then(this.handleSuccess)
        //    .catch(this.handleError);
    }

    componentWillMount() {
        // create Connection
        this.connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(this.url, signalR.HttpTransportType.WebSockets)
            .build();
        // start connection
        this.connection.start()
            .catch(err => console.error(err))
            .then(() => {

                this.connection.invoke('JoinGroup', this.props.match.params.groupName)
                    .then(() => {
                        console.info('Group registered');
                    })
                
                this.connection.on('Message', (msg: string) => {
                    this.setMsg('Recive: ' + msg);
                });
            });
    }

    componentWillUnmount() {
        this.connection.stop();
        this.connection = null;
    }

    setMsg = (msg: string) => {
        this.setState({
            messages: this.state.messages.concat([msg])
        }, () => {
            this.refChatAreaDiv.scrollTop = this.refChatAreaDiv.scrollHeight;
        });
    }

    handleSend = () => {
        this.setMsg('You: ' + this.refChatInput.value);
        if (this.connection == null) {
            console.warn('Not connected');
            return;
        }
        this.connection.invoke('GroupMessage',
            { GroupName: this.props.match.params.groupName, Message: this.refChatInput.value }
            );
        this.refChatInput.value = '';
    }

    handleSuccess = (stream: MediaStream) => {
        var videoTracks = stream.getVideoTracks();
        console.log('Got stream with constraints:', this.constraints);
        console.log('Using video device: ' + videoTracks[0].label);
        stream.oninactive = function () {
            console.log('Stream inactive');
        };
        //window.stream = stream; // make variable available to browser console
        this.video.srcObject = stream;
    };

    private Width(video: boolean | MediaTrackConstraints) {
        return ((video as MediaTrackConstraints).width as ConstrainLongRange).exact;
    };

    handleError = (error: any) => {
        if (error.name === 'ConstraintNotSatisfiedError') {


            this.errors.push('The resolution ' + this.Width(this.constraints.video) + 'x' +
                this.Width(this.constraints.video) + ' px is not supported by your device.');
        } else if (error.name === 'PermissionDeniedError') {
            this.errors.push('Permissions have not been granted to use your camera and ' +
                'microphone, you need to allow the page access to your devices in ' +
                'order for the demo to work.');
        }
        this.errors.push('getUserMedia error: ' + error.name, error);
    };

    public render() {
        return <div id="container">

            <div className="alert alert-info" role="alert">
                Welcome in group '<strong>{this.props.match.params.groupName}</strong>'
            </div>

            <div className="row mt-4">
                <div className="col-sm-6">
                    <div className="card">
                        <div className="card-header">
                            Local
                        </div>
                        <div className="card-body">
                            <h5 className="card-title">Special title treatment</h5>
                            <p className="card-text">With supporting text below as a natural lead-in to additional content.</p>
                            <a href="#" className="btn btn-primary">Go somewhere</a>
                        </div>
                    </div>
                </div>
                <div className="col-sm-6">
                    <div className="card">
                        <div className="card-header">
                            Remote
                        </div>
                        <div className="card-body">
                            <h5 className="card-title">Special title treatment</h5>
                            <p className="card-text">With supporting text below as a natural lead-in to additional content.</p>
                            <a href="#" className="btn btn-primary">Go somewhere</a>
                        </div>
                    </div>
                </div>
            </div>

            <div className="row mt-4">
                <div className="col-12">
                    <div className="card">
                        <div className="card-header">
                            Chat / Console
                        </div>
                        <div
                            ref={(div) => this.refChatAreaDiv = div as HTMLDivElement}
                            className="card-body chat-area">
                            {this.state.messages.map((msg) =>
                                <p>{msg}</p>
                            )}
                        </div>
                        <div className="card-footer">
                            <div className="input-group">
                                <input type="text"
                                    ref={(input) => this.refChatInput = input as HTMLInputElement}
                                    className="form-control"
                                    placeholder="Enter message" />
                                <div className="input-group-append">
                                    <button className="btn btn-outline-secondary"
                                        type="button"
                                        onClick={this.handleSend}>Send</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


            <h1><a href="//webrtc.github.io/samples/" title="WebRTC samples homepage">WebRTC samples</a> <span>getUserMedia</span></h1>

            <video id="gum-local" autoPlay={true} playsInline={true} ref={(v) => this.video = v as HTMLVideoElement}></video>

            <div id="errorMsg"></div>

            <p className="warning"><strong>Warning:</strong> if you're not using headphones, pressing play will cause feedback.</p>
            <p>Display the video stream from <code>getUserMedia()</code> in a video element.</p>
            <p>The <code>MediaStream</code> object <code>stream</code> passed to the <code>getUserMedia()</code> callback is in global scope, so you can inspect it from the console.</p>
            <a href="https://github.com/webrtc/samples/tree/gh-pages/src/content/getusermedia/gum" title="View source for this page on GitHub" id="viewSource">View source on GitHub</a>
        </div>
    }
}
