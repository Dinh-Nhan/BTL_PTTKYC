import "./index.css";
import App from "./App.tsx";
import React from "react";
import { BrowserRouter } from "react-router-dom";
import AuthProvider from "./contexts/auth-context.tsx";
import ReactDOM from "react-dom/client";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <BrowserRouter>
      <AuthProvider>
        <App />
      </AuthProvider>
    </BrowserRouter>
  </React.StrictMode>,
);
