import React, { FC, useState, useEffect } from "react";
import { Link, Redirect, useLocation } from "react-router-dom";
import { Descriptions, Button } from "antd";
import axios from "axios";
import moment from "moment";
import { MovieModel } from "../models/MovieModel";
const queryString = require("query-string");

const Delete: FC = (props) => {
  const [toList, setToList] = useState(false);
  
  const [data, setData] = useState<MovieModel>(new MovieModel());

  const location = useLocation();

  useEffect(() => {
    async function getData() {
      const search = queryString.parse(location.search);
      const result = await axios.get(`https://localhost:44357/movies/details/${search.id}/`);
      const data: MovieModel = result.data;
      setData(data);
    }
    getData();
  }, []);
  
  const onDelete = async (e: any) => {
    const search = queryString.parse(location.search);
    const result = await axios.post(`https://localhost:44357/movies/delete/${search.id}/`);
    setToList(true);
  }

  if (toList) {
    return <Redirect to="/movies/" />
  }

  return (
    <>
      <h1>Delete</h1>

      <h3>Are you sure you want to delete this?</h3>

      <h4>Movie</h4>
      <hr />

      <Descriptions bordered column={1}>
        <Descriptions.Item label="Title">{data.title}</Descriptions.Item>
        <Descriptions.Item label="Release Date">{moment(data.releaseDate).format("YYYY-MM-DD")}</Descriptions.Item>
        <Descriptions.Item label="Genre">{data.genre}</Descriptions.Item>
        <Descriptions.Item label="Price">{data.price}</Descriptions.Item>
      </Descriptions>

      <div>
        <Button type="primary" onClick={onDelete}>Delete</Button>
        <Link to="/movies/">Back to List</Link>
      </div>
    </>
  )
};

export default Delete;