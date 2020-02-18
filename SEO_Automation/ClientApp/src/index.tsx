import "bootstrap/dist/css/bootstrap.css";
import "react-toastify/dist/ReactToastify.css";

import * as React from "react";
import * as ReactDOM from "react-dom";
import App from "./App";
import { toast } from "react-toastify";
import registerServiceWorker from "./registerServiceWorker";

toast.configure({
  autoClose: 2000,
  position: "bottom-right"
});

ReactDOM.render(<App />, document.getElementById("root"));

registerServiceWorker();
