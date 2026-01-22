import { Route, Routes } from "react-router-dom";
import Admin from "./pages/Admin";
import RoomManagerPage from "./pages/Manager/RoomManagerPage";
import DashBoardPage from "./pages/Manager/DashboardPage";

const App = () => {
  return (
    // <Home />
    <Admin>
      <Routes>
        <Route path="/" element={<DashBoardPage />} />
        <Route path="rooms" element={<RoomManagerPage />} />
      </Routes>
    </Admin>
  );
};

export default App;
