import React, { FC, useState, useEffect } from "react";
import { Link, useLocation } from "react-router-dom";
import { Descriptions } from "antd";
import axios from "axios";
import moment from "moment";
import { MovieModel } from "../models/MovieModel";
const queryString = require("query-string");

const Details: FC = (props) => {
  const [data, setData] = useState<MovieModel>(new MovieModel());

  const location = useLocation();

  useEffect(() => {
    async function getData() {
      const search = queryString.parse(location.search);
      const result = await axios.get(`https://localhost:44357/movies/details/${search.id}/`);
      const movie: MovieModel = result.data;
      setData(movie);
      console.log(movie);
    }
    getData();
  }, []);

  return (
    <>
      <h1>Details</h1>

      <h4>Movie</h4>
      <hr />

      <Descriptions title={data.title} bordered column={1}>
        <Descriptions.Item label="Title">{data.title}</Descriptions.Item>
        <Descriptions.Item label="Release Date">{moment(data.releaseDate).format("YYYY-MM-DD")}</Descriptions.Item>
        <Descriptions.Item label="Genre">{data.genre}</Descriptions.Item>
        <Descriptions.Item label="Price">{data.price}</Descriptions.Item>
      </Descriptions>

      <Link to="/movies/">Back to List</Link>
    </>
  )
};

export default Details;