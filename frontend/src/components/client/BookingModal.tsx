import { useState } from "react";
import { Card } from "../ui/card";
import { X } from "lucide-react";
import bookingApi from "@/api/bookingApi";
import { useNavigate } from "react-router-dom";
// interface BookingModalProps {
//   room: {
//     id: number;
//     name: string;
//     price: number;
//     image: string;
//   };
//   onClose: () => void;
// }
const BookingModal = ({ room, onClose }) => {
  // console.log("room: ", room);
  const [step, setStep] = useState<"dates" | "details" | "payment">("dates");
  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");
  const [adult, setAdult] = useState("1");
  const [children, setChildren] = useState("0");
  const [email, setEmail] = useState("");
  const [fullName, setFullName] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [cardNumber, setCardNumber] = useState("");
  const [cardExpiry, setCardExpiry] = useState("");
  const [cardCvc, setCardCvc] = useState("");
  const navigate = useNavigate();
  // Tính số đêm đặt phòng
  const nights =
    checkIn && checkOut // Nếu = 0 tức là người dùng chưa chọn ngày
      ? Math.ceil(
          (new Date(checkOut).getTime() - new Date(checkIn).getTime()) /
            (1000 * 60 * 60 * 24),
        )
      : 1;
  // Tính tổng giá tiền
  const totalPrice = room.price * nights;

  // Xử lý chuyển step
  const handleNext = () => {
    if (step === "dates") {
      setStep("details");
    } else if (step === "details") {
      setStep("payment");
    }
  };

  //Xử lý xác nhận đặt phòng
  //TODO: Thực hiện thanh toán bằng NVPay hoặc Momo
  const handleSubmit = () => {
    alert(
      "Booking confirmed! Order ID: #" +
        Math.random().toString(36).substr(2, 9).toUpperCase(),
    );
    const response = bookingApi.createBooking({
      roomId: room.id,
      client: {
        fullName: fullName,
        phoneNumber: phoneNumber,
        email: email,
      },
      checkInDatetime: checkIn,
      checkOutDatetime: checkOut,
      adultCount: parseInt(adult),
      childCount: parseInt(children),
      note : ""
    }).then((res) => {
      console.log("Booking response: ", res.data);
      
      if(res.data.result!=null){
        navigate(res.data.result.paymentUrl);
      }
    }).catch((err) => {
      console.error("Booking error: ", err);
    });


    onClose();
  };

  //   Tạo biến kiểm tra tính hợp lẹ của từng bước
  const isStep1Valid = checkIn && checkOut;
  const isStep2Valid = fullName && email && adult;
  const isStep3Valid = cardNumber && cardExpiry && cardCvc;
  return (
    <>
      <div className="fixed inset-0 z-50 flex items-center justify-center bg-foreground/20 backdrop-blur-sm p-4 overflow-y-auto scrollbar-hide">
        <Card className="border-border bg-card w-full max-w-2xl shadow-lg">
          {/* Header */}
          <div className="flex items-center justify-between border-b border-border p-6">
            <h2 className="text-xl font-medium text-foreground">
              Book {room.name}
            </h2>
            <button
              onClick={onClose}
              className="rounded-full p-1 hover:bg-secondary transition-colors"
            >
              <X className="h-5 w-5 text-muted-foreground" />
            </button>
          </div>

          {/* Steps Indicator */}
          <div className="flex border-b border-border px-6 py-4">
            {["dates", "details", "payment"].map((stepName, idx) => (
              <div key={stepName} className="flex items-center">
                <div
                  className={`flex h-8 w-8 items-center justify-center rounded-full text-xs font-medium transition-colors ${
                    step === stepName
                      ? "bg-accent text-accent-foreground"
                      : ["dates", "details"].includes(stepName) &&
                          ["dates", "details", "payment"].indexOf(step) > idx
                        ? "bg-secondary text-foreground"
                        : "bg-secondary text-muted-foreground"
                  }`}
                >
                  {idx + 1}
                </div>
                {idx < 2 && <div className="mx-2 h-0.5 w-12 bg-border" />}
              </div>
            ))}
          </div>

          {/* Content */}
          <div className="p-6">
            {/* Step 1: Select Dates */}
            {step === "dates" && (
              <div className="space-y-4">
                <h3 className="text-lg font-medium text-foreground">
                  Chọn ngày nhận và trả phòng
                </h3>
                <div className="grid gap-4 sm:grid-cols-2">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-foreground">
                      Check-in
                    </label>
                    <input
                      type="date"
                      value={checkIn}
                      onChange={(e) => setCheckIn(e.target.value)}
                      className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                    />
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-foreground">
                      Check-out
                    </label>
                    <input
                      type="date"
                      value={checkOut}
                      onChange={(e) => setCheckOut(e.target.value)}
                      className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                    />
                  </div>
                </div>
              </div>
            )}

            {/* Step 2: Guest Details */}
            {step === "details" && (
              <div className="space-y-4">
                <h3 className="text-lg font-medium text-foreground">
                  Thông tin khách hàng
                </h3>
                <div className="space-y-3">
                  <div>
                    <label className="text-sm font-medium text-foreground">
                      Họ tên đầy đủ
                    </label>
                    <input
                      type="text"
                      value={fullName}
                      onChange={(e) => setFullName(e.target.value)}
                      placeholder="Nguyễn Văn A"
                      className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium text-foreground">
                      Email
                    </label>
                    <input
                      type="email"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      placeholder="nguyenvana@example.com"
                      className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium text-foreground">
                      Số điện thoại
                    </label>
                    <input
                      type="text"
                      value={phoneNumber}
                      onChange={(e) => setPhoneNumber(e.target.value)}
                      placeholder="0123456789"
                      className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium text-foreground">
                      Số lượng người lớn
                    </label>
                    <select
                      value={adult}
                      onChange={(e) => setAdult(e.target.value)}
                      className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                    >
                      {[1, 2, 3, 4, 5, 6].map((num) => (
                        <option key={num} value={num}>
                          {num} người lớn
                        </option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="text-sm font-medium text-foreground">
                      Số lượng trẻ em
                    </label>
                    <select
                      value={children}
                      onChange={(e) => setChildren(e.target.value)}
                      className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                    >
                      {[0, 1, 2, 3, 4, 5, 6].map((num) => (
                        <option key={num} value={num}>
                          {num} trẻ em
                        </option>
                      ))}
                    </select>
                  </div>
                </div>
              </div>
            )}

            {/* Step 3: Payment */}
            {step === "payment" && (
              <div className="space-y-4">
                <h3 className="text-lg font-medium text-foreground">
                  Thông tin thanh toán
                </h3>
                <div className="space-y-3">
                  <div>
                    <label className="text-sm font-medium text-foreground">
                      Số thẻ
                    </label>
                    <input
                      type="text"
                      value={cardNumber}
                      onChange={(e) =>
                        setCardNumber(e.target.value.replace(/\s/g, ""))
                      }
                      placeholder="1234 5678 9012 3456"
                      className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                    />
                  </div>
                  <div className="grid gap-3 sm:grid-cols-2">
                    <div>
                      <label className="text-sm font-medium text-foreground">
                        Ngày hết hạn
                      </label>
                      <input
                        type="text"
                        value={cardExpiry}
                        onChange={(e) => setCardExpiry(e.target.value)}
                        placeholder="MM/YY"
                        className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                      />
                    </div>
                    <div>
                      <label className="text-sm font-medium text-foreground">
                        CVC
                      </label>
                      <input
                        type="text"
                        value={cardCvc}
                        onChange={(e) => setCardCvc(e.target.value)}
                        placeholder="123"
                        className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-ring"
                      />
                    </div>
                  </div>
                </div>
              </div>
            )}
          </div>

          {/* Summary */}
          <div className="border-t border-border bg-secondary/30 p-6">
            <div className="space-y-2">
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground">
                  {room.price}VNĐ × {nights} đêm
                </span>
                <span className="text-foreground font-medium">
                  {totalPrice}VNĐ
                </span>
              </div>
              <div className="flex justify-between border-t border-border pt-2">
                <span className="font-medium text-foreground">Tổng cộng</span>
                <span className="text-2xl font-light text-foreground">
                  {totalPrice}VNĐ
                </span>
              </div>
            </div>
          </div>

          {/* Actions */}
          <div className="border-t border-border p-6 flex gap-3">
            {step !== "dates" && (
              <button
                onClick={() => {
                  if (step === "details") setStep("dates");
                  if (step === "payment") setStep("details");
                }}
                className="flex-1 rounded-md border border-input px-4 py-2.5 text-sm font-medium text-foreground hover:bg-secondary transition-colors"
              >
                Trở lại
              </button>
            )}
            <button
              onClick={step === "payment" ? handleSubmit : handleNext}
              disabled={
                (step === "dates" && !isStep1Valid) ||
                (step === "details" && !isStep2Valid) ||
                (step === "payment" && !isStep3Valid)
              }
              className="flex-1 rounded-md bg-accent px-4 py-2.5 text-sm font-medium text-accent-foreground hover:bg-accent/90 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {step === "payment" ? "Xác nhận đặt phòng" : "Tiếp tục"}
            </button>
          </div>
        </Card>
      </div>
    </>
  );
};

export default BookingModal;
