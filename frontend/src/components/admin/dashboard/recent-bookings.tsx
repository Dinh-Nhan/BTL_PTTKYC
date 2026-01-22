import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { bookings } from "@/lib/mock-data";

const statusStyles = {
  pending: "bg-warning/10 text-warning border-warning/20",
  confirmed: "bg-success/10 text-success border-success/20",
  cancelled: "bg-destructive/10 text-destructive border-destructive/20",
};

const RecentBookings = () => {
  const recentBookings = bookings.slice(0, 5);

  return (
    <Card className="border shadow-sm">
      <CardHeader className="pb-2">
        <CardTitle className="text-base font-semibold">
          Đặt phòng gần đây
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-3">
          {recentBookings.map((booking) => (
            <div
              key={booking.id}
              className="flex items-center justify-between p-3 rounded-lg bg-muted/50"
            >
              <div className="flex flex-col gap-0.5">
                <span className="text-sm font-medium text-foreground">
                  {booking.customerName}
                </span>
                <span className="text-xs text-muted-foreground">
                  Room {booking.roomNumber} | {booking.checkIn} -{" "}
                  {booking.checkOut}
                </span>
              </div>
              <Badge variant="outline" className={statusStyles[booking.status]}>
                {booking.status.charAt(0).toUpperCase() +
                  booking.status.slice(1)}
              </Badge>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
};

export default RecentBookings;
