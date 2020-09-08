import * as React from "react";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import "./App.less";
import Index from "./movies/index";
import Create from "./movies/create";
import Edit from "./movies/edit";
import Details from "./movies/details";
import Delete from "./movies/delete";

const App = () => {
  return (
    <>
      <h1>Hello</h1>
      <BrowserRouter>
        <Switch>
          <Route exact={true} path="/movies/" component={Index} />
          <Route exact={true} path="/movies/create/" component={Create} />
          <Route exact={true} path="/movies/edit/" component={Edit} />
          <Route exact={true} path="/movies/details/" component={Details} />
          <Route exact={true} path="/movies/delete/" component={Delete} />
        </Switch>
      </BrowserRouter>
    </>
  );
};

export default App;
