import axiosClient from "./axiosClient";

export const bookingApi = {
  createBooking: (bookingData) => {
    return axiosClient.post("/api/Booking/create-booking", { ...bookingData });
  },

  getAllBookings: () => {
    return axiosClient.get("/api/booking");
  },

  getBookingById: (bookingId) => {
    return axiosClient.get(`/api/booking/${bookingId}`);
  },

  cancelBooking: (bookingId) => {
    return axiosClient.patch(`/api/booking/canel-booking/${bookingId}`);
  },

  querydrBooking: (bookingId) => {
    return axiosClient.get(`/api/booking/querydr/${bookingId}`);
  },
};

export default bookingApi;
