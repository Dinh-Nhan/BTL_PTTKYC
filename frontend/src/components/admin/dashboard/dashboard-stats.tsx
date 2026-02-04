import { Card, CardContent } from "@/components/ui/card";
import { formatVND } from "@/lib/format";
// import { dashboardStats } from "@/lib/mock-data";
import { BedDouble, CalendarDays, CheckCircle, DollarSign } from "lucide-react";
import { useEffect, useState } from "react";
import roomApi from "@/api/roomApi";
import billApi from "@/api/billApi";

const DashboardStats = () => {
  const [totalRooms, setTotalRooms] = useState(0);
  const [totalRoomAvailables, setTotalRoomAvailables] = useState(0);
  const [totalRoomsReserved, setRoomsReserved] = useState(0);
  const [revenue, setRevenue] = useState(0);

  useEffect(() => {
    const getTotalRoom = async () => {
      try {
        const res = await roomApi.getAll();
        setTotalRooms(res.data.result.length);
      } catch (error) {
        console.log(error);
      }
    };
    const getRoomAvailabel= async () => {
      try {
        const res = await roomApi.getAvailable();
        setTotalRoomAvailables(res.data.result.length);
      } catch (error) {
        console.log(error)
      }
    }
    const getAllRoomsReserved = async () => {
          try {
            const res = await roomApi.getAll();
            const reservedRooms = res.data.result.filter(
              room => room.status === "booked"
            );
            setRoomsReserved(reservedRooms.length);
          } catch (error) {
            console.log(error)
          }
        }
        const getRevenue = async () => {
      try {
        const res = await billApi.getAll();
        const bills = res.data.result;

        const now = new Date();
        const currentMonth = now.getMonth(); // 0 - 11
        const currentYear = now.getFullYear();

        const totalRevenue = bills
          .filter((bill) => {
            const createdAt = new Date(bill.createdAt);
            return (
              createdAt.getMonth() === currentMonth &&
              createdAt.getFullYear() === currentYear
            );
          })
          .reduce((sum, bill) => sum + bill.finalAmount, 0);

        setRevenue(totalRevenue);
      } catch (error) {
        console.log(error);
      }
    };


    getTotalRoom();
    getRoomAvailabel();
    getAllRoomsReserved();
    getRevenue();
  }, []);

  const stats = [
    {
      label: "Tổng số phòng",
      value: totalRooms,
      icon: BedDouble,
      color: "bg-primary/10 text-primary",
    },
    {
      label: "Phòng hiện có sẵn",
      value: totalRoomAvailables,
      icon: CheckCircle,
      color: "bg-success/10 text-success",
    },
    {
      label: "Số phòng đã đặt",
      value: totalRoomsReserved,
      icon: CalendarDays,
      color: "bg-warning/10 text-warning",
    },
    {
      label: "Doanh thu tháng",
      value: formatVND(revenue),
      icon: DollarSign,
      color: "bg-chart-5/10 text-chart-5",
    },
  ];

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      {stats.map((item) => (
        <Card key={item.label} className="border shadow-sm">
          <CardContent className="p-5">
            <div className="flex items-center gap-4">
              <div className={`p-3 rounded-lg ${item.color}`}>
                <item.icon className="w-5 h-5" />
              </div>
              <div>
                <div className="text-sm text-muted-foreground">
                  {item.label}
                </div>
                <div className="text-2xl font-semibold">
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
