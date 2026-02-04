import { Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import RoomDetail from "./pages/RoomDetail";
const App = () => {
  return (
    // <Admin>
    //   <Routes>
    //     <Route path="/" element={<DashBoardPage/>} />
    //     <Route path="rooms" element={<RoomManagerPage />} />
    //     <Route path="bookings" element={<BookingPage />} />
    //     <Route path="invoices" element={<InvoicesPage />} />
    //     <Route path="staff" element={<StaffPage />} />
    //     <Route path="customers" element={<CustomerPage />} />
    //   </Routes>
    // </Admin>
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/room/:id" element={<RoomDetail/>} />

      {/* <Route path="/login" element={<Login />} /> */}
    </Routes>
  );
};

export default App;
