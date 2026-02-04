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

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { useEffect, useState } from "react";

/* ================= TYPES ================= */

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

  onUpdateDeposit: (bookingId: number, deposit: number) => Promise<void>;

  onUpdateStatus: (
    bookingId: number,
    status: string
  ) => Promise<void>;
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
  onUpdateStatus,
}: BookingDetailsModalProps) => {
  const [deposit, setDeposit] = useState(0);
  const [status, setStatus] = useState("");

  const [loadingDeposit, setLoadingDeposit] = useState(false);
  const [loadingStatus, setLoadingStatus] = useState(false);

  /* Sync data */
  useEffect(() => {
    if (booking) {
      setDeposit(booking.depositAmount);
      setStatus(booking.status);
    }
  }, [booking]);

  if (!booking) return null;

  /* ================= HANDLER ================= */

  const handleUpdateDeposit = async () => {
    try {
      setLoadingDeposit(true);

      await onUpdateDeposit(booking.bookingId, deposit);

      onOpenChange(false);
    } catch (error) {
      console.error(error);
      alert("Cập nhật tiền cọc thất bại");
    } finally {
      setLoadingDeposit(false);
    }
  };

  const handleUpdateStatus = async () => {
    try {
      setLoadingStatus(true);

      await onUpdateStatus(booking.bookingId, status);

      onOpenChange(false);
    } catch (error) {
      console.error(error);
      alert("Cập nhật trạng thái thất bại");
    } finally {
      setLoadingStatus(false);
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

          {/* UPDATE STATUS */}
          {booking.status !== "cancelled" && (

            <div className="p-4 border rounded-lg bg-muted/30 space-y-3">

              <Label>Cập nhật trạng thái</Label>

              <div className="flex gap-2">

                <Select
                  value={status}
                  onValueChange={setStatus}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>

                  <SelectContent>

                    <SelectItem value="pending">
                      Pending
                    </SelectItem>

                    <SelectItem value="unpaid">
                      Unpaid
                    </SelectItem>

                    <SelectItem value="cancelled">
                      Cancelled
                    </SelectItem>

                    <SelectItem value="paid">
                      Paid
                    </SelectItem>

                    <SelectItem value="check-out">
                      Check-out
                    </SelectItem>

                  </SelectContent>
                </Select>

                <Button
                  onClick={handleUpdateStatus}
                  disabled={loadingStatus}
                >
                  {loadingStatus ? "..." : "Cập nhật"}
                </Button>

              </div>

            </div>
          )}

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
                  disabled={loadingDeposit}
                >
                  {loadingDeposit ? "..." : "Cập nhật"}
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
