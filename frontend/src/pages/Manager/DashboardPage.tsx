import BookingsChart from "@/components/admin/dashboard/bookings-chart";
import DashboardStats from "@/components/admin/dashboard/dashboard-stats";
import RecentBookings from "@/components/admin/dashboard/recent-bookings";
import Header from "@/components/admin/layout/header";

const DashBoardPage = () => {
  return (
    <div className="flex flex-col h-full">
      <Header title="Tổng quan" />
      <div className="flex-1 p-6 space-y-6">
        <DashboardStats />
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <BookingsChart />
          <RecentBookings />
        </div>
      </div>
    </div>
  );
};

export default DashBoardPage;
