import axiosClient from "./axiosClient";

const bookingApi = {
  createBooking: (data) => {
    return axiosClient.post("/api/Booking/create-booking", data);
  },

  getAll: () => {
    return axiosClient.get("/api/Booking");
  },

  updateDeposit: (id, deposit) => {
    return axiosClient.patch(`/api/Booking/${id}/deposit`, {
      depositAmount: deposit,
    });
  },


  getById: (id) => {
    return axiosClient.get(`/api/Booking/${id}`);
  },

  cancel: (id, reason) => {
    return axiosClient.patch(`/api/Booking/cancel-booking/${id}`, {
      reason,
    });
  },

  queryTransaction: (id) => {
    return axiosClient.get(`/api/Booking/query-transaction/${id}`);
  },
};

export default bookingApi;
