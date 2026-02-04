import axiosClient from "./axiosClient";

export const roomTypeApi = {
  getAll: () => {
    return axiosClient.get("/api/RoomType");
  },

  addRoomType: (data) => {
    return axiosClient.post("/api/RoomType", data);
  },

  getRoomTypeById: (roomTypeId) => {
    return axiosClient.get(`/api/RoomType/${roomTypeId}`);
  },

  updateRoomType: (roomType, data) => {
    return axiosClient.put(`/api/RoomType/${roomType}`, data);
  },

  deleteRoomType: (roomTypeId) => {
    return axiosClient.delete(`/api/RoomType/${roomTypeId}`);
  },
};
