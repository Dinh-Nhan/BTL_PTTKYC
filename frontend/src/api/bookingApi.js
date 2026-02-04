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
  createDraft: (draftData) => {
    return axiosClient.post("/api/Booking/draft", { ...draftData });
  },
  getDraft: (draftId) => {
    return axiosClient.get(`/api/Booking/draft/${draftId}`);
  },
  sendBookingEmail: (bookingId) => {
    return axiosClient.post("/api/EmailConfirmation/send-booking-email", { bookingId });
  }
};

export default bookingApi;
