import { Badge } from "@/components/ui/badge";
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

import { useEffect, useState } from "react";

// import { Booking } from "@/types/booking";

interface Booking {
  bookingId: number;
  checkInDatetime: string;
  checkOutDatetime: string;
  depositAmount: number;
  status: string;

  roomResponse: {
    roomNumber: string;
  };

  client?: {
    fullName: string;
  };
}

/* ================= PROPS ================= */

interface BookingDetailsModalProps {
  booking: Booking | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onUpdateDeposit: (bookingId: number, deposit: number) => void;
}

/* ================= STYLE ================= */

const statusStyles: Record<string, string> = {
  pending: "bg-warning/10 text-warning border-warning/20",
  confirmed: "bg-success/10 text-success border-success/20",
  cancelled: "bg-destructive/10 text-destructive border-destructive/20",
};

/* ================= COMPONENT ================= */

const BookingDetailsModal = ({
  booking,
  open,
  onOpenChange,
  onUpdateDeposit,
}: BookingDetailsModalProps) => {
  const [deposit, setDeposit] = useState(0);
  const [loading, setLoading] = useState(false);

  /* Sync deposit when booking changes */
  useEffect(() => {
    if (booking) {
      setDeposit(booking.depositAmount);
    }
  }, [booking]);

  if (!booking) return null;

  /* ================= HANDLER ================= */

  const handleUpdateDeposit = async () => {
    if (!booking) return;

    try {
      setLoading(true);

      await onUpdateDeposit(booking.bookingId, deposit);

      onOpenChange(false); // đóng modal
    } catch (error) {
      console.error(error);
      alert("Cập nhật thất bại");
    } finally {
      setLoading(false);
    }
  };


  /* ================= RENDER ================= */

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">

        {/* HEADER */}
        <DialogHeader>
          <DialogTitle className="flex items-center gap-3">
            Booking #{booking.bookingId}

            <Badge
              variant="outline"
              className={statusStyles[booking.status]}
            >
              {booking.status.toUpperCase()}
            </Badge>
          </DialogTitle>
        </DialogHeader>

        {/* CONTENT */}
        <div className="space-y-4 py-4">

          {/* CUSTOMER + ROOM */}
          <div className="grid grid-cols-2 gap-4">

            <div>
              <Label className="text-xs text-muted-foreground">
                Khách hàng
              </Label>

              <p className="font-medium">
                {booking.client?.fullName || "Khách lẻ"}
              </p>
            </div>

            <div>
              <Label className="text-xs text-muted-foreground">
                Phòng
              </Label>

              <p className="font-medium">
                {booking.roomResponse?.roomNumber}
              </p>
            </div>

          </div>

          {/* CHECK IN / OUT */}
          <div className="grid grid-cols-2 gap-4">

            <div>
              <Label className="text-xs text-muted-foreground">
                Check In
              </Label>

              <p className="font-medium">
                {new Date(
                  booking.checkInDatetime
                ).toLocaleDateString("vi-VN")}
              </p>
            </div>

            <div>
              <Label className="text-xs text-muted-foreground">
                Check Out
              </Label>

              <p className="font-medium">
                {new Date(
                  booking.checkOutDatetime
                ).toLocaleDateString("vi-VN")}
              </p>
            </div>

          </div>

          {/* DEPOSIT */}
          {booking.status !== "cancelled" && (

            <div className="p-4 border rounded-lg bg-muted/30 space-y-3">

              <Label>Cập nhật tiền cọc (VND)</Label>

              <div className="flex gap-2">

                <Input
                  type="number"
                  value={deposit}
                  onChange={(e) =>
                    setDeposit(Number(e.target.value))
                  }
                />

                <Button
                  onClick={handleUpdateDeposit}
                  disabled={loading}
                >
                  {loading ? "..." : "Cập nhật"}
                </Button>

              </div>

              <p className="text-xs text-muted-foreground">
                Hiện tại: {booking.depositAmount.toLocaleString()} VND
              </p>

            </div>
          )}

        </div>

        {/* FOOTER */}
        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
          >
            Đóng
          </Button>
        </DialogFooter>

      </DialogContent>
    </Dialog>
  );
};

export default BookingDetailsModal;
