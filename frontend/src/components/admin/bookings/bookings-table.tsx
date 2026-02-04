import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import { formatVND } from "@/lib/format";
import { Eye, XCircle } from "lucide-react";

// import  Booking  from "@/types/booking";

/* ================= TYPE ================= */

interface BookingsTableProps {
  bookings: Booking[];
  onView: (booking: Booking) => void;
  onCancel: (booking: Booking) => void;
}

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

/* ================= STYLE ================= */

const statusStyles: Record<string, string> = {
  pending: "bg-warning/10 text-warning border-warning/20",
  confirmed: "bg-success/10 text-success border-success/20",
  cancelled: "bg-destructive/10 text-destructive border-destructive/20",
  checked_in: "bg-primary/10 text-primary border-primary/20",
  checked_out: "bg-muted text-muted-foreground border-muted",
};

/* ================= COMPONENT ================= */

const BookingsTable = ({
  bookings,
  onView,
  onCancel,
}: BookingsTableProps) => {
  return (
    <Card className="border shadow-sm">
      <CardContent className="p-0">
        <Table>

          {/* HEADER */}
          <TableHeader>
            <TableRow className="bg-muted/50">
              <TableHead>Booking ID</TableHead>
              <TableHead>Khách hàng</TableHead>
              <TableHead>Phòng</TableHead>
              <TableHead>Check In</TableHead>
              <TableHead>Check Out</TableHead>
              <TableHead>Tiền cọc</TableHead>
              <TableHead>Trạng thái</TableHead>
              <TableHead className="text-right">Thao tác</TableHead>
            </TableRow>
          </TableHeader>

          {/* BODY */}
          <TableBody>
            {bookings.length === 0 ? (
              <TableRow>
                <TableCell
                  colSpan={8}
                  className="text-center py-8 text-muted-foreground"
                >
                  Không có dữ liệu booking
                </TableCell>
              </TableRow>
            ) : (
              bookings.map((booking) => (
                <TableRow key={booking.bookingId}>

                  {/* ID */}
                  <TableCell className="font-medium">
                    #{booking.bookingId}
                  </TableCell>

                  {/* CUSTOMER */}
                  <TableCell>
                    {booking.client?.fullName || "Khách lẻ"}
                  </TableCell>

                  {/* ROOM */}
                  <TableCell>
                    {booking.roomResponse?.roomNumber || "-"}
                  </TableCell>

                  {/* CHECK IN */}
                  <TableCell>
                    {new Date(
                      booking.checkInDatetime
                    ).toLocaleDateString("vi-VN")}
                  </TableCell>

                  {/* CHECK OUT */}
                  <TableCell>
                    {new Date(
                      booking.checkOutDatetime
                    ).toLocaleDateString("vi-VN")}
                  </TableCell>

                  {/* DEPOSIT */}
                  <TableCell>
                    {formatVND(booking.depositAmount)}
                  </TableCell>

                  {/* STATUS */}
                  <TableCell>
                    <Badge
                      variant="outline"
                      className={
                        statusStyles[booking.status.toLowerCase()] ||
                        "bg-muted text-muted-foreground"
                      }
                    >
                      {booking.status.toUpperCase()}
                    </Badge>
                  </TableCell>

                  {/* ACTION */}
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-1">

                      {/* VIEW */}
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => onView(booking)}
                      >
                        <Eye className="w-4 h-4" />
                      </Button>

                      {/* CANCEL */}
                      {booking.status.toLowerCase() !== "cancelled" && (
                        <Button
                          variant="ghost"
                          size="sm"
                          className="text-destructive"
                          onClick={() => onCancel(booking)}
                        >
                          <XCircle className="w-4 h-4" />
                        </Button>
                      )}

                    </div>
                  </TableCell>

                </TableRow>
              ))
            )}
          </TableBody>

        </Table>
      </CardContent>
    </Card>
  );
};

export default BookingsTable;
