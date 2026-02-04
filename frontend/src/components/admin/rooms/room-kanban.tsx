import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { formatVND } from "@/lib/format";
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

/* ================= TYPE ================= */

type RoomStatus =
  | "available"
  | "occupied"
  | "booked"
  | "cleaning"
  | "maintenance"
  | "inactive";

interface RoomType {
  roomTypeId: number;
  typeName: string;
  basePrice: number;
  amenities: string;
}

interface Room {
  roomId: number;
  roomNumber: string;
  floor: number;
  status: string; // 👈 để string cho an toàn backend
  note: string;

  roomType: RoomType;
}

/* ================= PROPS ================= */

interface RoomKanbanProps {
  rooms: Room[];

  onEdit: (room: Room) => void;

  onStatusChange: (
    roomId: number,
    newStatus: RoomStatus
  ) => Promise<void>;
}

/* ================= CONSTANT ================= */

const VALID_STATUS: RoomStatus[] = [
  "available",
  "occupied",
  "booked",
  "cleaning",
  "maintenance",
  "inactive",
];

/* ================= COMPONENT ================= */

const RoomKanban = ({ rooms, onEdit, onStatusChange }: RoomKanbanProps) => {
  const [draggedRoom, setDraggedRoom] = useState<Room | null>(null);

  const [dragOverStatus, setDragOverStatus] =
    useState<RoomStatus | null>(null);

  /* ================= STATE ================= */

  const [roomsByStatus, setRoomsByStatus] = useState<
    Record<RoomStatus, Room[]>
  >({
    available: [],
    occupied: [],
    booked: [],
    cleaning: [],
    maintenance: [],
    inactive: [],
  });

  /* ================= GROUP DATA ================= */

  useEffect(() => {
    const grouped: Record<RoomStatus, Room[]> = {
      available: [],
      occupied: [],
      booked: [],
      cleaning: [],
      maintenance: [],
      inactive: [],
    };

    rooms.forEach((room) => {
      const status = room.status?.toLowerCase();

      // Nếu status không hợp lệ → cho vào inactive
      const safeStatus: RoomStatus = VALID_STATUS.includes(
        status as RoomStatus
      )
        ? (status as RoomStatus)
        : "inactive";

      grouped[safeStatus].push({
        ...room,
        status: safeStatus,
      });
    });

    setRoomsByStatus(grouped);
  }, [rooms]);

  /* ================= STATUS CONFIG ================= */

  const statusConfig: Record<
    RoomStatus,
    {
      label: string;
      color: string;
      headerColor: string;
    }
  > = {
    available: {
      label: "Có sẵn",
      color: "bg-green-100 border-green-300",
      headerColor: "bg-green-500 text-white",
    },

    occupied: {
      label: "Đang phục vụ",
      color: "bg-blue-100 border-blue-300",
      headerColor: "bg-blue-500 text-white",
    },

    booked: {
      label: "Đã đặt",
      color: "bg-purple-100 border-purple-300",
      headerColor: "bg-purple-500 text-white",
    },

    cleaning: {
      label: "Đang dọn dẹp",
      color: "bg-yellow-100 border-yellow-300",
      headerColor: "bg-yellow-500 text-black",
    },

    maintenance: {
      label: "Bảo trì",
      color: "bg-orange-100 border-orange-300",
      headerColor: "bg-orange-500 text-white",
    },

    inactive: {
      label: "Ngừng hoạt động",
      color: "bg-gray-100 border-gray-300",
      headerColor: "bg-gray-500 text-white",
    },
  };

  const columns = Object.keys(statusConfig) as RoomStatus[];

  /* ================= ICON ================= */

  const amenityIcons: Record<string, JSX.Element> = {
    Wifi: <Wifi className="h-3 w-3" />,
    TV: <Tv className="h-3 w-3" />,
    "Máy lạnh": <Wind className="h-3 w-3" />,
    "Tủ lạnh mini": <Wine className="h-3 w-3" />,
    "Bồn tắm": <Bath className="h-3 w-3" />,
    "Ban công": <Building2 className="h-3 w-3" />,
  };

  /* ================= DRAG ================= */

  const handleDragStart = (
    e: React.DragEvent,
    room: Room
  ) => {
    setDraggedRoom(room);
    e.dataTransfer.effectAllowed = "move";
  };

  const handleDrop = async (
    e: React.DragEvent,
    status: RoomStatus
  ) => {
    e.preventDefault();

    if (!draggedRoom) return;

    const oldStatus = draggedRoom.status as RoomStatus;

    if (oldStatus === status) return;

    moveRoom(draggedRoom.roomId, oldStatus, status);

    try {
      await onStatusChange(draggedRoom.roomId, status);
    } catch (error) {
      console.error(error);

      moveRoom(draggedRoom.roomId, status, oldStatus);

      alert("Cập nhật trạng thái thất bại!");
    }

    setDraggedRoom(null);
    setDragOverStatus(null);
  };

  const handleDragEnd = () => {
    setDraggedRoom(null);
    setDragOverStatus(null);
  };

  /* ================= MOVE ================= */

  const moveRoom = (
    roomId: number,
    from: RoomStatus,
    to: RoomStatus
  ) => {
    setRoomsByStatus((prev) => {
      const source = [...prev[from]];
      const target = [...prev[to]];

      const index = source.findIndex(
        (r) => r.roomId === roomId
      );

      if (index === -1) return prev;

      const [movedRoom] = source.splice(index, 1);

      movedRoom.status = to;

      target.unshift(movedRoom);

      return {
        ...prev,
        [from]: source,
        [to]: target,
      };
    });
  };

  /* ================= RENDER ================= */

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 xl:grid-cols-3 gap-4 h-full">
      {columns.map((status) => {
        const config = statusConfig[status];

        const statusRooms = roomsByStatus[status];

        return (
          <div
            key={status}
            className={cn(
              "flex flex-col rounded-xl border-2",
              config.color,
              dragOverStatus === status &&
                "ring-2 ring-primary ring-offset-2"
            )}
            onDragOver={(e) => {
              e.preventDefault();
              setDragOverStatus(status);
            }}
            onDrop={(e) => handleDrop(e, status)}
          >
            {/* HEADER */}
            <div
              className={cn(
                "px-4 py-3 rounded-t-lg",
                config.headerColor
              )}
            >
              <div className="flex justify-between items-center">
                <h3 className="font-semibold">
                  {config.label}
                </h3>

                <Badge className="bg-white/30 text-white">
                  {statusRooms.length}
                </Badge>
              </div>
            </div>

            {/* BODY */}
            <div className="flex-1 p-3 space-y-3 overflow-y-auto min-h-[400px]">
              {statusRooms.length === 0 ? (
                <div className="h-24 flex items-center justify-center text-sm text-muted-foreground border border-dashed rounded-lg">
                  Không có phòng
                </div>
              ) : (
                statusRooms.map((room) => {
                  const amenities =
                    room.roomType?.amenities
                      ?.split(",")
                      .map((a) => a.trim()) || [];

                  return (
                    <Card
                      key={room.roomId}
                      draggable
                      onDragStart={(e) =>
                        handleDragStart(e, room)
                      }
                      onDragEnd={handleDragEnd}
                      className="cursor-grab hover:shadow-md"
                    >
                      <CardHeader className="p-3 pb-2">
                        <div className="flex justify-between items-start">
                          <div className="flex gap-2 items-center">
                            <GripVertical className="h-4 w-4" />

                            <CardTitle className="text-base">
                              Phòng {room.roomNumber}
                            </CardTitle>
                          </div>

                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() => onEdit(room)}
                          >
                            <Pencil className="h-4 w-4" />
                          </Button>
                        </div>
                      </CardHeader>

                      <CardContent className="p-3 pt-0 space-y-2">
                        {/* TYPE + PRICE */}
                        <div className="flex justify-between">
                          <Badge
                            variant="outline"
                            className="text-xs"
                          >
                            {room.roomType?.typeName}
                          </Badge>

                          <span className="text-sm font-semibold text-primary">
                            {formatVND(
                              room.roomType?.basePrice || 0
                            )}
                          </span>
                        </div>

                        {/* FLOOR */}
                        <div className="text-xs text-muted-foreground">
                          Tầng {room.floor}
                        </div>

                        {/* AMENITIES */}
                        <div className="flex flex-wrap gap-1">
                          {amenities
                            .slice(0, 4)
                            .map((a) => (
                              <div
                                key={a}
                                className="flex gap-1 items-center px-2 py-1 bg-muted rounded text-xs"
                              >
                                {amenityIcons[a]}
                                {a}
                              </div>
                            ))}
                        </div>
                      </CardContent>
                    </Card>
                  );
                })
              )}
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default RoomKanban;
