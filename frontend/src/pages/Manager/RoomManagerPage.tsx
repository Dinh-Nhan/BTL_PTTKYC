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
import { rooms as initialRooms } from "@/lib/mock-data";
import type { Room } from "@/lib/mock-data";
import { Search } from "lucide-react";
import { useState } from "react";
import { useSearchParams } from "react-router-dom";
import { toast } from "sonner";

const RoomManagerPage = () => {
  const searchParams = useSearchParams();
  const [rooms, setRooms] = useState<Room[]>(initialRooms);
  const [editingRoom, setEditingRoom] = useState<Room | null>(null);
  const [searchQuery, setSearchQuery] = useState("");
  const [typeFilter, setTypeFilter] = useState("all");

  const filteredRooms = rooms.filter((room) => {
    const matchesSearch = room.number
      .toLowerCase()
      .includes(searchQuery.toLowerCase());
    const matchesType = typeFilter === "all" || room.type === typeFilter;
    return matchesSearch && matchesType;
  });

  const handleUpdateRoom = (updatedRoom: Room) => {
    setRooms((prev) =>
      prev.map((r) => (r.id === updatedRoom.id ? updatedRoom : r)),
    );
    setEditingRoom(null);
  };

  const handleStatusChange = (roomId: string, newStatus: Room["status"]) => {
    setRooms((prev) =>
      prev.map((r) => (r.id === roomId ? { ...r, status: newStatus } : r)),
    );
    const room = rooms.find((r) => r.id === roomId);
    if (room) {
      toast.success(`Room ${room.number} moved to ${newStatus}`);
    }
  };

  return (
    <div className="flex flex-col h-full">
      <Header title="Quản lý phòng" />
      <div className="flex-1 p-6 flex flex-col gap-4">
        <div className="flex flex-col sm:flex-row gap-3">
          <div className="relative flex-1 max-w-sm">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Tìm số phòng..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pl-9"
            />
          </div>
          <Select value={typeFilter} onValueChange={setTypeFilter}>
            <SelectTrigger className="w-full sm:w-[160px]">
              <SelectValue placeholder="Room Type" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Tất cả</SelectItem>
              <SelectItem value="single">Phòng đơn</SelectItem>
              <SelectItem value="double">Phòng đôi</SelectItem>
              <SelectItem value="suite">Suite</SelectItem>
              <SelectItem value="deluxe">Deluxe</SelectItem>
            </SelectContent>
          </Select>
        </div>

        <div className="flex-1 min-h-0">
          <RoomKanban
            rooms={filteredRooms}
            onEdit={setEditingRoom}
            onStatusChange={handleStatusChange}
          />
        </div>
      </div>

      <EditRoomModal
        room={editingRoom}
        open={!!editingRoom}
        onOpenChange={(open) => !open && setEditingRoom(null)}
        onSave={handleUpdateRoom}
      />
    </div>
  );
};

export default RoomManagerPage;
