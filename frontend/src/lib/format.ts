export const formatVND = (value: number) =>
  new Intl.NumberFormat("vi-VN", {
    style: "decimal",
    maximumFractionDigits: 0,
  }).format(value) + " VNĐ";
