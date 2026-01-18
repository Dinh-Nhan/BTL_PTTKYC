import { useState } from "react";
import FilterSidebar from "./FilterSidebar";
import RoomCard from "./RoomCard";
import { Card } from "./ui/card";

interface RoomListingProps {
  searchParams: any;
}

// Dữ liệu mẫu
const ROOMS = [
  {
    id: 1,
    name: "Deluxe Room",
    description: "Spacious room with city view and modern amenities",
    price: 2500000,
    rating: 4.8,
    reviews: 245,
    guests: 2,
    type: "standard",
    image: "/luxury-hotel-room-deluxe.jpg",
    amenities: ["WiFi", "AC", "TV", "Mini Bar"],
  },
  {
    id: 2,
    name: "Premium Suite",
    description: "Luxurious suite with separate living area and bath",
    price: 4200000,
    rating: 4.9,
    reviews: 312,
    guests: 4,
    type: "suite",
    image: "/luxury-hotel-room-suite.jpg",
    amenities: ["WiFi", "AC", "TV", "Kitchenette", "Jacuzzi"],
  },
  {
    id: 3,
    name: "Standard Room",
    description: "Comfortable room perfect for solo travelers",
    price: 1500000,
    rating: 4.6,
    reviews: 189,
    guests: 1,
    type: "standard",
    image: "/luxury-hotel-room-standard.jpg",
    amenities: ["WiFi", "AC", "TV"],
  },
  {
    id: 4,
    name: "Family Suite",
    description: "Large family-friendly suite with multiple bedrooms",
    price: 5500000,
    rating: 4.9,
    reviews: 156,
    guests: 6,
    type: "family",
    image: "/luxury-hotel-room-family-suite.jpg",
    amenities: ["WiFi", "AC", "TV", "Kitchen", "Multiple Bedrooms"],
  },
  {
    id: 5,
    name: "Ocean View Room",
    description: "Spectacular ocean views with private balcony",
    price: 3800000,
    rating: 4.9,
    reviews: 298,
    guests: 2,
    type: "deluxe",
    image: "/luxury-hotel-ocean-view-room.jpg",
    amenities: ["WiFi", "AC", "TV", "Balcony", "Ocean View"],
  },
  {
    id: 6,
    name: "Presidential Suite",
    description: "Ultimate luxury experience with panoramic views",
    price: 9800000,
    rating: 5.0,
    reviews: 87,
    guests: 6,
    type: "suite",
    image: "/luxury-hotel-presidential-suite.jpg",
    amenities: ["WiFi", "AC", "TV", "Kitchen", "Concierge", "Private Spa"],
  },
];

const RoomListing = ({ searchParams }: RoomListingProps) => {
  const [priceRange, setPriceRange] = useState<[number, number]>([0, Infinity]);
  const [selectedType, setSelectedType] = useState("all");
  const [sortBy, setSortBy] = useState("featured");

  const filteredRooms = ROOMS.filter((room) => {
    const matchesPrice =
      room.price >= priceRange[0] && room.price <= priceRange[1];
    const matchesType = selectedType === "all" || room.type === selectedType;
    const matchesGuests = searchParams.guests
      ? room.guests >= searchParams.guests
      : true;

    return matchesPrice && matchesType && matchesGuests;
  });

  const sortedRooms = [...filteredRooms].sort((a, b) => {
    switch (sortBy) {
      case "price-low":
        return a.price - b.price;
      case "price-high":
        return b.price - a.price;
      case "rating":
        return b.rating - a.rating;
      default:
        return 0;
    }
  });

  return (
    <div className="grid gap-8 lg:grid-cols-4">
      {/* Filters Sidebar */}
      <div className="lg:col-span-1">
        <FilterSidebar
          priceRange={priceRange}
          onPriceChange={setPriceRange}
          selectedType={selectedType}
          onTypeChange={setSelectedType}
          sortBy={sortBy}
          onSortChange={setSortBy}
        />
      </div>

      {/* Room Grid */}
      <div className="lg:col-span-3">
        <div className="mb-6 flex items-center justify-between">
          <p className="text-sm text-muted-foreground">
            Hiển thị được {sortedRooms.length} kết quả
          </p>
        </div>

        {sortedRooms.length > 0 ? (
          <div className="grid gap-6 sm:grid-cols-2">
            {sortedRooms.map((room) => (
              <RoomCard key={room.id} room={room} />
            ))}
          </div>
        ) : (
          <Card className="border-border bg-card p-8 text-center">
            <p className="text-muted-foreground">
              Rất tiếc, hiện không có phòng đáp ứng tiêu chí tìm kiếm của bạn.{" "}
              <br />
              Vui lòng điều chỉnh bộ lọc để khám phá thêm các lựa chọn khác.
            </p>
          </Card>
        )}
      </div>
    </div>
  );
};

export default RoomListing;
