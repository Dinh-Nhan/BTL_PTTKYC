import { useEffect, useState } from "react";
import { toast } from "sonner";

import Header from "@/components/admin/layout/header";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";

import BookingsTable from "@/components/admin/bookings/bookings-table";
import CreateBookingModal from "@/components/admin/bookings/create-booking-modal";
import BookingDetailsModal from "@/components/admin/bookings/booking-details-modal";
import ConfirmDialog from "@/components/admin/shared/confirm-dialog";

import bookingApi from "@/api/booKingApi";

/* ================= TYPE ================= */

interface Booking {
  bookingId: number;
  checkInDatetime: string;
  checkOutDatetime: string;
  depositAmount: number;
  status: string;

  roomResponse: {
    roomNumber: string;
  };

  clientResponse?: {
    fullName: string;
  };
}

/* ================= COMPONENT ================= */

const BookingPage = () => {
  const [bookingsList, setBookingsList] = useState<Booking[]>([]);
  const [showCreateModal, setShowCreateModal] = useState(false);

  const [viewingBooking, setViewingBooking] =
    useState<Booking | null>(null);

  const [cancellingBooking, setCancellingBooking] =
    useState<Booking | null>(null);

  /* ================= FETCH ================= */

  const fetchBookings = async () => {
    try {
      const res = await bookingApi.getAll();

      setBookingsList(res.data.result || []);
    } catch (error) {
      console.log(error);
      toast.error("Không tải được danh sách booking");
    }
  };

  useEffect(() => {
    fetchBookings();
  }, []);

  /* ================= UPDATE STATUS ================= */

  const updateBookingStatus = async (
    bookingId: number,
    status: string
  ) => {
    try {
      await bookingApi.updateStatusBooking(bookingId, status);

      await fetchBookings();

      toast.success("Cập nhật trạng thái thành công");
    } catch (error) {
      console.log(error);
      toast.error("Cập nhật trạng thái thất bại");
    }
  };

  /* ================= HANDLER ================= */

  const handleCreateBooking = (newBooking: Booking) => {
    setBookingsList((prev) => [newBooking, ...prev]);

    setShowCreateModal(false);

    toast.success("Tạo booking thành công");
  };

  const handleCancelBooking = async () => {
    if (!cancellingBooking) return;

    try {
      await bookingApi.cancel(
        cancellingBooking.bookingId,
        "Admin hủy"
      );

      await fetchBookings();

      toast.success("Đã hủy booking");
    } catch (error) {
      console.log(error);
      toast.error("Hủy booking thất bại");
    }

    setCancellingBooking(null);
  };

  const handleUpdateDeposit = async (
    bookingId: number,
    deposit: number
  ) => {
    try {
      await bookingApi.updateDeposit(bookingId, deposit);

      await fetchBookings();

      toast.success("Cập nhật tiền cọc thành công");
    } catch (error) {
      console.log(error);
      toast.error("Cập nhật thất bại");
    }
  };

  /* ================= TEMP DATA ================= */

  const availableRooms: any[] = [];

  /* ================= RENDER ================= */

  return (
    <div className="flex flex-col h-full">
      <Header title="Booking Management" />

      <div className="flex-1 p-6 space-y-4">
        {/* BUTTON */}
        <div className="flex justify-end">
          <Button onClick={() => setShowCreateModal(true)}>
            <Plus className="w-4 h-4 mr-2" />
            Tạo đơn đặt phòng
          </Button>
        </div>

        {/* TABLE */}
        <BookingsTable
          bookings={bookingsList}
          onView={setViewingBooking}
          onCancel={setCancellingBooking}
        />
      </div>

      {/* CREATE MODAL */}
      <CreateBookingModal
        open={showCreateModal}
        onOpenChange={setShowCreateModal}
        onSave={handleCreateBooking}
        availableRooms={availableRooms}
      />

      {/* DETAIL MODAL */}
      <BookingDetailsModal
        booking={viewingBooking}
        open={!!viewingBooking}
        onOpenChange={(open) =>
          !open && setViewingBooking(null)
        }
        onUpdateDeposit={handleUpdateDeposit}
        onUpdateStatus={updateBookingStatus}
      />

      {/* CONFIRM CANCEL */}
      <ConfirmDialog
        open={!!cancellingBooking}
        onOpenChange={(open) =>
          !open && setCancellingBooking(null)
        }
        title="Hủy đặt phòng"
        description={`Bạn có chắc muốn hủy đặt phòng #${cancellingBooking?.bookingId}? Thao tác này không thể khôi phục.`}
        onConfirm={handleCancelBooking}
        confirmText="Xác nhận"
        variant="destructive"
      />
    </div>
  );
};

export default BookingPage;
