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
import { customers, type Booking, type Room } from "@/lib/mock-data";
import { useState } from "react";

interface CreateBookingModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSave: (booking: Booking) => void;
  availableRooms: Room[];
}

const CreateBookingModal = ({
  open,
  onOpenChange,
  onSave,
  availableRooms,
}: CreateBookingModalProps) => {
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    customerId: "",
    roomId: "",
    checkIn: "",
    checkOut: "",
    deposit: 0,
  });

  const selectedRoom = availableRooms.find((r) => r.id === formData.roomId);
  const selectedCustomer = customers.find((c) => c.id === formData.customerId);

  const calculateTotal = () => {
    if (!selectedRoom || !formData.checkIn || !formData.checkOut) return 0;
    const checkIn = new Date(formData.checkIn);
    const checkOut = new Date(formData.checkOut);
    const nights = Math.ceil(
      (checkOut.getTime() - checkIn.getTime()) / (1000 * 60 * 60 * 24),
    );
    return nights > 0 ? nights * selectedRoom.price : 0;
  };

  const handleSave = async () => {
    if (
      !selectedRoom ||
      !selectedCustomer ||
      !formData.checkIn ||
      !formData.checkOut
    )
      return;

    setLoading(true);
    await new Promise((resolve) => setTimeout(resolve, 500));
    setLoading(false);

    const newBooking: Booking = {
      id: `B${String(Date.now()).slice(-4)}`,
      roomId: formData.roomId,
      roomNumber: selectedRoom.number,
      customerName: selectedCustomer.name,
      customerId: formData.customerId,
      checkIn: formData.checkIn,
      checkOut: formData.checkOut,
      status: "pending",
      totalAmount: calculateTotal(),
      deposit: formData.deposit,
      createdAt: new Date().toISOString().split("T")[0],
    };

    onSave(newBooking);
    setFormData({
      customerId: "",
      roomId: "",
      checkIn: "",
      checkOut: "",
      deposit: 0,
    });
  };

  const isValid =
    formData.customerId &&
    formData.roomId &&
    formData.checkIn &&
    formData.checkOut &&
    calculateTotal() > 0;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Tạo đặt phòng mới</DialogTitle>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <div className="space-y-2">
            <Label>Khách hàng</Label>
            <Select
              value={formData.customerId}
              onValueChange={(value) =>
                setFormData({ ...formData, customerId: value })
              }
            >
              <SelectTrigger>
                <SelectValue placeholder="Chọn khách hàng" />
              </SelectTrigger>
              <SelectContent>
                {customers.map((customer) => (
                  <SelectItem key={customer.id} value={customer.id}>
                    {customer.name} ({customer.email})
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label>Phòng</Label>
            <Select
              value={formData.roomId}
              onValueChange={(value) =>
                setFormData({ ...formData, roomId: value })
              }
            >
              <SelectTrigger>
                <SelectValue placeholder="Chọn phòng hiện có" />
              </SelectTrigger>
              <SelectContent>
                {availableRooms.map((room) => (
                  <SelectItem key={room.id} value={room.id}>
                    Phòng {room.number} - {room.type} ({formatVND(room.price)}
                    /đêm)
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="checkIn">Check In</Label>
              <Input
                id="checkIn"
                type="date"
                value={formData.checkIn}
                onChange={(e) =>
                  setFormData({ ...formData, checkIn: e.target.value })
                }
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="checkOut">Check Out</Label>
              <Input
                id="checkOut"
                type="date"
                value={formData.checkOut}
                onChange={(e) =>
                  setFormData({ ...formData, checkOut: e.target.value })
                }
              />
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="deposit">Tiền cọc (VNĐ)</Label>
            <Input
              id="deposit"
              type="number"
              value={formData.deposit}
              onChange={(e) =>
                setFormData({ ...formData, deposit: Number(e.target.value) })
              }
            />
          </div>

          {calculateTotal() > 0 && (
            <div className="p-3 rounded-lg bg-muted/50">
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground">Tổng tiền: </span>
                <span className="font-semibold">
                  {formatVND(calculateTotal())}
                </span>
              </div>
            </div>
          )}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Hủy
          </Button>
          <Button onClick={handleSave} disabled={loading || !isValid}>
            {loading ? "Đang tạo" : "Tạo"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default CreateBookingModal;
