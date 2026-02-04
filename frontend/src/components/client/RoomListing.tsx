import { useEffect, useState } from "react";
import FilterSidebar from "./FilterSidebar";
import RoomCard from "./RoomCard";
import { Card } from "../ui/card";
import roomApi from "@/api/roomApi.js";

interface RoomListingProps {
  searchParams: any;
}

const RoomListing = ({ searchParams }: RoomListingProps) => {
  const [priceRange, setPriceRange] = useState<[number, number]>([0, Infinity]);
  const [selectedType, setSelectedType] = useState("all");
  const [sortBy, setSortBy] = useState("featured");

  const [rooms, setRooms] = useState<any[]>([]);
  const [load, setLoad] = useState(false);

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        setLoad(true);

        const res = await roomApi.getAll();

        const mappedRooms = res.data.result.map((room: any) => ({
          id: room.roomId,
          name: room.roomType?.typeName,
          description: room.roomType?.description,
          price: room.roomType?.basePrice,
          guests: room.roomType?.maxAdult,
          type: room.roomType?.typeName,
          image: room.roomType?.imageUrl,
          amenities:
            room.roomType?.amenities?.split(",").map((a: string) => a.trim()) ||
            [],

          roomType: room.roomType, // giữ lại nếu cần
        }));

        setRooms(mappedRooms);
      } catch (error) {
        console.error("Failed to fetch rooms:", error);
      } finally {
        setLoad(false);
      }
    };

    fetchRooms();
  }, []);

  const filteredRooms = rooms.filter((room) => {
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
      default:
        return 0;
    }
  });

  if (load) {
    return <p>Đang tải phòng...</p>;
  }

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
