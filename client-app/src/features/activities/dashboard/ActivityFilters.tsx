import React from "react";
import { Header, Menu } from "semantic-ui-react";
import Calendar from 'react-calendar';

export default function ActivityFilters() {
    return (
        <>
        <Menu vertical size='large' style={{width: '100%', marginTop: 25}}>
            <Header icon='filter' attached color='teal' content='Filters'></Header>
            <Menu.Item content='All activities'></Menu.Item>
            <Menu.Item content="I'm going"></Menu.Item>
            <Menu.Item content="I'm hosting"></Menu.Item>
        </Menu>
        <Header />
        <Calendar />
        </>
    )
}