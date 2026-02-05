import { useEffect, useState } from "react";
import { Card } from "../ui/card";
import { X, Loader2 } from "lucide-react";
import bookingApi from "@/api/bookingApi";
import { useNavigate, useSearchParams } from "react-router-dom";
import otpApi from "@/api/otpApi";
import Swal from "sweetalert2";

const BookingModal = ({ room, onClose }) => {
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

  const [isProcessing, setIsProcessing] = useState(false);
  const [hasProcessedVerification, setHasProcessedVerification] = useState(false);

  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const nights =
    checkIn && checkOut
      ? Math.ceil(
        (new Date(checkOut).getTime() - new Date(checkIn).getTime()) /
        (1000 * 60 * 60 * 24),
      ) 
      : 1;

  const totalPrice = room.price * nights * 0.1 ;

  const isValidEmail = (email: string) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const isValidPhone = (phone: string) => {
    const phoneRegex = /^(0|\+84)(3|5|7|8|9)[0-9]{8}$/;
    return phoneRegex.test(phone.replace(/\s/g, ""));
  };

  const isValidDates = () => {
    if (!checkIn || !checkOut) return false;
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const checkInDate = new Date(checkIn);
    const checkOutDate = new Date(checkOut);

    if (checkInDate < today) return false;
    if (checkOutDate <= checkInDate) return false;

    return true;
  };

  const isStep1Valid = isValidDates();
  const isStep2Valid =
    fullName.trim() !== "" &&
    isValidEmail(email) &&
    isValidPhone(phoneNumber) &&
    parseInt(adult) > 0;
  const isStep3Valid =
    cardNumber.replace(/\s/g, "").length >= 13 &&
    cardExpiry.match(/^(0[1-9]|1[0-2])\/\d{2}$/) !== null &&
    cardCvc.length >= 3;

  // 🔥 Xử lý email verification callback
  useEffect(() => {
    // Chỉ xử lý 1 lần
    if (hasProcessedVerification) return;

    const emailVerified = searchParams.get("emailVerified");
    const draftId = searchParams.get("draftId");
    const verifiedEmail = searchParams.get("email");
    const roomIdParam = searchParams.get("roomId");

    if (emailVerified === "1" && draftId && roomIdParam === room.id.toString()) {
      setHasProcessedVerification(true);
      handleEmailVerified(draftId, verifiedEmail);
    }
  }, [searchParams, room.id, hasProcessedVerification]);

  // Xử lý sau khi email được verify
  const handleEmailVerified = async (draftId: string, verifiedEmail: string) => {
    try {
      setIsProcessing(true);

      Swal.fire({
        title: "Email đã xác nhận!",
        html: "Đang chuyển đến trang thanh toán...",
        allowOutsideClick: false,
        didOpen: () => {
          Swal.showLoading();
        },
      });

      // Lấy thông tin draft
      const draftRes = await bookingApi.getDraft(draftId);
      const draft = draftRes.data;

      console.log("Draft data:", draft);

      // Cập nhật lại form từ draft
      setEmail(draft.email);
      setFullName(draft.fullName);
      setPhoneNumber(draft.phoneNumber);
      setCheckIn(new Date(draft.checkIn).toISOString().split('T')[0]);
      setCheckOut(new Date(draft.checkOut).toISOString().split('T')[0]);
      setAdult(draft.adult.toString());
      setChildren(draft.child.toString());

      // Tạo booking
      const response = await bookingApi.createBooking({
        roomId: draft.roomId,
        client: {
          fullName: draft.fullName,
          phoneNumber: draft.phoneNumber,
          email: draft.email,
        },
        checkInDatetime: draft.checkIn,
        checkOutDatetime: draft.checkOut,
        adultCount: draft.adult,
        childCount: draft.child,
        note: "",
      });

      console.log("Booking response:", response.data);

      // Redirect đến VNPay
      if (response.data.result?.paymentUrl) {
        window.location.href = response.data.result.paymentUrl;
      } else {
        throw new Error("Không nhận được payment URL");
      }
    } catch (error: any) {
      console.error("Error processing booking:", error);

      Swal.fire({
        icon: "error",
        title: "Có lỗi xảy ra",
        text: error.response?.data?.message || error.message || "Vui lòng thử lại",
        confirmButtonColor: "#dc2626",
      });

      setIsProcessing(false);
    }
  };

  const handleNext = () => {
    if (step === "dates") {
      if (!isStep1Valid) {
        Swal.fire({
          icon: "warning",
          title: "Vui lòng chọn ngày hợp lệ",
          text: "Check-in phải từ hôm nay và check-out phải sau check-in",
          confirmButtonColor: "#ff6b35",
        });
        return;
      }
      setStep("details");
    } else if (step === "details") {
      if (!fullName.trim()) {
        Swal.fire({
          icon: "warning",
          title: "Thiếu thông tin",
          text: "Vui lòng nhập họ tên",
          confirmButtonColor: "#ff6b35",
        });
        return;
      }

      if (!isValidEmail(email)) {
        Swal.fire({
          icon: "warning",
          title: "Email không hợp lệ",
          text: "Vui lòng nhập đúng định dạng email",
          confirmButtonColor: "#ff6b35",
        });
        return;
      }

      if (!isValidPhone(phoneNumber)) {
        Swal.fire({
          icon: "warning",
          title: "Số điện thoại không hợp lệ",
          text: "Vui lòng nhập số điện thoại Việt Nam hợp lệ",
          confirmButtonColor: "#ff6b35",
        });
        return;
      }

      setStep("payment");
    }
  };

  const handleSubmit = async () => {
    if (!isStep3Valid) {
      Swal.fire({
        icon: "warning",
        title: "Thông tin thanh toán không đầy đủ",
        confirmButtonColor: "#ff6b35",
      });
      return;
    }

    try {
      setIsProcessing(true);

      Swal.fire({
        title: "Đang xử lý...",
        html: "Vui lòng đợi trong giây lát",
        allowOutsideClick: false,
        didOpen: () => {
          Swal.showLoading();
        },
      });

      // 1️⃣ Tạo draft với đầy đủ thông tin
      const draftData = {
        roomId: room.id,
        email,
        fullName,
        phoneNumber,
        checkIn,
        checkOut,
        adult: parseInt(adult),
        child: parseInt(children),
      };

      console.log("Creating draft:", draftData);

      const draftRes = await bookingApi.createDraft(draftData);
      const draftId = draftRes.data.draftId;

      console.log("Draft created:", draftId);

      // 2️⃣ Gửi email xác nhận (bao gồm roomId)
      await otpApi.sendConfirm({
        draftId: draftId,
        email: email,
        roomId: room.id,
      });

      setIsProcessing(false);

      // 3️⃣ Thông báo kiểm tra email
      Swal.fire({
        icon: "info",
        title: "Vui lòng xác nhận email",
        html: `
          <p>Chúng tôi đã gửi email xác nhận đến:</p>
          <p class="font-bold text-lg mt-2">${email}</p>
          <p class="text-sm text-gray-600 mt-3">
            Click vào link trong email để tiếp tục thanh toán
          </p>
          <p class="text-xs text-gray-500 mt-2">
            ⏰ Email có hiệu lực trong 15 phút
          </p>
          <p class="text-xs text-orange-600 mt-3">
            💡 Sau khi click link, bạn sẽ quay lại trang này để tiếp tục
          </p>
        `,
        allowOutsideClick: false,
        confirmButtonText: "Đã hiểu",
        confirmButtonColor: "#ff6b35",
        showCancelButton: true,
        cancelButtonText: "Gửi lại email",
        cancelButtonColor: "#6b7280",
      }).then(async (result) => {
        if (result.isDismissed && result.dismiss === Swal.DismissReason.cancel) {
          try {
            await otpApi.sendConfirm({
              draftId: draftId,
              email: email,
              roomId: room.id,
            });

            Swal.fire({
              icon: "success",
              title: "Email đã được gửi lại",
              timer: 2000,
              showConfirmButton: false,
            });
          } catch (error) {
            Swal.fire({
              icon: "error",
              title: "Gửi email thất bại",
              text: "Vui lòng thử lại",
              confirmButtonColor: "#dc2626",
            });
          }
        }
      });

    } catch (error: any) {
      console.error("Error in handleSubmit:", error);
      setIsProcessing(false);

      Swal.fire({
        icon: "error",
        title: "Có lỗi xảy ra",
        text: error.response?.data?.message || "Vui lòng thử lại",
        confirmButtonColor: "#dc2626",
      });
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-foreground/20 backdrop-blur-sm p-4 overflow-y-auto scrollbar-hide">
      <Card className="border-border bg-card w-full max-w-2xl shadow-lg">
        {/* Header */}
        <div className="flex items-center justify-between border-b border-border p-6">
          <h2 className="text-xl font-medium text-foreground">
            Book {room.name}
          </h2>
          <button
            onClick={onClose}
            disabled={isProcessing}
            className="rounded-full p-1 hover:bg-secondary transition-colors disabled:opacity-50"
          >
            <X className="h-5 w-5 text-muted-foreground" />
          </button>
        </div>

        {/* Steps Indicator */}
        <div className="flex border-b border-border px-6 py-4">
          {["dates", "details", "payment"].map((stepName, idx) => (
            <div key={stepName} className="flex items-center">
              <div
                className={`flex h-8 w-8 items-center justify-center rounded-full text-xs font-medium transition-colors ${step === stepName
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
                    min={new Date().toISOString().split("T")[0]}
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
                    min={checkIn || new Date().toISOString().split("T")[0]}
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
                    Họ tên đầy đủ <span className="text-red-500">*</span>
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
                    Email <span className="text-red-500">*</span>
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
                    Số điện thoại <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="text"
                    value={phoneNumber}
                    onChange={(e) => setPhoneNumber(e.target.value)}
                    placeholder="0912345678"
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
                    maxLength={16}
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
                      onChange={(e) => {
                        let value = e.target.value.replace(/\D/g, "");
                        if (value.length >= 2) {
                          value = value.slice(0, 2) + "/" + value.slice(2, 4);
                        }
                        setCardExpiry(value);
                      }}
                      placeholder="MM/YY"
                      maxLength={5}
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
                      onChange={(e) =>
                        setCardCvc(e.target.value.replace(/\D/g, ""))
                      }
                      placeholder="123"
                      maxLength={4}
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
                {room.price?.toLocaleString() || 0}VNĐ × {nights} đêm
              </span>
              <span className="text-foreground font-medium">
                {totalPrice.toLocaleString()}VNĐ
              </span>
            </div>
            <div className="flex justify-between border-t border-border pt-2">
              <span className="font-medium text-foreground">Tổng cộng</span>
              <span className="text-2xl font-light text-foreground">
                {totalPrice.toLocaleString()}VNĐ
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
              disabled={isProcessing}
              className="flex-1 rounded-md border border-input px-4 py-2.5 text-sm font-medium text-foreground hover:bg-secondary transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Trở lại
            </button>
          )}
          <button
            onClick={step === "payment" ? handleSubmit : handleNext}
            disabled={
              (step === "dates" && !isStep1Valid) ||
              (step === "details" && !isStep2Valid) ||
              (step === "payment" && (!isStep3Valid || isProcessing))
            }
            className="flex-1 rounded-md bg-accent px-4 py-2.5 text-sm font-medium
                      text-accent-foreground hover:bg-accent/90
                      disabled:opacity-50 disabled:cursor-not-allowed transition-colors
                      flex items-center justify-center gap-2"
          >
            {isProcessing && <Loader2 className="h-4 w-4 animate-spin" />}
            {step !== "payment" && "Tiếp tục"}
            {step === "payment" &&
              (isProcessing ? "Đang xử lý..." : "Xác nhận đặt phòng")}
          </button>
        </div>
      </Card>
    </div>
  );
};

export default BookingModal;