import * as React from 'react';
import { Link, NavLink } from 'react-router-dom';

export class NavMenu extends React.Component<{}, {}> {
    public render() {
        return <nav className="navbar navbar-expand-lg navbar-light bg-light">
            <Link className='navbar-brand' to={'/'}>Sayranet.WebRTC.WebReact</Link>
            <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse" id="navbarNav">
                <ul className="navbar-nav">
                    <li className="nav-item">

                        <NavLink to={'/'} className="btn" exact activeClassName='btn-outline-info' >
                            <i className="fas fa-home mr-2"></i>
                            Home
                            </NavLink>
                    </li>
                    <li className="nav-item">
                        <NavLink to={'/Webrtc'} className="btn" exact activeClassName='btn-outline-info' >
                            <i className="fas fa-camera mr-2"></i>
                            WebRTC
                            </NavLink>
                    </li>
                </ul>
            </div>
        </nav>;




        //<div className='main-nav'>
        //        <div className='navbar navbar-inverse'>
        //        <div className='navbar-header'>
        //            <button type='button' className='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
        //                <span className='sr-only'>Toggle navigation</span>
        //                <span className='icon-bar'></span>
        //                <span className='icon-bar'></span>
        //                <span className='icon-bar'></span>
        //            </button>
        //            <Link className='navbar-brand' to={ '/' }>Sayranet.WebRTC.WebReact</Link>
        //        </div>
        //        <div className='clearfix'></div>
        //        <div className='navbar-collapse collapse'>
        //            <ul className='nav navbar-nav'>
        //                <li>
        //                    <NavLink to={ '/' } exact activeClassName='active'>
        //                        <span className='glyphicon glyphicon-home'></span> Home
        //                    </NavLink>
        //                </li>
        //                <li>
        //                    <NavLink to={ '/counter' } activeClassName='active'>
        //                        <span className='glyphicon glyphicon-education'></span> Counter
        //                    </NavLink>
        //                </li>
        //                <li>
        //                    <NavLink to={ '/fetchdata' } activeClassName='active'>
        //                        <span className='glyphicon glyphicon-th-list'></span> Fetch data
        //                    </NavLink>
        //                </li>
        //            </ul>
        //        </div>
        //    </div>
        //</div>;
    }
}
