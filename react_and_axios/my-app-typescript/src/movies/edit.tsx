import React, { FC, useState, useEffect } from "react";
import { Link, Redirect, useLocation } from "react-router-dom";
import { Form, Input, InputNumber, DatePicker, Button, } from "antd";
import axios from "axios";
import moment from "moment";
import { MovieModel } from "../models/MovieModel";
const queryString = require("query-string");

const Edit: FC = (props) => {
  const [toList, setToList] = useState(false);
  
  const [data, setData] = useState<MovieModel>(new MovieModel());

  const location = useLocation();

  const onFinish = async (values: any) => {
    const movie: MovieModel = {
      id: data.id,
      genre: values.genre,
      price: values.price,
      releaseDate: values.releaseDate.format("YYYY-MM-DD"),
      title: values.title,
    };
    const result = await axios.post(`https://localhost:44357/movies/edit/?id=${data.id}`, movie);
    console.log(result);
    if (result.data) {
      setToList(true);
    }
  };
  const onFinishFailed = (errorInfo: any) => {
    console.log("failed", errorInfo);
  };
  
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

  if (toList) {
    return <Redirect to="/movies/" />
  }
  if (!data.id) {
    return <h1>loading...</h1>
  }

  return (
    <>
      <h1>Edit</h1>

      <h4>Movie</h4>
      <hr />
      <Form
        labelCol={{span: 8}}
        wrapperCol={{span: 16}}
        onFinish={onFinish}
        onFinishFailed={onFinishFailed}
        initialValues={{title: data.title, releaseDate: moment(data.releaseDate), genre: data.genre, price: data.price}}
      >
        <Form.Item label="Title" name="title" rules={[{ required: true, message: "Title required!" }]}>
          <Input />
        </Form.Item>
        <Form.Item label="Release Date" name="releaseDate" rules={[{ required: true, message: "Release date required!" }]}>
          <DatePicker />
        </Form.Item>
        <Form.Item label="Genre" name="genre" rules={[{ required: true, message: "Genre required!" }]}>
          <Input />
        </Form.Item>
        <Form.Item label="Price" name="price" rules={[{ required: true, message: "Price required!" }]}>
          <InputNumber />
        </Form.Item>

        <Form.Item wrapperCol={{offset: 8, span: 16 }}>
          <Button type="primary" htmlType="submit">Save</Button>
        </Form.Item>
      </Form>

      <Link to="/movies/">Back to List</Link>
    </>
  )
};

export default Edit;
