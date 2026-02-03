import axiosClient from "./axiosClient";

const billApi = {
    getAll: () => {
        return axiosClient.get("/api/bill");
    },
}

export default billApi;