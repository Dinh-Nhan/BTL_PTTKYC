import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type { Staff } from "@/lib/mock-data";
import { useEffect, useState } from "react";

interface StaffModalProps {
  staff: Staff | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSave: (staff: Staff) => void;
}

const defaultStaff: Staff = {
  id: "",
  name: "",
  email: "",
  password: "",
  phone: "",
  birth: "2005-08-31",
  role: "1",
  gender: "1",
  status: "1",
  joinedAt: new Date().toISOString().split("T")[0],
};

const StaffModal = ({ staff, open, onOpenChange, onSave }: StaffModalProps) => {
  const [formData, setFormData] = useState<Staff>(defaultStaff);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (staff) {
      setFormData(staff);
    } else {
      setFormData(defaultStaff);
    }
  }, [staff, open]);

  const handleSave = async () => {
    setLoading(true);
    await new Promise((resolve) => setTimeout(resolve, 500));
    setLoading(false);
    onSave(formData);
  };

  const isValid = formData.name && formData.email && formData.phone;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>{staff ? "Chỉnh sửa" : "Thêm nhân viên"}</DialogTitle>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <div className="space-y-2">
            <Label htmlFor="name">Họ tên đầy đủ</Label>
            <Input
              id="name"
              value={formData.name}
              onChange={(e) =>
                setFormData({ ...formData, name: e.target.value })
              }
              placeholder="Nhập đầy đủ họ tên"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input
              id="email"
              type="email"
              value={formData.email}
              onChange={(e) =>
                setFormData({ ...formData, email: e.target.value })
              }
              placeholder="Nhập địa chỉ email"
            />
          </div>

          {!staff && (
            <div className="space-y-2">
              <Label htmlFor="password">Mật khẩu</Label>
              <Input
                id="password"
                type="password"
                value={formData.password}
                onChange={(e) =>
                  setFormData({ ...formData, password: e.target.value })
                }
                placeholder="Nhập mật khẩu"
              />
            </div>
          )}

          <div className="space-y-2">
            <Label htmlFor="phone">Số điện thoại</Label>
            <Input
              id="phone"
              value={formData.phone}
              onChange={(e) =>
                setFormData({ ...formData, phone: e.target.value })
              }
              placeholder="Nhập số điện thoại"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="birth">Ngày sinh</Label>
            <Input
              id="birth"
              type="date"
              value={formData.birth}
              onChange={(e) =>
                setFormData({ ...formData, birth: e.target.value })
              }
            />
          </div>


          <div className="grid grid-cols-2 gap-4">

            {!staff && (
              <div className="space-y-2">
                <Label>Quyền</Label>
                <Select
                  value={formData.role}
                  onValueChange={(value) =>
                    setFormData({ ...formData, role: value as Staff["role"] })
                  }
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="0">Admin</SelectItem>
                    <SelectItem value="1">Nhân viên</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            )
            }

            <div className="space-y-2">
              <Label>Giới tính</Label>
              <Select
                value={formData.gender}
                onValueChange={(value) =>
                  setFormData({ ...formData, gender: value as Staff["gender"] })
                }
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="0">Nữ</SelectItem>
                  <SelectItem value="1">Nam</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label>Trạng thái</Label>
              <Select
                value={formData.status}
                onValueChange={(value) =>
                  setFormData({ ...formData, status: value as Staff["status"] })
                }
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="1">Active</SelectItem>
                  <SelectItem value="0">Inactive</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Hủy
          </Button>
          <Button onClick={handleSave} disabled={loading || !isValid}>
            {loading ? "Saving..." : staff ? "Cập nhật" : "Thêm"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default StaffModal;
