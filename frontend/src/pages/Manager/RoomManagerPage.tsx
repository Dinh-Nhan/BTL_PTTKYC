import Header from "@/components/admin/layout/header";
import EditRoomModal from "@/components/admin/rooms/edit-room-modal";
import RoomKanban from "@/components/admin/rooms/room-kanban";

import { Input } from "@/components/ui/input";

import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { Search } from "lucide-react";

import { useEffect, useState } from "react";

import { toast } from "sonner";

import roomApi from "@/api/roomApi";

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
  status: RoomStatus;
  note: string;
  roomType: RoomType;
}

/* ================= COMPONENT ================= */

const RoomManagerPage = () => {
  const [rooms, setRooms] = useState<Room[]>([]);

  const [editingRoom, setEditingRoom] =
    useState<Room | null>(null);

  const [searchQuery, setSearchQuery] = useState("");

  const [typeFilter, setTypeFilter] = useState("all");

  const [loading, setLoading] = useState(false);

  /* ================= LOAD API ================= */

  useEffect(() => {
    const getAllRoom = async () => {
      try {
        setLoading(true);

        const res = await roomApi.getAll();

        setRooms(res.data.result || []);
      } catch (err) {
        console.error(err);
        toast.error("Không tải được danh sách phòng");
      } finally {
        setLoading(false);
      }
    };

    getAllRoom();
  }, []);

  /* ================= FILTER ================= */

  const filteredRooms = rooms.filter((room) => {
    /* Search theo số phòng */
    const matchesSearch = room.roomNumber
      .toLowerCase()
      .includes(searchQuery.toLowerCase());

    /* Filter theo loại phòng */
    const matchesType =
      typeFilter === "all" ||
      room.roomType?.typeName === typeFilter;

    return matchesSearch && matchesType;
  });

  /* ================= UPDATE ROOM ================= */

  const handleUpdateRoom = (updatedRoom: Room) => {
    setRooms((prev) =>
      prev.map((r) =>
        r.roomId === updatedRoom.roomId
          ? updatedRoom
          : r
      )
    );

    setEditingRoom(null);
  };

  /* ================= CHANGE STATUS ================= */ 

  const handleStatusChange = async (
    id: number,
    status: RoomStatus
  ) => {
    await roomApi.changeStatusRoom(id, status);

    setRooms((prev) =>
      prev.map((r) =>
        r.roomId === id
          ? { ...r, status }
          : r
      )
    );
  };



  /* ================= RENDER ================= */

  return (
    <div className="flex flex-col h-full">
      <Header title="Quản lý phòng" />

      <div className="flex-1 p-6 flex flex-col gap-4">
        {/* FILTER BAR */}
        <div className="flex flex-col sm:flex-row gap-3">
          {/* SEARCH */}
          <div className="relative flex-1 max-w-sm">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />

            <Input
              placeholder="Tìm số phòng..."
              value={searchQuery}
              onChange={(e) =>
                setSearchQuery(e.target.value)
              }
              className="pl-9"
            />
          </div>

          {/* TYPE FILTER */}
          <Select
            value={typeFilter}
            onValueChange={setTypeFilter}
          >
            <SelectTrigger className="w-full sm:w-[180px]">
              <SelectValue placeholder="Loại phòng" />
            </SelectTrigger>

            <SelectContent>
              <SelectItem value="all">
                Tất cả
              </SelectItem>

              <SelectItem value="Standard">
                Tiêu chuẩn
              </SelectItem>

              <SelectItem value="Deluxe">
                Cao cấp
              </SelectItem>

              <SelectItem value="Suite">
                Suite
              </SelectItem>

              <SelectItem value="Family">
                Gia đình
              </SelectItem>
            </SelectContent>
          </Select>
        </div>

        {/* KANBAN */}
        <div className="flex-1 min-h-0">
          {loading ? (
            <div className="h-full flex items-center justify-center text-muted-foreground">
              Đang tải dữ liệu...
            </div>
          ) : (
            <RoomKanban
              rooms={filteredRooms}
              onEdit={setEditingRoom}
              onStatusChange={handleStatusChange}
            />
          )}
        </div>
      </div>

      {/* EDIT MODAL */}
      <EditRoomModal
        room={editingRoom}
        open={!!editingRoom}
        onOpenChange={(open) =>
          !open && setEditingRoom(null)
        }
        onSave={handleUpdateRoom}
      />
    </div>
  );
};

export default RoomManagerPage;
