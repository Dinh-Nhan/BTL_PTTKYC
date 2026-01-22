import { useState } from "react";

import {
  bookings as initialBookings,
  rooms,
  type Booking,
} from "@/lib/mock-data";
import { toast } from "sonner";
import Header from "@/components/admin/layout/header";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import BookingsTable from "@/components/admin/bookings/bookings-table";
import CreateBookingModal from "@/components/admin/bookings/create-booking-modal";
import BookingDetailsModal from "@/components/admin/bookings/booking-details-modal";
import ConfirmDialog from "@/components/admin/shared/confirm-dialog";

const BookingPage = () => {
  const [bookingsList, setBookingsList] = useState<Booking[]>(initialBookings);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [viewingBooking, setViewingBooking] = useState<Booking | null>(null);
  const [cancellingBooking, setCancellingBooking] = useState<Booking | null>(
    null,
  );

  const handleCreateBooking = (newBooking: Booking) => {
    setBookingsList((prev) => [newBooking, ...prev]);
    setShowCreateModal(false);
    toast.success("Booking created successfully");
  };

  const handleCancelBooking = () => {
    if (!cancellingBooking) return;

    setBookingsList((prev) =>
      prev.map((b) =>
        b.id === cancellingBooking.id
          ? { ...b, status: "cancelled" as const }
          : b,
      ),
    );
    setCancellingBooking(null);
    toast.success("Booking cancelled successfully");
  };

  const handleUpdateDeposit = (bookingId: string, deposit: number) => {
    setBookingsList((prev) =>
      prev.map((b) => (b.id === bookingId ? { ...b, deposit } : b)),
    );
    toast.success("Deposit updated successfully");
  };

  const availableRooms = rooms.filter((r) => r.status === "available");

  return (
    <div className="flex flex-col h-full">
      <Header title="Booking Management" />
      <div className="flex-1 p-6 space-y-4">
        <div className="flex justify-end">
          <Button onClick={() => setShowCreateModal(true)}>
            <Plus className="w-4 h-4 mr-2" />
            Tạo đơn đặt phòng
          </Button>
        </div>

        <BookingsTable
          bookings={bookingsList}
          onView={setViewingBooking}
          onCancel={setCancellingBooking}
        />
      </div>

      <CreateBookingModal
        open={showCreateModal}
        onOpenChange={setShowCreateModal}
        onSave={handleCreateBooking}
        availableRooms={availableRooms}
      />

      <BookingDetailsModal
        booking={viewingBooking}
        open={!!viewingBooking}
        onOpenChange={(open) => !open && setViewingBooking(null)}
        onUpdateDeposit={handleUpdateDeposit}
      />

      <ConfirmDialog
        open={!!cancellingBooking}
        onOpenChange={(open) => !open && setCancellingBooking(null)}
        title="Hủy đặt phòng"
        description={`Bạn có chắc muốn hủy đặt phòng ${cancellingBooking?.id}? Thao tác này không thể khôi phục.`}
        onConfirm={handleCancelBooking}
        confirmText="Xác nhận"
        variant="destructive"
      />
    </div>
  );
};

export default BookingPage;
