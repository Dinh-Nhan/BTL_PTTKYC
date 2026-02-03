import { Route, Routes } from "react-router-dom";
// import Admin from "./pages/Admin";
// import RoomManagerPage from "./pages/Manager/RoomManagerPage";
// import DashBoardPage from "./pages/Manager/DashboardPage";
// import BookingPage from "./pages/Manager/BookingPage";
// import InvoicesPage from "./pages/Manager/InvoicesPage";
// import StaffPage from "./pages/Manager/StaffPage";
// import CustomerPage from "./pages/Manager/CustomerPage";
import Home from "./pages/Home";
import RoomDetail from "./pages/RoomDetail";

const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/room/:id" element={<RoomDetail />} />
    </Routes>

    // <Admin>
    //   <Routes>

    //     <Route path="/admin" element={<DashBoardPage />} />
    //     <Route path="/admin/rooms" element={<RoomManagerPage />} />
    //     <Route path="/admin/bookings" element={<BookingPage />} />
    //     <Route path="/admin/invoices" element={<InvoicesPage />} />
    //     <Route path="/admin/staff" element={<StaffPage />} />
    //     <Route path="/admin/customers" element={<CustomerPage />} />
    //   </Routes>
    // </Admin>
  );
};

export default App;
