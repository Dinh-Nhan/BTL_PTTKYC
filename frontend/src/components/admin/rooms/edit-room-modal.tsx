import { useEffect, useState } from "react";
import { toast } from "sonner";
import type { Room } from "@/lib/mock-data";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface EditRoomModalProps {
  room: Room | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSave: (room: Room) => void;
}

const EditRoomModal = ({
  room,
  open,
  onOpenChange,
  onSave,
}: EditRoomModalProps) => {
  const [formData, setFormData] = useState<Room | null>(null);
  const [loading, setLoading] = useState(false);

  // eslint-disable-next-line react-hooks/exhaustive-deps
  useEffect(() => {
    if (room) {
      setFormData({ ...room });
    }
  }, [room]);

  const handleSave = async () => {
    if (!formData) return;

    setLoading(true);
    await new Promise((resolve) => setTimeout(resolve, 500));
    setLoading(false);

    onSave(formData);
    toast.success(`Phòng ${formData.number} cập nhật thành công`);
  };

  if (!formData) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Chỉnh sửa phòng{formData.number}</DialogTitle>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="type">Kiểu phòng</Label>
              <Select
                value={formData.type}
                onValueChange={(value) =>
                  setFormData({ ...formData, type: value as Room["type"] })
                }
              >
                <SelectTrigger id="type">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="single">Tiêu chuẩn</SelectItem>
                  <SelectItem value="double">Cao cấp</SelectItem>
                  <SelectItem value="suite">Siêu cao cấp</SelectItem>
                  <SelectItem value="deluxe">Gia đình</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="status">Trạng thái</Label>
              <Select
                value={formData.status}
                onValueChange={(value) =>
                  setFormData({ ...formData, status: value as Room["status"] })
                }
              >
                <SelectTrigger id="status">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="available">Có sẵn</SelectItem>
                  <SelectItem value="booked">Đã đặt</SelectItem>
                  <SelectItem value="maintenance">Bảo trì</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="price">Giá cho một đêm($)</Label>
              <Input
                id="price"
                type="number"
                value={formData.price}
                onChange={(e) =>
                  setFormData({ ...formData, price: Number(e.target.value) })
                }
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="floor">Tầng</Label>
              <Input
                id="floor"
                type="number"
                value={formData.floor}
                onChange={(e) =>
                  setFormData({ ...formData, floor: Number(e.target.value) })
                }
              />
            </div>
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Hủy
          </Button>
          <Button onClick={handleSave} disabled={loading}>
            {loading ? "Saving..." : "Save Changes"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default EditRoomModal;
