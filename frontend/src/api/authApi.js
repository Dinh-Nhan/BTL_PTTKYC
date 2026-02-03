import axiosClient from "./axiosClient";

const authApi = {
  getMe: () => {
    return axiosClient.get("/api/auth/me");
  },

  login: (email, password) => {
    return axiosClient.post("/api/auth/login", { email, password });
  },

  logout: () => {
    return axiosClient.post("/api/auth/logout");
  },

  refresh: () => {
    return axiosClient.post("/api/auth/refresh");
  },

  introspect: (token) => {
    return axiosClient.post("/api/auth/introspect", { token });
  },
};

export default authApi;
