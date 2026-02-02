import { Card, CardContent } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Pencil } from "lucide-react";
import type { Room } from "@/lib/mock-data";
import { formatVND } from "@/lib/format";

// Định nghĩa kiểu props cho RoomTable
interface RoomsTableProps {
  rooms: Room[];
  onEdit: (room: Room) => void;
}

// CSS cho trạng thái phòng
const statusStyles = {
  available: "bg-success/10 text-success border-success/20",
  booked: "bg-primary/10 text-primary border-primary/20",
  maintenance: "bg-warning/10 text-warning border-warning/20",
};

// Định dạng lại nhãn loại phòng
const typeLabels = {
  single: "Single",
  double: "Double",
  suite: "Suite",
  deluxe: "Deluxe",
};

const RoomsTable = ({ rooms, onEdit }: RoomsTableProps) => {
  return (
    <Card className="border shadow-sm">
      <CardContent className="p-0">
        <Table>
          <TableHeader>
            <TableRow className="bg-muted/50">
              <TableHead>Phòng số</TableHead>
              <TableHead>Kiểu</TableHead>
              <TableHead>Tầng</TableHead>
              <TableHead>Giá/Đêm</TableHead>
              <TableHead>Trạng thái</TableHead>
              <TableHead>Tiện ích</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {rooms.length === 0 ? (
              <TableRow>
                <TableCell
                  colSpan={7}
                  className="text-center py-8 text-muted-foreground"
                >
                  Không tìm thấy phòng
                </TableCell>
              </TableRow>
            ) : (
              rooms.map((room) => (
                <TableRow key={room.id}>
                  <TableCell className="font-medium">{room.number}</TableCell>
                  <TableCell>{typeLabels[room.type]}</TableCell>
                  <TableCell>{room.floor}</TableCell>
                  <TableCell>{formatVND(room.price)}</TableCell>
                  <TableCell>
                    <Badge
                      variant="outline"
                      className={statusStyles[room.status]}
                    >
                      {room.status.charAt(0).toUpperCase() +
                        room.status.slice(1)}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    <span className="text-sm text-muted-foreground">
                      {room.amenities.slice(0, 3).join(", ")}
                      {room.amenities.length > 3 &&
                        ` +${room.amenities.length - 3}`}
                    </span>
                  </TableCell>
                  <TableCell className="text-right">
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => onEdit(room)}
                    >
                      <Pencil className="w-4 h-4 mr-1" />
                      Chỉnh sửa
                    </Button>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  );
};

export default RoomsTable;
