import axiosClient from "./axiosClient";

const roomApi = {
  getAll: () => {
    return axiosClient.get("/api/room");
  },

  getAvailable() {
    return axiosClient.get("/api/Room/available");
  },

  // PATCH /api/Room/{roomId}/activate
  activate(roomId) {
    return axiosClient.patch(`/api/Room/${roomId}/activate`);
  },

  // PATCH /api/Room/{roomId}/deactivate
  deactivate(roomId) {
    return axiosClient.patch(`/api/Room/${roomId}/deactivate`);
  },
};

export default roomApi;
