import React, { FC, useState } from "react";
import { Link, Redirect } from "react-router-dom";
import { Form, Input, InputNumber, DatePicker, Button, Layout, } from "antd";
import axios from "axios";
import { MovieModel } from "../models/MovieModel";
const { Item } = Form;

const Create: FC = () => {
  const [toList, setToList] = useState(false);

  const onFinish = async (values: any) => {
    const movie: MovieModel = {
      genre: values.genre,
      id: values.id,
      price: values.price,
      releaseDate: values.releaseDate.format("YYYY-MM-DD"),
      title: values.title,
    };
    const result = await axios.post("https://localhost:44357/movies/create/", movie);
    console.log(result);
    if (result.data) {
      setToList(true);
    }
  };
  const onFinishFailed = (errorInfo: any) => {
    console.log("failed", errorInfo);
  };
  

  if (toList) {
    return <Redirect to="/movies/" />
  }

  return (
    <>
      <h1>Create</h1>

      <h4>Movie</h4>
      <hr />
      <Form
        labelCol={{span: 8}}
        wrapperCol={{span: 16}}
        onFinish={onFinish}
        onFinishFailed={onFinishFailed}
      >
        <Item label="Title" name="title" rules={[{ required: true, message: "Title required!" }]}>
          <Input />
        </Item>
        <Item label="Release Date" name="releaseDate" rules={[{ required: true, message: "Release date required!" }]}>
          <DatePicker />
        </Item>
        <Item label="Genre" name="genre" rules={[{ required: true, message: "Genre required!" }]}>
          <Input />
        </Item>
        <Item label="Price" name="price" rules={[{ required: true, message: "Price required!" }]}>
          <InputNumber />
        </Item>

        <Item wrapperCol={{offset: 8, span: 16 }}>
          <Button type="primary" htmlType="submit">Create</Button>
        </Item>
      </Form>

      <Link to="/movies/">Back to List</Link>
    </>
  )
};

export default Create;