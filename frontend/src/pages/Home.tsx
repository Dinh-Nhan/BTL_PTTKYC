import Header from "@/components/client/Header";
import RoomListing from "@/components/client/RoomListing";
import SearchPanel from "@/components/client/SearchPanel";
import { useState, useEffect } from "react";
import Swal from 'sweetalert2';
import { useNavigate, useSearchParams as useRouterSearchParams } from "react-router-dom";
import bookingApi from "@/api/bookingApi";

const Home = () => {
  const [searchParams, setSearchParams] = useState({
    checkIn: "",
    checkOut: "",
    guests: 1,
    priceRange: [0, 500],
    roomType: "all",
  });

  const [rooms, setRooms] = useState([]);
  const [autoOpenRoomId, setAutoOpenRoomId] = useState<number | null>(null);
  
  const [urlParams] = useRouterSearchParams();
  const navigate = useNavigate();

  const handleSearchResult = (results: any[]) => {
    setRooms(results);
  };

  // 🔥 Xử lý email verification callback
  useEffect(() => {
    const emailVerified = urlParams.get('emailVerified');
    const roomId = urlParams.get('roomId');
    
    if (emailVerified === '1' && roomId) {
      const roomIdNum = parseInt(roomId);
      
      // Set roomId để RoomListing auto-open modal
      setAutoOpenRoomId(roomIdNum);
      
      // Xóa URL params ngay lập tức (không chờ)
      navigate('/', { replace: true });
    }
  }, [urlParams, navigate]);

  // 🔥 Xử lý payment callback
  useEffect(() => {
    const payment = urlParams.get('payment');
    const bookingId = urlParams.get('bookingId');
    const message = urlParams.get('message');



    if (payment === 'success' && bookingId) {
      Swal.fire({
        icon: 'success',
        title: 'Thanh toán thành công!',
        html: `
          <p>Cảm ơn bạn đã đặt phòng!</p>
          <p class="font-semibold mt-2">Mã booking: <span class="text-orange-600">${bookingId}</span></p>
          <p class="text-sm text-gray-600 mt-2">Email xác nhận đã được gửi đến hộp thư của bạn</p>
        `,
        confirmButtonColor: '#ff6b35',
        showCancelButton: true,
        cancelButtonText: 'Đóng',
      }).then((result) => {
        console.log('Gửi email xác nhận booking...', bookingId);
        bookingApi.sendBookingEmail(parseInt(bookingId));
        if (result.isConfirmed) {
          console.log('Xem chi tiết booking:', bookingId);
        }
      });

      navigate('/', { replace: true });
    } 
    else if (payment === 'failed') {
      Swal.fire({
        icon: 'error',
        title: 'Thanh toán thất bại!',
        text: message || 'Đã có lỗi xảy ra trong quá trình thanh toán. Vui lòng thử lại.',
        confirmButtonText: 'Đóng',
        confirmButtonColor: '#dc2626',
      });

      navigate('/', { replace: true });
    }
  }, [urlParams, navigate]);

  return (
    <>
      <main className="min-h-screen bg-background">
        <Header />
        <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
          <SearchPanel onSearch={handleSearchResult} />
          <RoomListing
            rooms={rooms}
            setRooms={setRooms} 
            searchParams={searchParams}
            autoOpenRoomId={autoOpenRoomId}
            onModalOpened={() => setAutoOpenRoomId(null)}
          />
        </div>
      </main>
    </>
  );
};

export default Home;