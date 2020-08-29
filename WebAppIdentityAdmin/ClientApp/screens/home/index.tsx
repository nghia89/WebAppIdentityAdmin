import * as React from 'react';
import Moment  from 'moment'
export default class Home extends React.Component {
    private renderDNow = () => {
        const date = new Date();
        return  Moment(date).format()
    }
    render() {
        return (
            <div>
                <h1>{this.renderDNow()}Trang chủ </h1>
            </div>
        );
    }

}