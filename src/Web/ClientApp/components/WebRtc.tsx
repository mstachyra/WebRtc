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

interface WebRtcSdp {
    type: string,
    sdp: string
}

export class WebRtc extends React.Component<RouteComponentProps<WebRtcParams>, WebRtcState> {
    
    private connection: signalR.HubConnection;

    // Published Signaling service with TLS
    private url: string = `https://test1.mstachyra.hostingasp.pl/chat`;
    //private url: string = `http://localhost:50780/chat`;
    private refChatInput: HTMLInputElement;
    private refChatAreaDiv: HTMLDivElement;

    private localVideo: HTMLVideoElement;
    private localStream: MediaStream;
    private remoteVideo: HTMLVideoElement;

    private rtcConfiguration: RTCConfiguration = {
        'iceServers': [
            {
                'urls': 'stun:stun.l.google.com:19302'
            },
            {
                'urls': 'turn:192.158.29.39:3478?transport=udp',
                'credential': 'JZEOEt2V3Qb0y27GRntt2u2PAYA=',
                'username': '28224511:1379330808'
            },
            {
                'urls': 'turn:192.158.29.39:3478?transport=tcp',
                'credential': 'JZEOEt2V3Qb0y27GRntt2u2PAYA=',
                'username': '28224511:1379330808'
            }
        ]
    };

    private peerConnection: RTCPeerConnection;

    // Put variables in global scope to make them available to the browser console.
    private constraints: MediaStreamConstraints = {
        audio: true,
        video: true
    };

    constructor() {
        super();
        this.connection = null;
        this.state = {
            messages: ['Welcome in app']
        };

        this.handleError.bind(this);
        this.handleSendOffer.bind(this);
        this.handleSdp.bind(this);
        this.handleStop.bind(this);
        this.handleStart.bind(this);
        this.handleIceCandidate.bind(this);
    }

    componentWillMount() {
        // create Connection
        this.connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl(this.url, signalR.HttpTransportType.WebSockets)
            .build();


        // start connection
        this.connection.start()
            .catch((err) => {
                console.error(err)
            })
            .then(() => {

                // Send Groupname to register in signaling service
                this.connection.invoke('JoinGroup', this.props.match.params.groupName)
                    .then(() => {
                        console.info('Group registered');
                    });

                // Simple group Message recive
                this.connection.on('Message', (msg: string) => {
                    this.setMsg('Recive: ' + msg);
                });

                this.connection.on('Sdp', this.handleSdp);
                this.connection.on('IceCandidate', this.handleIceCandidate);
            });
    }

    componentWillUnmount() {
        this.connection.stop();
        this.connection = null;
        this.handleStop();
    }

    // Add message to Chat area
    setMsg = (msg: string) => {
        this.setState({
            messages: this.state.messages.concat([msg])
        }, () => {
            this.refChatAreaDiv.scrollTop = this.refChatAreaDiv.scrollHeight;
        });
    }

    // Handler for button send in chat
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

    handleStart = () => {
        navigator
            .mediaDevices
            .getUserMedia(this.constraints)
            .then((stream: MediaStream) => {
                stream.oninactive = function () {
                    console.log('Stream inactive');
                };
                //window.stream = stream; // make variable available to browser console
                this.localVideo.srcObject = stream;
                this.localStream = stream;

                var videoTracks = this.localStream.getVideoTracks();
                var audioTracks = this.localStream.getAudioTracks();
                if (videoTracks.length > 0) {
                    console.log('Using video device: ' + videoTracks[0].label);
                }
                if (audioTracks.length > 0) {
                    console.log('Using audio device: ' + audioTracks[0].label);
                }

                console.log('Got stream with constraints:', this.constraints);
                console.log('Using video device: ' + videoTracks[0].label);
                
                this.peerConnection = new RTCPeerConnection(this.rtcConfiguration);
                console.log('Created local peer connection');

                this.peerConnection.onicecandidate = (e) => {
                    console.log('onicecandidate ', JSON.stringify(e.candidate));
                    //console.log('On ice candidate', e);
                    //this.peerConnection.addIceCandidate(e.candidate);
                    this.connection.invoke('GroupIceCandidate',
                        { GroupName: this.props.match.params.groupName, IceCandidate: e.candidate }
                    );
                };

                this.peerConnection.oniceconnectionstatechange = function (e) {
                    console.log('ICE state change event: ', e);
                };

                this.peerConnection.onaddstream = (e) => {
                    console.log('onaddstream ', e);
                    this.remoteVideo.srcObject = e.stream;
                };

                this.peerConnection.addStream(this.localStream);
                console.log('handleSuccess end');
            })
            .catch(this.handleError);
    };

