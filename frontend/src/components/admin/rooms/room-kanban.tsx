import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { formatVND } from "@/lib/format";
import type { Room } from "@/lib/mock-data";
import { cn } from "@/lib/utils";
import {
  Bath,
  Building2,
  GripVertical,
  Pencil,
  Tv,
  Wifi,
  Wind,
  Wine,
} from "lucide-react";
import { useEffect, useState } from "react";
import roomTypeApi from "@/api/roomTypeApi";

interface RoomKanbanProps {
  rooms: Room[];
  onEdit: (room: Room) => void;
  onStatusChange: (roomId: string, newStatus: Room["status"]) => void;
}

interface RoomType {
  roomTypeId: number;
  typeName: string;
}

const RoomKanban = ({ rooms, onEdit, onStatusChange }: RoomKanbanProps) => {
  const [draggedRoom, setDraggedRoom] = useState<Room | null>(null);
  const [dragOverStatus, setDragOverStatus] = useState<Room["status"] | null>(
    null
  );

  const [typeRooms, setTypeRooms] = useState<RoomType[]>([]);

  useEffect(() => {
    const getRoomType = async () => {
      try {
        const res = await roomTypeApi.getAll();
        // Cấu trúc API của bạn: res.data.result
        setTypeRooms(res.data.result);
      } catch (error) {
        console.error("Error fetching room types:", error);
      }
    };
    getRoomType(); 
  }, []);

  
  const statusConfig = {
    available: {
      label: "Có sẵn",
      color: "bg-success/10 border-success/30",
      headerColor: "bg-success text-success-foreground",
      badgeVariant: "default" as const,
    },
    booked: {
      label: "Đã đặt",
      color: "bg-primary/10 border-primary/30",
      headerColor: "bg-primary text-primary-foreground",
      badgeVariant: "secondary" as const,
    },
    maintenance: {
      label: "Bảo trì",
      color: "bg-warning/10 border-warning/30",
      headerColor: "bg-warning text-warning-foreground",
      badgeVariant: "outline" as const,
    },
  };

  const typeLabels = typeRooms.reduce((acc, curr) => {
    acc[curr.roomTypeId] = curr.typeName;
    return acc;
  }, {} as Record<number | string, string>);

  // const typeLabels: Record<Room["type"], string> = {
  //   single: "Phòng đơn",
  //   double: "Phòng đôi",
  //   suite: "Suite",
  //   deluxe: "Deluxe",
  // };

  const amenityIcons: Record<string, JSX.Element> = {
    WiFi: <Wifi className="h-3 w-3" />,
    TV: <Tv className="h-3 w-3" />,
    AC: <Wind className="h-3 w-3" />,
    "Mini Bar": <Wine className="h-3 w-3" />,
    Jacuzzi: <Bath className="h-3 w-3" />,
    Balcony: <Building2 className="h-3 w-3" />,
  };



  const columns: Room["status"][] = ["available", "booked", "maintenance"];

  const handleDragStart = (e: React.DragEvent, room: Room) => {
    setDraggedRoom(room);
    e.dataTransfer.effectAllowed = "move";
  };

  const handleDragEnd = () => {
    setDraggedRoom(null);
    setDragOverStatus(null);
  };

  const handleDragOver = (e: React.DragEvent, status: Room["status"]) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = "move";
    setDragOverStatus(status);
  };

  const handleDragLeave = () => {
    setDragOverStatus(null);
  };

  const handleDrop = (e: React.DragEvent, status: Room["status"]) => {
    e.preventDefault();
    if (draggedRoom && draggedRoom.status !== status) {
      onStatusChange(draggedRoom.id, status);
    }
    setDraggedRoom(null);
    setDragOverStatus(null);
  };

  const getRoomsByStatus = (status: Room["status"]) =>
    rooms.filter((room) => room.status === status);

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6 h-full">
      {columns.map((status) => {
        const config = statusConfig[status];
        const statusRooms = getRoomsByStatus(status);

        return (
          <div
            key={status}
            className={cn(
              "flex flex-col rounded-xl border-2 transition-all duration-200",
              config.color,
              dragOverStatus === status && "ring-2 ring-primary ring-offset-2"
            )}
            onDragOver={(e) => handleDragOver(e, status)}
            onDragLeave={handleDragLeave}
            onDrop={(e) => handleDrop(e, status)}
          >
            <div className={cn("px-4 py-3 rounded-t-lg", config.headerColor)}>
              <div className="flex items-center justify-between">
                <h3 className="font-semibold">{config.label}</h3>
                <Badge
                  variant="secondary"
                  className="bg-background/20 text-inherit"
                >
                  {statusRooms.length}
                </Badge>
              </div>
            </div>

            <div className="flex-1 p-3 space-y-3 overflow-y-auto min-h-[400px] max-h-[calc(100vh-320px)]">
              {statusRooms.length === 0 ? (
                <div className="flex items-center justify-center h-24 text-muted-foreground text-sm border-2 border-dashed rounded-lg">
                  Không có phòng
                </div>
              ) : (
                statusRooms.map((room) => (
                  <Card
                    key={room.id}
                    draggable
                    onDragStart={(e) => handleDragStart(e, room)}
                    onDragEnd={handleDragEnd}
                    className={cn(
                      "cursor-grab active:cursor-grabbing transition-all duration-200 hover:shadow-md",
                      draggedRoom?.id === room.id && "opacity-50 scale-95"
                    )}
                  >
                    <CardHeader className="p-3 pb-2">
                      <div className="flex items-start justify-between gap-2">
                        <div className="flex items-center gap-2">
                          <GripVertical className="h-4 w-4 text-muted-foreground" />
                          <CardTitle className="text-lg font-bold">
                            Phòng {room.number}
                          </CardTitle>
                        </div>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-7 w-7"
                          onClick={(e) => {
                            e.stopPropagation();
                            onEdit(room);
                          }}
                        >
                          <Pencil className="h-3.5 w-3.5" />
                          <span className="sr-only">Chỉnh sửa</span>
                        </Button>
                      </div>
                    </CardHeader>

                    <CardContent className="p-3 pt-0 space-y-3">
                      <div className="flex items-center justify-between">
                        <Badge variant="outline" className="text-xs">
                          {/* 4. Hiển thị tên loại phòng dựa trên ID */}
                          {typeLabels[room.type] || "Chưa xác định"}
                        </Badge>
                        <span className="text-sm font-semibold text-primary">
                          {formatVND(room.price)}/đêm
                        </span>
                      </div>

                      <div className="text-xs text-muted-foreground">
                        Tầng {room.floor}
                      </div>

                      <div className="flex flex-wrap gap-1.5">
                        {room.amenities.slice(0, 4).map((amenity) => (
                          <div
                            key={amenity}
                            className="flex items-center gap-1 px-2 py-1 bg-muted rounded-md text-xs"
                            title={amenity}
                          >
                            {amenityIcons[amenity]}
                            <span className="hidden sm:inline">{amenity}</span>
                          </div>
                        ))}
                        {room.amenities.length > 4 && (
                          <div className="px-2 py-1 bg-muted rounded-md text-xs text-muted-foreground">
                            +{room.amenities.length - 4}
                          </div>
                        )}
                      </div>
                    </CardContent>
                  </Card>
                ))
              )}
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default RoomKanban;
