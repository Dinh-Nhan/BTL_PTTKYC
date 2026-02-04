import axiosClient from "../axiosClient";

export const paymentApi = {
  vnpayReturn: () => {
    return axiosClient.get("/api/payment/vnpay-return");
  },

  vnpayIpn: () => {
    return axiosClient.get("/api/payment/vnpay-ipn");
  },
};
