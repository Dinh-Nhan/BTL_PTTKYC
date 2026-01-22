import { Route, Routes } from "react-router-dom";
import Admin from "./pages/Admin";
import RoomManagerPage from "./pages/Manager/RoomManagerPage";
import DashBoardPage from "./pages/Manager/DashboardPage";
import BookingPage from "./pages/Manager/BookingPage";
import InvoicesPage from "./pages/Manager/InvoicesPage";
import StaffPage from "./pages/Manager/StaffPage";
import CustomerPage from "./pages/Manager/CustomerPage";

const App = () => {
  return (
    // <Home />
    <Admin>
      <Routes>
        <Route path="/" element={<DashBoardPage />} />
        <Route path="rooms" element={<RoomManagerPage />} />
        <Route path="bookings" element={<BookingPage />} />
        <Route path="invoices" element={<InvoicesPage />} />
        <Route path="staff" element={<StaffPage />} />
        <Route path="customers" element={<CustomerPage />} />
      </Routes>
    </Admin>
  );
};

export default App;
