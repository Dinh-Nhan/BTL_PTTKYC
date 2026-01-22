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
import type { Booking } from "@/lib/mock-data";

interface BookingDetailsModalProps {
  booking: Booking | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onUpdateDeposit: (bookingId: string, deposit: number) => void;
}

const statusStyles = {
  pending: "bg-warning/10 text-warning border-warning/20",
  confirmed: "bg-success/10 text-success border-success/20",
  cancelled: "bg-destructive/10 text-destructive border-destructive/20",
};

const BookingDetailsModal = ({
  booking,
  open,
  onOpenChange,
  onUpdateDeposit,
}: BookingDetailsModalProps) => {
  const [deposit, setDeposit] = useState(booking?.deposit || 0);
  const [loading, setLoading] = useState(false);

  if (!booking) return null;

  const handleUpdateDeposit = async () => {
    setLoading(true);
    await new Promise((resolve) => setTimeout(resolve, 500));
    setLoading(false);
    onUpdateDeposit(booking.id, deposit);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-3">
            Booking {booking.id}
            <Badge variant="outline" className={statusStyles[booking.status]}>
              {booking.status.charAt(0).toUpperCase() + booking.status.slice(1)}
            </Badge>
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label className="text-muted-foreground text-xs">Customer</Label>
              <p className="font-medium">{booking.customerName}</p>
            </div>
            <div>
              <Label className="text-muted-foreground text-xs">Room</Label>
              <p className="font-medium">{booking.roomNumber}</p>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label className="text-muted-foreground text-xs">Check In</Label>
              <p className="font-medium">{booking.checkIn}</p>
            </div>
            <div>
              <Label className="text-muted-foreground text-xs">Check Out</Label>
              <p className="font-medium">{booking.checkOut}</p>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label className="text-muted-foreground text-xs">
                Total Amount
              </Label>
              <p className="font-semibold text-lg">${booking.totalAmount}</p>
            </div>
            <div>
              <Label className="text-muted-foreground text-xs">
                Created At
              </Label>
              <p className="font-medium">{booking.createdAt}</p>
            </div>
          </div>

          {booking.status !== "cancelled" && (
            <div className="p-4 rounded-lg border border-border bg-muted/30 space-y-3">
              <Label htmlFor="depositUpdate">Update Deposit ($)</Label>
              <div className="flex gap-2">
                <Input
                  id="depositUpdate"
                  type="number"
                  value={deposit}
                  onChange={(e) => setDeposit(Number(e.target.value))}
                  className="flex-1"
                />
                <Button onClick={handleUpdateDeposit} disabled={loading}>
                  {loading ? "..." : "Update"}
                </Button>
              </div>
              <p className="text-xs text-muted-foreground">
                Current deposit: ${booking.deposit}
              </p>
            </div>
          )}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Close
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default BookingDetailsModal;
