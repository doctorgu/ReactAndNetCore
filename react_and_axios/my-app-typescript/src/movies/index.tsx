import React, { FC, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Button, Table, } from "antd";
import { MovieModel } from "../models/MovieModel";
import axios from "axios";
const { Column } = Table;


const Index: FC = () => {
    const [data, setData] = useState<MovieModel[]>([]);

    useEffect(() => {
        // axios.get("https://localhost:44357/movies")
        //     .then((value: any) => {
        //         const movies: IMovie[] = value.data;
        //         console.log(movies);
        //         setData(movies);
        //     });

        async function getData() {
            const result = await axios.get("https://localhost:44357/movies");
            const movies: MovieModel[] = result.data;
            setData(movies);
            console.log(movies);
        }
        getData();
    }, []);
    
    const dataSource = data;
            
    return (
        <div>
            <Link to="/movies/create">
                <Button type="primary">Create</Button>
            </Link>

            <Table rowKey={"id"} dataSource={dataSource}>
                <Column title="Genre" dataIndex="genre" />
                <Column title="ID" dataIndex="id" />
                <Column title="Price" dataIndex="price" />
                <Column title="ReleaseDate" dataIndex="releaseDate" />
                <Column title="Title" dataIndex="title" />
                <Column title="Action" render={
                    (text: string, record: any) => (
                        <>
                            <Link to={{ pathname: "/movies/edit/", search: `?id=${record.id}` }}>Edit</Link>
                            &nbsp;| <Link to={{ pathname: "/movies/details", search: `id=${record.id}`}}>Details</Link>
                            &nbsp;| <Link to={{ pathname: "/movies/delete", search: `id=${record.id}`}}>Delete</Link>
                        </>
                    )
                } />                
            </Table>

            {/* <Table rowKey={"id"} dataSource={dataSource} columns={columns} />; */}
        </div>
    );
};

export default Index;