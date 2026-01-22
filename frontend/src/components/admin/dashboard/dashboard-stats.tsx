import { Card, CardContent } from "@/components/ui/card";
import { formatVND } from "@/lib/format";
import { dashboardStats } from "@/lib/mock-data";
import { BedDouble, CalendarDays, CheckCircle, DollarSign } from "lucide-react";

const stats = [
  {
    label: "Tổng số phòng",
    value: dashboardStats.totalRooms,
    icon: BedDouble,
    color: "bg-primary/10 text-primary",
  },
  {
    label: "Phòng hiện có sẵn",
    value: dashboardStats.availableRooms,
    icon: CheckCircle,
    color: "bg-success/10 text-success",
  },
  {
    label: "Đặt phòng hôm nay",
    value: dashboardStats.bookingsToday,
    icon: CalendarDays,
    color: "bg-warning/10 text-warning",
  },
  {
    label: "Doanh thu tháng",
    value: formatVND(dashboardStats.monthlyRevenue),
    icon: DollarSign,
    color: "bg-chart-5/10 text-chart-5",
  },
];

const DashboardStats = () => {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      {stats.map((item) => (
        <Card key={item.label} className="border shadow-sm">
          <CardContent className="p-5">
            <div className="flex items-center gap-4">
              <div className={`p-3 rounded-lg ${item.color}`}>
                <item.icon className="w-5 h-5" />
              </div>
              <div className="flex flex-clos">
                <div className="text-2xl font-semibold text-foreground">
                  <div className="text-sm text-muted-foreground">
                    {item.label}
                  </div>
                  {item.value}
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
};

export default DashboardStats;
