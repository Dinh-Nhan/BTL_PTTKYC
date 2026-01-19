import { useState } from "react";
import BookingModal from "./BookingModal";
import { Star } from "lucide-react";
import { formatVND } from "@/lib/format";
import { Card } from "../ui/card";

interface RoomCardProps {
  room: {
    id: number;
    name: string;
    description: string;
    price: number;
    rating: number;
    reviews: number;
    guests: number;
    type: string;
    image: string;
    amenities: string[];
  };
}

const RoomCard = ({ room }: RoomCardProps) => {
  const [showBooking, setShowBooking] = useState(false);

  return (
    <>
      <Card className="overflow-hidden border-border bg-card hover:shadow-md transition-shadow">
        <div className="aspect-video bg-secondary overflow-hidden">
          <img
            src={room.image || "/placeholder.svg"}
            alt={room.name}
            className="h-full w-full object-cover hover:scale-105 transition-transform duration-300"
          />
        </div>

        <div className="space-y-4 p-4 sm:p-5">
          {/* Room Name and Rating */}
          <div className="space-y-2">
            <h3 className="text-lg font-medium text-foreground">{room.name}</h3>
            <div className="flex items-center gap-2">
              <div className="flex items-center gap-1">
                <Star className="h-4 w-4 fill-accent text-accent" />
                <span className="text-sm font-medium text-foreground">
                  {room.rating}
                </span>
              </div>
              <span className="text-xs text-muted-foreground">
                ({room.reviews} bài đánh giá)
              </span>
            </div>
          </div>

          {/* Description */}
          <p className="text-sm text-muted-foreground leading-relaxed">
            {room.description}
          </p>

          {/* Amenities */}
          <div className="flex flex-wrap gap-2">
            {room.amenities.slice(0, 3).map((amenity) => (
              <span
                key={amenity}
                className="inline-block rounded-full bg-secondary px-2.5 py-1 text-xs text-foreground"
              >
                {amenity}
              </span>
            ))}
            {room.amenities.length > 3 && (
              <span className="inline-block text-xs text-muted-foreground pt-1">
                +{room.amenities.length - 3}
              </span>
            )}
          </div>

          {/* Guests Info */}
          <div className="text-xs text-muted-foreground">
            Tối đa {room.guests} khách
          </div>

          {/* Price and CTA */}
          <div className="flex items-center justify-between border-t border-border pt-4">
            <div>
              <p className="text-xs text-muted-foreground">Chỉ từ</p>
              <p className="text-2xl font-light text-foreground">
                {formatVND(room.price)}
                <span className="text-xs text-muted-foreground">/đêm</span>
              </p>
            </div>
            <button
              onClick={() => setShowBooking(true)}
              className="rounded-md bg-accent px-5 py-2.5 text-sm font-medium text-accent-foreground hover:bg-accent/90 transition-colors"
            >
              Đặt phòng
            </button>
          </div>
        </div>
      </Card>

      {showBooking && (
        <BookingModal room={room} onClose={() => setShowBooking(false)} />
      )}
    </>
  );
};

export default RoomCard;
