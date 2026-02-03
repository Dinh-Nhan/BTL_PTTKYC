import axiosClient from "../axiosClient";

export const clientApi = {
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
