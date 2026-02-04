import axios from "axios";
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

  //Get api/Room/search-by-date
  searchByDate(checkIn, checkOut, adult, children) {
    return axiosClient.get(`/api/Room/search-by-date?checkInDate=${checkIn}&checkOutDate=${checkOut}&adult=${adult}&children=${children}`);
  },

  getRoomById(roomId) {
    return axiosClient.get(`/api/Room/${roomId}`);
  },

  changeStatusRoom(roomId, status) {
    return axiosClient.patch(`/api/Room/${roomId}/status`, {
      status: status,
    })
  }
};

export default roomApi;
