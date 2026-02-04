import axiosClient from "./axiosClient";

const optApi = {

    sendConfirm: (data) => {
        return axiosClient.post("/api/EmailConfirmation/send-confirmation", data);
    },
    confirmEmail: (token) => {
        return axiosClient.post(`/api/EmailConfirmation/confirm?token=${token}`);
    },
};

export default optApi;
