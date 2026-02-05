import axiosClient from "../api/axiosClient";

const clientApi = {
  getAll: () => {
    return axiosClient.get("/api/client");
  },

  deleteClient: (clientId) => {
    return axiosClient.delete(`/api/client/${clientId}`);
  },

  getClientByInfo: (info) => {
    return axiosClient.get(`/api/client/search`, { params: { info } });
  },
};

export default clientApi;