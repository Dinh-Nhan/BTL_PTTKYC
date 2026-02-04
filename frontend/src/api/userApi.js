import axiosClient from "./axiosClient";

const userApi = {
  getAll: () => {
    return axiosClient.get("/api/User");
  },

  addUser: (data) => {
    return axiosClient.post("/api/user", data);
  },

  searchUserByInfo: (info) => {
    return axiosClient.get("/api/user/search", { params: { info } });
  },

  searchEmployee: (info) => {
    return axiosClient.get("/api/user/search-employee", { params: { info } });
  },

  deleteUser: (userId) => {
    return axiosClient.delete(`/api/user/${userId}`);
  },

  editUser: (userId, data) => {
    return axiosClient.put(`/api/user/${userId}`, data);
  },

  deactiveUser: (userId) => {
    return axiosClient.patch(`/api/user/deactive/${userId}`);
  },

  activeUser: (userId) => {
    return axiosClient.patch(`/api/user/active/${userId}`);
  },
};
export default userApi;
