import { useParams, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import {
  ChevronLeft,
  ChevronRight,
  Users,
  Wifi,
  Tv,
  Wind,
  MoreHorizontal,
} from "lucide-react";
import { formatVND } from "@/lib/format";
import { getRoomById, getRelatedRooms } from "@/lib/room-data";
import Header from "@/components/client/Header";
import RoomCard from "@/components/client/RoomCard";
import BookingModal from "@/components/client/BookingModal";
import roomApi from "@/api/roomApi";

const RoomDetail = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [showBooking, setShowBooking] = useState(false);
  const [room, setRoom] = useState<any>(null);
  // const room = getRoomById(Number(id));
  const [loading, setLoading] = useState(true);
  const [relatedRooms, setRelatedRooms] = useState<any[]>([]);

  // Scroll to top when room id changes
  useEffect(() => {
    window.scrollTo(0, 0);

    const getDetail = async () => {
      try {
        setLoading(true);
        const response = await roomApi.getRoomById(id);
        const roomData = response.data.result;
        
        const roomsData = await roomApi.getAll();
        const allRooms = roomsData.data.result;

        const related = allRooms.filter(
          (r: any) =>
            r.roomType?.typeName === roomData.roomType?.typeName &&
            r.roomId !== roomData.roomId
        );
        setRelatedRooms(
          related.map((r: any) => ({
            id: r.roomId,
            name: r.roomType?.typeName,
            description: r.roomType?.description,
            price: r.roomType?.basePrice,
            guests: r.roomType?.maxAdult,
            type: r.roomType?.typeName,
            image: r.roomType?.imageUrl,
            amenities:
              r.roomType?.amenities
                ?.split(",")
                .map((a: string) => a.trim()) || [],
            roomType: r.roomType,
          }))
        );
        // Map dữ liệu từ API về format hiển thị
        const mappedRoom = {
          id: roomData.roomId,
          name: roomData.roomType?.typeName,
          description: roomData.roomType?.description,
          fullDescription: roomData.roomType?.description,
          price: roomData.roomType?.basePrice,
          guests: parseInt(roomData.roomType?.maxAdult) + parseInt(roomData.roomType?.maxChildren),
          type: roomData.roomType?.typeName,
          image: roomData.roomType?.imageUrl,
          images: roomData.roomType?.imageUrl ? [roomData.roomType?.imageUrl] : [],
          amenities: roomData.roomType?.amenities?.split(",").map((a: string) => a.trim()) || [],
          rating: 4.5, // Nếu API không có, dùng giá trị mặc định
          reviews: 10,
          area: roomData.roomType?.roomArea || "N/A",
          status: roomData.status || "AVAILABLE",
          roomType: roomData.roomType         
        };
        
        setRoom(mappedRoom);
        
      } catch (error) {
        console.error("Error fetching room details:", error);
        setRoom(null);
      } finally {
        setLoading(false);
      }
    };

    if (id) {
      getDetail();
    }
  }, [id]);


  if (loading) {
    return (
      <div className="min-h-screen bg-background">
        <Header />
        <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
          <div className="flex flex-col items-center justify-center py-12">
            <p className="mb-4 text-lg text-muted-foreground">
              Đang tải thông tin phòng...
            </p>
          </div>
        </div>
      </div>
    );
  }

  if (!room) {
    return (
      <div className="min-h-screen bg-background">
        <Header />
        <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
          <div className="flex flex-col items-center justify-center py-12">
            <p className="mb-4 text-lg text-muted-foreground">
              Phòng không tìm thấy
            </p>
            <button
              onClick={() => navigate("/")}
              className="rounded-md bg-accent px-6 py-2 text-accent-foreground hover:bg-accent/90 transition-colors"
            >
              Quay lại
            </button>
          </div>
        </div>
      </div>
    );
  }

  const images = room.images || [room.image];
  const currentImage = images[currentImageIndex];

  const handlePrevImage = () => {
    setCurrentImageIndex((prev) => (prev === 0 ? images.length - 1 : prev - 1));
  };

  const handleNextImage = () => {
    setCurrentImageIndex((prev) => (prev === images.length - 1 ? 0 : prev + 1));
  };

  const getAmenityIcon = (amenity: string) => {
    switch (amenity.toLowerCase()) {
      case "wifi":
        return <Wifi className="h-4 w-4" />;
      case "tv":
        return <Tv className="h-4 w-4" />;
      case "ac":
      case "điều hòa":
        return <Wind className="h-4 w-4" />;
      default:
        return <MoreHorizontal className="h-4 w-4" />;
    }
  };

  return (
    <>
      <div className="min-h-screen bg-background">
        <Header />

        <main className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
          {/* Back Button */}
          <button
            onClick={() => navigate("/")}
            className="mb-6 flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
          >
            <ChevronLeft className="h-4 w-4" />
            Quay lại danh sách phòng
          </button>

          <div className="grid gap-8 lg:grid-cols-3">
            {/* Image Slider */}
            <div className="lg:col-span-2">
              <div className="relative mb-4 aspect-video overflow-hidden rounded-lg bg-secondary">
                <img
                  src='https://tubepfurniture.com/wp-content/uploads/2020/09/phong-mau-khach-san-go-cong-nghiep-01.jpg'
                  alt={room.name}
                  className="h-full w-full object-cover"
                />

                {/* Navigation Buttons */}
                {images.length > 1 && (
                  <>
                    <button
                      onClick={handlePrevImage}
                      className="absolute left-4 top-1/2 -translate-y-1/2 rounded-full bg-black/50 p-2 text-white hover:bg-black/70 transition-colors"
                    >
                      <ChevronLeft className="h-6 w-6" />
                    </button>
                    <button
                      onClick={handleNextImage}
                      className="absolute right-4 top-1/2 -translate-y-1/2 rounded-full bg-black/50 p-2 text-white hover:bg-black/70 transition-colors"
                    >
                      <ChevronRight className="h-6 w-6" />
                    </button>

                    {/* Image Counter */}
                    <div className="absolute bottom-4 right-4 rounded-full bg-black/70 px-3 py-1 text-xs text-white">
                      {currentImageIndex + 1} / {images.length}
                    </div>
                  </>
                )}
              </div>

              {/* Thumbnails */}
              {images.length > 1 && (
                <div className="flex gap-2 overflow-x-auto pb-2">
                  {images.map((image, index) => (
                    <button
                      key={index}
                      onClick={() => setCurrentImageIndex(index)}
                      className={`h-20 w-20 flex-shrink-0 overflow-hidden rounded-lg border-2 transition-colors ${
                        index === currentImageIndex
                          ? "border-accent"
                          : "border-border hover:border-muted-foreground"
                      }`}
                    >
                      <img
                        src='https://tubepfurniture.com/wp-content/uploads/2020/09/phong-mau-khach-san-go-cong-nghiep-01.jpg'
                        alt={`${room.name} ${index + 1}`}
                        className="h-full w-full object-cover"
                      />
                    </button>
                  ))}
                </div>
              )}

              {/* Room Info */}
              <div className="mt-8 space-y-6">
                <div>
                  <h1 className="text-3xl font-semibold text-foreground">
                    {room.name}
                  </h1>
                </div>

                {/* Rating and Reviews */}
                <div className="flex items-center gap-4 border-b border-border pb-6">
                  <div>
                    <div className="flex items-center gap-2">
                      <span className="text-lg font-semibold text-foreground">
                        ⭐ {room.rating}
                      </span>
                      <span className="text-sm text-muted-foreground">
                        ({room.reviews} bài đánh giá)
                      </span>
                    </div>
                  </div>
                </div>

                {/* Description */}
                <div>
                  <h2 className="mb-2 text-lg font-semibold text-foreground">
                    Mô tả
                  </h2>
                  <p className="text-sm leading-relaxed text-muted-foreground">
                    {room.fullDescription || room.description}
                  </p>
                </div>

                {/* Room Details */}
                <div className="grid gap-4 sm:grid-cols-2">
                  <div className="rounded-lg bg-secondary p-4">
                    <p className="text-xs text-muted-foreground">Diện tích</p>
                    <p className="mt-1 text-lg font-semibold text-foreground">
                      {room.area || "N/A"} m²
                    </p>
                  </div>
                  <div className="rounded-lg bg-secondary p-4">
                    <div className="flex items-center gap-2">
                      <Users className="h-4 w-4 text-muted-foreground" />
                      <div>
                        <p className="text-xs text-muted-foreground">
                          Tối đa khách
                        </p>
                        <p className="mt-1 text-lg font-semibold text-foreground">
                          {room.guests} người
                        </p>
                      </div>
                    </div>
                  </div>
                </div>

                {/* Amenities */}
                <div>
                  <h2 className="mb-4 text-lg font-semibold text-foreground">
                    Tiện nghi
                  </h2>
                  <div className="grid gap-3 sm:grid-cols-2">
                    {room.amenities.map((amenity) => (
                      <div
                        key={amenity}
                        className="flex items-center gap-3 rounded-lg bg-secondary p-4"
                      >
                        {getAmenityIcon(amenity)}
                        <span className="text-sm text-foreground">
                          {amenity}
                        </span>
                      </div>
                    ))}
                  </div>
                </div>

                
              </div>
            </div>

            {/* Sidebar - Booking Info */}
            <div className="lg:col-span-1">
              <div className="sticky top-6 rounded-lg border border-border bg-card p-6">
                <div className="space-y-4">
                  {/* Price */}
                  <div>
                    <p className="text-xs text-muted-foreground">Chỉ từ</p>
                    <p className="text-3xl font-light text-foreground">
                      {formatVND(room.price)}
                      <span className="text-base text-muted-foreground">
                        /đêm
                      </span>
                    </p>
                  </div>

                  {/* Status */}
                  <div>
                    <span
                      className={`inline-block rounded-full px-3 py-1 text-xs font-medium ${
                        room.status === "AVAILABLE"
                          ? "bg-green-100 text-green-800"
                          : "bg-red-100 text-red-800"
                      }`}
                    >
                      {room.status === "AVAILABLE"
                        ? "✓ Còn phòng"
                        : "✕ Hết phòng"}
                    </span>
                  </div>

                  {/* Booking Button */}
                  <button
                    onClick={() => setShowBooking(true)}
                    disabled={room.status === "unavailable"}
                    className="w-full rounded-md bg-accent px-5 py-3 text-sm font-medium text-accent-foreground hover:bg-accent/90 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
                  >
                    {room.status === "AVAILABLE"
                      ? "Đặt phòng ngay"
                      : "Không có sẵn"}
                  </button>

                  {/* Contact Section */}
                  <div className="rounded-lg bg-secondary p-4 text-center">
                    <p className="text-xs text-muted-foreground mb-2">
                      Cần tư vấn?
                    </p>
                    <a
                      href="tel:+84123456789"
                      className="inline-block text-lg font-semibold text-accent hover:text-accent/80 transition-colors"
                    >
                      +84 (123) 456-789
                    </a>
                    <p className="text-xs text-muted-foreground mt-2">
                      Liên hệ ngay để được tư vấn
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Related Rooms */}
          {relatedRooms.length > 0 && (
            <div className="mt-16 border-t border-border pt-12">
              <h2 className="mb-8 text-2xl font-semibold text-foreground">
                Các phòng tương tự
              </h2>
              <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
                {relatedRooms.map((relatedRoom) => (
                  <RoomCard key={relatedRoom.id} room={relatedRoom} />
                ))}
              </div>
            </div>
          )}
        </main>
      </div>

      {showBooking && (
        <BookingModal room={room} onClose={() => setShowBooking(false)} />
      )}
    </>
  );
};

export default RoomDetail;