    handleStop = () => {
        if (this.peerConnection) {
            this.peerConnection.close()
            this.peerConnection = null;
        }

        this.localVideo.srcObject = null;

        if (this.localStream) {
            this.localStream.getVideoTracks().forEach((v) => {
                v.stop();
            });
            this.localStream.getAudioTracks().forEach((v) => {
                v.stop();
            });
        }
    };

    handleSdp = (sdp: WebRtcSdp) => {
        // Init from Caller, not by button send Offer
        console.debug('Recive SDP\n', sdp);

        var typeSdp = this.GetType(sdp.type.toString());

        var s: any = {
            sdp: sdp.sdp, type: typeSdp
        };

        if (s.type == "offer") {
            
            this.peerConnection.setRemoteDescription(s);

            this.peerConnection.createAnswer().then((answer) => {

                console.debug("Success createAnswer\n", answer);
                this.peerConnection.setLocalDescription(answer);
                
                // SEND Answer
                this.connection.invoke('GroupSdp', {
                    GroupName: this.props.match.params.groupName,
                    Sdp: {
                        Type: answer.type,
                        Sdp: answer.sdp
                    }
                })
                .then(() => {
                    console.info('Offer sended, finish this side');
                });
            });

        } else if (s.type == "answer") {

            // Peer must be created and configured
            this.peerConnection.setRemoteDescription(s).then(() => {
                console.log('Success setRemoteDescription, finish signaling');

            });

        }
    }

    handleIceCandidate = (iceCandidate: RTCIceCandidate) => {
        if (this.peerConnection) {
            this.peerConnection.addIceCandidate(iceCandidate);
        } else {
            console.warn('handleIceCandidate ', JSON.stringify(iceCandidate));
        }
    }

    handleSendOffer = () => {
        // check if started
        if (this.peerConnection) {

            this.peerConnection.createOffer().then((sdp) => {
                console.log('Offer from loacal\n' + sdp.sdp);

                this.peerConnection.setLocalDescription(sdp).then(() => {
                    console.log('Success setLocalDescription, wait for answer');
                });

                let groupSDP = {
                    GroupName: this.props.match.params.groupName,
                    Sdp: {
                        Type: sdp.type,
                        Sdp: sdp.sdp
                    }
                };

                this.connection.invoke('GroupSdp', groupSDP)
                    .then(() => {
                        console.info('Offer sended');
                    });
            });
        } else {
            this.setMsg('WebRTC not started');
        }
    };



    private GetType(type: string): RTCSdpType {
        switch (type) {
            case "0": return "offer";
            case "1": return "pranswer";
            case "2": return "answer";
            default:
        }
        return "pranswer";
    };

    private Width(video: boolean | MediaTrackConstraints) {
        return ((video as MediaTrackConstraints).width as ConstrainLongRange).exact;
    };

    handleError = (error: any) => {
        if (error.name === 'ConstraintNotSatisfiedError') {
            console.error('The resolution ' + this.Width(this.constraints.video) + 'x' +
                this.Width(this.constraints.video) + ' px is not supported by your device.');
        } else if (error.name === 'PermissionDeniedError') {
            console.error('Permissions have not been granted to use your camera and ' +
                'microphone, you need to allow the page access to your devices in ' +
                'order for the demo to work.');
        }
        console.error('getUserMedia error: ' + error.name, error);
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
                            <video autoPlay={true} ref={(v) => this.localVideo = v as HTMLVideoElement} width={320} height={240}></video>
                        </div>
                    </div>
                </div>
                <div className="col-sm-6">
                    <div className="card">
                        <div className="card-header">
                            Remote
                        </div>
                        <div className="card-body">
                            <video autoPlay={true} ref={(v) => this.remoteVideo = v as HTMLVideoElement} width={320} height={240}></video>
                        </div>
                    </div>
                </div>
                <div>
                    <div className="col">
                        <button className="btn btn-info" onClick={this.handleStart}>Start</button>
                        <button className="btn btn-info" onClick={this.handleSendOffer}>Send Offer</button>
                        <button className="btn btn-info" onClick={this.handleStop}>Stop</button>
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
        </div>
    }
}
