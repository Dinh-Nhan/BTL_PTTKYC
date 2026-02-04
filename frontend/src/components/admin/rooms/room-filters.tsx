import { useState } from "react";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Search } from "lucide-react";

interface RoomFiltersProps {
  onFilter: (filters: { type: string; status: string; search: string }) => void;
}

const RoomFilters = ({ onFilter }: RoomFiltersProps) => {
  const [type, setType] = useState("all");
  const [status, setStatus] = useState("all");
  const [search, setSearch] = useState("");

  const handleTypeChange = (value: string) => {
    setType(value);
    onFilter({ type: value, status, search });
  };

  const handleStatusChange = (value: string) => {
    setStatus(value);
    onFilter({ type, status: value, search });
  };

  const handleSearchChange = (value: string) => {
    setSearch(value);
    onFilter({ type, status, search: value });
  };

  return (
    <div className="flex flex-cols sm:flex-row gap-3">
      <div className="relative flex-1 max-w-sm">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
        <Input
          placeholder="Tìm kiếm theo số phòng..."
          value={search}
          onChange={(e) => handleSearchChange(e.target.value)}
          className="pl-10"
        />
      </div>

      <Select value={type} onValueChange={handleTypeChange}>
        <SelectTrigger className="w-full sm:w-40">
          <SelectValue placeholder="Tất cả kiểu phòng" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="all">Tất cả</SelectItem>
          <SelectItem value="single">Tiêu chuẩn</SelectItem>
          <SelectItem value="double">Cao cấp</SelectItem>
          <SelectItem value="suite">Siêu cao cấp</SelectItem>
          <SelectItem value="deluxe">Gia đình</SelectItem>
        </SelectContent>
      </Select>

      <Select value={type} onValueChange={handleStatusChange}>
        <SelectTrigger className="w-full sm:w-40">
          <SelectValue placeholder="Trạng thái" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="all">Tất cả</SelectItem>
          <SelectItem value="available">Có sẵn</SelectItem>
          <SelectItem value="booked">Đã đặt</SelectItem>
          <SelectItem value="maintenance">Bảo trì</SelectItem>
        </SelectContent>
      </Select>
    </div>
  );
};

export default RoomFilters;
