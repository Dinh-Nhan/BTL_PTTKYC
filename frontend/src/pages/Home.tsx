import Header from "@/components/client/Header";
import RoomListing from "@/components/client/RoomListing";
import SearchPanel from "@/components/client/SearchPanel";
import { useState, useEffect } from "react";
import Swal from 'sweetalert2';
import { useNavigate, useSearchParams } from "react-router-dom";
const Home = () => {
  const [searchParams, setSearchParams] = useState({
    checkIn: "",
    checkOut: "",
    guests: 1,
    priceRange: [0, 500],
    roomType: "all",
  });

  const [rooms, setRooms] = useState([]);
  const handleSearchResult = (results: any[]) => {
    setRooms(results);
  };

  const navigate = useNavigate();

  const urlSearchParams = new URLSearchParams(window.location.search);
  useEffect(() => {
    const payment = urlSearchParams.get('payment');
    const bookingId = urlSearchParams.get('bookingId');
    const message = urlSearchParams.get('message');

    if (payment === 'success' && bookingId) {
      // Hiển thị thông báo thành công
      Swal.fire({
        icon: 'success',
        title: 'Thanh toán thành công!',
        html: `
          <p>Cảm ơn bạn đã đặt phòng!</p>
          <p class="font-semibold mt-2">Mã booking: <span class="text-orange-600">${bookingId}</span></p>
          <p class="text-sm text-gray-600 mt-2">Email xác nhận đã được gửi đến hộp thư của bạn</p>
        `,
        confirmButtonText: 'Xem chi tiết',
        confirmButtonColor: '#ff6b35',
        showCancelButton: true,
        cancelButtonText: 'Đóng',
      }).then((result) => {
        if (result.isConfirmed) {
          // Redirect đến trang chi tiết booking (nếu có)
          // navigate(`/booking/${bookingId}`);
          console.log('Xem chi tiết booking:', bookingId);
        }
      });

      // Xóa query params khỏi URL sau khi hiển thị
      navigate('/', { replace: true });
    } 
    else if (payment === 'failed') {
      // Hiển thị thông báo thất bại
      Swal.fire({
        icon: 'error',
        title: 'Thanh toán thất bại!',
        text: message || 'Đã có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại.',
        confirmButtonText: 'Đóng',
        confirmButtonColor: '#dc2626',
      });

      // Xóa query params
      navigate('/', { replace: true });
    }
  }, [urlSearchParams, navigate]);
  return (
    <>
      <main className="min-h-screen bg-background">
        <Header />
        <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
          <SearchPanel 
          onSearch={handleSearchResult} />
          <RoomListing
          rooms={rooms}
          setRooms={setRooms} 
          searchParams={searchParams} />
        </div>
      </main>
    </>
  );
};

export default Home;
