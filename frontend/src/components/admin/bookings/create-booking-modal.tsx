import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { formatVND } from "@/lib/format";
import { useEffect, useState } from "react";

import roomApi from "@/api/roomApi";
import bookingApi from "@/api/bookingApi";

/* ================= TYPES ================= */

interface Room {
  roomId: number;
  roomNumber: string;
  floor: number;
  status: string;
  note: string;

  roomType: {
    roomTypeId: number;
    typeName: string;
    basePrice: number;
  };
}

interface CreateBookingModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

/* ================= COMPONENT ================= */

const CreateBookingModal = ({
  open,
  onOpenChange,
}: CreateBookingModalProps) => {
  const [loading, setLoading] = useState(false);

  /* ===== ROOMS ===== */

  const [rooms, setRooms] = useState<Room[]>([]);

  useEffect(() => {
    if (!open) return;

    const fetchRooms = async () => {
      try {
        const res = await roomApi.getAvailable();
        setRooms(res.data.result);
      } catch (err) {
        console.error("Load rooms error:", err);
      }
    };

    fetchRooms();
  }, [open]);

  /* ===== FORM ===== */

  const [formData, setFormData] = useState({
    fullName: "",
    phoneNumber: "",
    email: "",

    roomId: "",

    checkIn: "",
    checkOut: "",

    adultCount: 1,
    childCount: 0,

    note: "",
  });

  /* ===== SELECTED ROOM ===== */

  const selectedRoom = rooms.find(
    (r) => r.roomId === Number(formData.roomId)
  );

  /* ===== TOTAL ===== */

  const calculateTotal = () => {
    if (!selectedRoom || !formData.checkIn || !formData.checkOut) return 0;

    const inDate = new Date(formData.checkIn);
    const outDate = new Date(formData.checkOut);

    const nights = Math.ceil(
      (outDate.getTime() - inDate.getTime()) /
        (1000 * 60 * 60 * 24)
    );

    if (nights <= 0) return 0;

    return nights * selectedRoom.roomType.basePrice;
  };

  /* ===== SAVE ===== */

  const handleSave = async () => {
    if (!isValid) return;

    if (new Date(formData.checkOut) <= new Date(formData.checkIn)) {
      alert("Check-out phải sau Check-in");
      return;
    }

    try {
      setLoading(true);

      const payload = {
        roomId: Number(formData.roomId),

        client: {
          fullName: formData.fullName.trim(),
          phoneNumber: formData.phoneNumber.trim(),
          email: formData.email.trim(),
        },

        checkInDatetime: `${formData.checkIn}T00:00:00`,
        checkOutDatetime: `${formData.checkOut}T00:00:00`,

        adultCount: Number(formData.adultCount),
        childCount: Number(formData.childCount),

        note: formData.note,
      };

      console.log("SEND BOOKING:", payload);

      await bookingApi.createBooking(payload);

      alert("Đặt phòng thành công!");

      onOpenChange(false);

      // Reset form
      setFormData({
        fullName: "",
        phoneNumber: "",
        email: "",

        roomId: "",

        checkIn: "",
        checkOut: "",

        adultCount: 1,
        childCount: 0,

        note: "",
      });
    } catch (err: any) {
      console.error("Create booking error:", err.response?.data || err);
      alert("Đặt phòng thất bại!");
    } finally {
      setLoading(false);
    }
  };

  /* ===== VALID ===== */

  const isValid =
    formData.fullName &&
    formData.phoneNumber &&
    formData.roomId &&
    formData.checkIn &&
    formData.checkOut &&
    formData.adultCount > 0 &&
    calculateTotal() > 0;

  /* ===== DATE ===== */

  const minCheckIn = new Date();
  minCheckIn.setDate(minCheckIn.getDate() + 3);

  const minCheckInDate = minCheckIn
    .toISOString()
    .split("T")[0];


  /* ================= UI ================= */

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">

        <DialogHeader>
          <DialogTitle>Tạo đặt phòng mới</DialogTitle>
        </DialogHeader>

        <div className="space-y-4 py-4">

          {/* CUSTOMER */}

          <div className="space-y-2">
            <Label>Họ tên</Label>
            <Input
              value={formData.fullName}
              onChange={(e) =>
                setFormData({ ...formData, fullName: e.target.value })
              }
            />
          </div>

          <div className="space-y-2">
            <Label>Số điện thoại</Label>
            <Input
              value={formData.phoneNumber}
              onChange={(e) =>
                setFormData({ ...formData, phoneNumber: e.target.value })
              }
            />
          </div>

          <div className="space-y-2">
            <Label>Email</Label>
            <Input
              value={formData.email}
              onChange={(e) =>
                setFormData({ ...formData, email: e.target.value })
              }
            />
          </div>

          {/* ROOM */}

          <div className="space-y-2">
            <Label>Phòng</Label>

            <Select
              value={formData.roomId}
              onValueChange={(v) =>
                setFormData({ ...formData, roomId: v })
              }
            >
              <SelectTrigger>
                <SelectValue placeholder="Chọn phòng" />
              </SelectTrigger>

              <SelectContent>
                {rooms.map((room) => (
                  <SelectItem
                    key={room.roomId}
                    value={room.roomId.toString()}
                  >
                    Phòng {room.roomNumber} - {room.roomType.typeName} (
                    {formatVND(room.roomType.basePrice)})
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {/* DATE */}

          <div className="grid grid-cols-2 gap-4">

            <div>
              <Label>Check In</Label>
              <Input
                type="date"
                min={minCheckInDate}
                value={formData.checkIn}
                onChange={(e) =>
                  setFormData({ ...formData, checkIn: e.target.value })
                }
              />
            </div>

            <div>
              <Label>Check Out</Label>
              <Input
                type="date"
                min={formData.checkIn || minCheckInDate}
                value={formData.checkOut}
                onChange={(e) =>
                  setFormData({ ...formData, checkOut: e.target.value })
                }
              />
            </div>

          </div>

          {/* PEOPLE */}

          <div className="grid grid-cols-2 gap-4">

            <div>
              <Label>Người lớn</Label>
              <Input
                type="number"
                min={1}
                value={formData.adultCount}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    adultCount: Number(e.target.value),
                  })
                }
              />
            </div>

            <div>
              <Label>Trẻ em</Label>
              <Input
                type="number"
                min={0}
                value={formData.childCount}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    childCount: Number(e.target.value),
                  })
                }
              />
            </div>

          </div>

          {/* NOTE */}

          <div>
            <Label>Ghi chú</Label>
            <Input
              value={formData.note}
              onChange={(e) =>
                setFormData({ ...formData, note: e.target.value })
              }
            />
          </div>

          {/* TOTAL */}

          {calculateTotal() > 0 && (
            <div className="p-3 bg-muted/50 rounded">

              <div className="flex justify-between">

                <span>Tổng tiền:</span>

                <span className="font-semibold">
                  {formatVND(calculateTotal())}
                </span>

              </div>

            </div>
          )}
        </div>

        {/* FOOTER */}

        <DialogFooter>

          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
          >
            Hủy
          </Button>

          <Button
            disabled={!isValid || loading}
            onClick={handleSave}
          >
            {loading ? "Đang tạo..." : "Tạo"}
          </Button>

        </DialogFooter>

      </DialogContent>
    </Dialog>
  );
};

export default CreateBookingModal;
