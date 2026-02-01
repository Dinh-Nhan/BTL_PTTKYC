export interface Room {
  id: number;
  name: string;
  description: string;
  fullDescription?: string;
  price: number;
  rating: number;
  reviews: number;
  guests: number;
  type: string;
  image: string;
  images?: string[];
  amenities: string[];
  area?: number;
  status?: "available" | "unavailable";
  additionalInfo?: {
    checkInTime?: string;
    checkOutTime?: string;
    cancellationPolicy?: string;
  };
}

export const ROOMS: Room[] = [
  {
    id: 1,
    name: "Deluxe Room",
    description: "Spacious room with city view and modern amenities",
    fullDescription:
      "Một căn phòng rộng rãi được thiết kế với phong cách hiện đại, cung cấp khung cảnh thành phố tuyệt đẹp. Phòng được trang bị các tiện nghi cao cấp bao gồm giường ngủ hai mẫu đơn, phòng tắm lớn với vòi sen mưa, và không gian làm việc riêng. Hoàn hảo cho những du khách muốn tận hưởng sự thoải mái và tiện lợi.",
    price: 2500000,
    rating: 4.8,
    reviews: 245,
    guests: 2,
    type: "deluxe",
    image: "/luxury-hotel-room-deluxe.jpg",
    images: [
      "/luxury-hotel-room-deluxe.jpg",
      "/luxury-hotel-room-deluxe-2.jpg",
      "/luxury-hotel-room-deluxe-3.jpg",
    ],
    area: 35,
    status: "available",
    amenities: [
      "WiFi",
      "AC",
      "TV",
      "Hồ bơi",
      "Bãi đỗ xe",
      "Xe đưa đón sân bay",
    ],
    additionalInfo: {
      checkInTime: "14:00",
      checkOutTime: "12:00",
      cancellationPolicy: "Miễn phí hủy trước 48 giờ",
    },
  },
  {
    id: 2,
    name: "Premium Suite",
    description: "Luxurious suite with separate living area and bath",
    fullDescription:
      "Suite cao cấp với khu vực phòng khách riêng biệt, phòng ngủ rộng, và phòng tắm sang trọng. Được trang bị đầy đủ các tiện nghi cao cấp, bao gồm nhà bếp mini, bồn tắm nước nóng, và bàn làm việc. Lý tưởng cho các cặp đôi hoặc gia đình nhỏ.",
    price: 4200000,
    rating: 4.9,
    reviews: 312,
    guests: 4,
    type: "suite",
    image: "/luxury-hotel-room-suite.jpg",
    images: [
      "/luxury-hotel-room-suite.jpg",
      "/luxury-hotel-room-suite-2.jpg",
      "/luxury-hotel-room-suite-3.jpg",
    ],
    area: 55,
    status: "available",
    amenities: [
      "WiFi",
      "AC",
      "TV",
      "Kitchenette",
      "Hồ bơi",
      "Bãi đỗ xe",
      "Buffet sáng",
    ],
    additionalInfo: {
      checkInTime: "14:00",
      checkOutTime: "12:00",
      cancellationPolicy: "Miễn phí hủy trước 48 giờ",
    },
  },
  {
    id: 3,
    name: "Standard Room",
    description: "Comfortable room perfect for solo travelers",
    fullDescription:
      "Phòng tiêu chuẩn được thiết kế gọn gàng với đầy đủ tiện nghi cần thiết cho du khách một mình. Được trang bị giường đơn hoặc đôi, phòng tắm hiện đại, và không gian làm việc nhỏ. Một lựa chọn tuyệt vời với giá cả hợp lý.",
    price: 1500000,
    rating: 4.6,
    reviews: 189,
    guests: 1,
    type: "standard",
    image: "/luxury-hotel-room-standard.jpg",
    images: [
      "/luxury-hotel-room-standard.jpg",
      "/luxury-hotel-room-standard-2.jpg",
    ],
    area: 25,
    status: "available",
    amenities: [
      "WiFi",
      "AC",
      "TV",
      "Hồ bơi",
      "Bãi đỗ xe",
      "Gym / Fitness center",
    ],
    additionalInfo: {
      checkInTime: "14:00",
      checkOutTime: "12:00",
      cancellationPolicy: "Miễn phí hủy trước 24 giờ",
    },
  },
  {
    id: 4,
    name: "Family Suite",
    description: "Large family-friendly suite with multiple bedrooms",
    fullDescription:
      "Suite rộng rãi được thiết kế đặc biệt cho gia đình với nhiều phòng ngủ. Bao gồm phòng khách rộng, bếp đầy đủ, và nhiều phòng tắm. Một lựa chọn hoàn hảo cho những kỳ nghỉ gia đình dài hạn.",
    price: 5500000,
    rating: 4.9,
    reviews: 156,
    guests: 6,
    type: "family",
    image: "/luxury-hotel-room-family-suite.jpg",
    images: [
      "/luxury-hotel-room-family-suite.jpg",
      "/luxury-hotel-room-family-suite-2.jpg",
      "/luxury-hotel-room-family-suite-3.jpg",
    ],
    area: 75,
    status: "available",
    amenities: [
      "WiFi",
      "AC",
      "TV",
      "Bếp đầy đủ",
      "Hồ bơi",
      "Bãi đỗ xe",
      "Buffet sáng",
    ],
    additionalInfo: {
      checkInTime: "14:00",
      checkOutTime: "12:00",
      cancellationPolicy: "Miễn phí hủy trước 72 giờ",
    },
  },
  {
    id: 5,
    name: "Ocean View Room",
    description: "Spectacular ocean views with private balcony",
    fullDescription:
      "Phòng có khung cảnh biển tuyệt đẹp với ban công riêng tư, cho phép du khách tận hưởng những ánh chiều tà trên đại dương. Được trang bị đầy đủ tiện nghi cao cấp và không gian yên tĩnh để thư giãn.",
    price: 3800000,
    rating: 4.9,
    reviews: 298,
    guests: 2,
    type: "deluxe",
    image: "/luxury-hotel-ocean-view-room.jpg",
    images: [
      "/luxury-hotel-ocean-view-room.jpg",
      "/luxury-hotel-ocean-view-room-2.jpg",
      "/luxury-hotel-ocean-view-room-3.jpg",
    ],
    area: 40,
    status: "available",
    amenities: [
      "WiFi",
      "AC",
      "TV",
      "Ban công riêng",
      "Hồ bơi",
      "Xe đưa đón sân bay",
      "Gym / Fitness center",
    ],
    additionalInfo: {
      checkInTime: "14:00",
      checkOutTime: "12:00",
      cancellationPolicy: "Miễn phí hủy trước 48 giờ",
    },
  },
  {
    id: 6,
    name: "Presidential Suite",
    description: "Ultimate luxury experience with panoramic views",
    fullDescription:
      "Suite tổng thống mang đến trải nghiệm xa xỉ tối cao với tầm nhìn toàn cảnh thành phố. Được trang bị với các tiện nghi cao cấp nhất bao gồm spa riêng, bếp đầy đủ chức năng, và dịch vụ concierge 24/7.",
    price: 9800000,
    rating: 5.0,
    reviews: 87,
    guests: 6,
    type: "suite",
    image: "/luxury-hotel-presidential-suite.jpg",
    images: [
      "/luxury-hotel-presidential-suite.jpg",
      "/luxury-hotel-presidential-suite-2.jpg",
      "/luxury-hotel-presidential-suite-3.jpg",
    ],
    area: 120,
    status: "available",
    amenities: [
      "WiFi",
      "AC",
      "TV",
      "Bếp đầy đủ",
      "Hồ bơi",
      "Xe đưa đón sân bay",
      "Buffet sáng",
      "Gym / Fitness center",
    ],
    additionalInfo: {
      checkInTime: "14:00",
      checkOutTime: "12:00",
      cancellationPolicy: "Miễn phí hủy trước 72 giờ hoặc hoàn tiền đầy đủ",
    },
  },
];

export const getRoomById = (id: number): Room | undefined => {
  return ROOMS.find((room) => room.id === id);
};

export const getRelatedRooms = (currentId: number, limit: number = 3): Room[] => {
  return ROOMS.filter((room) => room.id !== currentId).slice(0, limit);
};
