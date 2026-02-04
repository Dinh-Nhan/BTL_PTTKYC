import { useAuth } from "@/contexts/auth-context";
import { Suspense, useEffect, useState } from "react";
import { toast } from "sonner";
import Header from "@/components/admin/layout/header";
import { Plus, Search } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import StaffTable from "@/components/admin/staff/staff-table";
import StaffModal from "@/components/admin/staff/staff-modal";
import ConfirmDialog from "@/components/admin/shared/confirm-dialog";
import userApi from "@/api/userApi";
import { Navigate } from "react-router-dom";

interface Staff {
  id: string;
  name: string;
  email: string;
  password: string;
  phone: string;
  birth: string;        // yyyy-MM-dd
  role: "0" | "1";      // 0 = admin, 1 = employee
  gender: "0" | "1";    // 0 = nữ, 1 = nam
  status: "0" | "1";    // 0 = inactive, 1 = active
  joinedAt: string;
}

const mapStaffToApi = (staff: Staff) => ({
  email: staff.email,
  passwordHashing: staff.password,
  fullName: staff.name,
  phoneNumber: staff.phone,
  gender: staff.gender === "1",
  dateOfBirth: staff.birth,            // yyyy-MM-dd
  roleId: staff.role === "1",
  isActive: staff.status === "1",
  createdAt: new Date().toISOString(),
});

const mapApiToStaff = (apiStaff: any): Staff => ({
  id: String(apiStaff.userId),
  name: apiStaff.fullName,
  email: apiStaff.email,
  password: "",
  phone: apiStaff.phoneNumber,
  birth: apiStaff.dateOfBirth,
  role: apiStaff.roleId ? "1" : "0",
  gender: apiStaff.gender ? "1" : "0",
  status: apiStaff.isActive ? "1" : "0",
  joinedAt: apiStaff.createdAt,
});

const mapStaffToUpdateApi = (staff: Staff) => ({
  userId: Number(staff.id),
  email: staff.email,
  fullName: staff.name,
  phoneNumber: staff.phone,
  gender: staff.gender === "1",
  dateOfBirth: staff.birth,
  isActive: staff.status === "1",
  updatedAt: new Date().toISOString(),
});

const StaffPage = () => {
  const { isAdmin } = useAuth();
  const [staffList, setStaffList] = useState<Staff[]>([]);
  const [filteredStaff, setFilteredStaff] = useState<Staff[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [editingStaff, setEditingStaff] = useState<Staff | null>(null);
  const [deletingStaff, setDeletingStaff] = useState<Staff | null>(null);

  // Only admin can access this page
  if (!isAdmin) {
    return <Navigate to="/" replace />;
  }

  useEffect(() => {
    const fetchStaff = async () => {
      try {
        const res = await userApi.getAll();
        const staffs: Staff[] = res.data.result.map(mapApiToStaff);

        setStaffList(staffs);
        setFilteredStaff(staffs);
      } catch (error) {
        console.log("Failed to fetch staff:", error);
        toast.error("Lỗi khi lấy dữ liệu nhân viên");
      }
    };

    fetchStaff();
  }, []);

  const handleSearch = (query: string) => {
    if (!query) {
      setFilteredStaff(staffList);
      return;
    }
    const filtered = staffList.filter(
      (s) =>
        s.name.toLowerCase().includes(query.toLowerCase()) ||
        s.email.toLowerCase().includes(query.toLowerCase()) ||
        s.role.toLowerCase().includes(query.toLowerCase()),
    );
    setFilteredStaff(filtered);
  };

  const handleSaveStaff = async (staff: Staff) => {
    try {
      if (editingStaff) {
        // Update existing
        const payload = mapStaffToUpdateApi(staff);

        console.log("EDIT STAFF ID:", staff.id, typeof staff.id);

        console.log(payload);

        await userApi.editUser(staff.id, payload);

        const updated = staffList.map((s) => s.id === staff.id ? staff : s);

        setStaffList(updated);
        setFilteredStaff(updated);
        toast.success("Cập nhật nhân viên thành công");
      } else {
        // Add new
        const payload = mapStaffToApi(staff);

        console.log(payload);

        const res = await userApi.addUser(payload);

        const apiStaff = res.data;

        const newStaff: Staff = {
          id: String(apiStaff.userId),
          name: apiStaff.fullName,
          email: apiStaff.email,
          password: "",
          phone: apiStaff.phoneNumber,
          birth: apiStaff.dateOfBirth,
          role: apiStaff.roleId ? "1" : "0",
          gender: apiStaff.gender ? "1" : "0",
          status: apiStaff.isActive ? "1" : "0",
          joinedAt: apiStaff.createdAt,
        }

        setStaffList((prev) => [newStaff, ...prev]);
        setFilteredStaff((prev) => [newStaff, ...prev]);
        toast.success("Thêm nhân viên thành công");
      }
      setShowModal(false);
      setEditingStaff(null);
    } catch (error) {
      console.error("Lỗi khi lưu nhân viên: ", error);
      toast.error("Lỗi khi lưu nhân viên");
    }
  };

  const handleDeleteStaff = async () => {
    if (!deletingStaff) return;

    try {
      await userApi.deleteUser(deletingStaff.id);

      const updated = staffList.filter(
        (s) => s.id !== deletingStaff.id
      );

      setStaffList(updated);
      setFilteredStaff(updated);
      setDeletingStaff(null);

      toast.success("Xóa nhân viên thành công");
    } catch (error) {
      console.error("Delete staff error:", error);
      toast.error("Xóa nhân viên thất bại");
    }
  };

  const handleEdit = (staff: Staff) => {
    setEditingStaff(staff);
    setShowModal(true);
  };

  return (
    <div className="flex flex-col h-full">
      <Header title="Quản lý nhân viên" />
      <div className="flex-1 p-6 space-y-4">
        <div className="flex flex-col sm:flex-row gap-3 justify-between">
          <div className="relative max-w-sm flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              placeholder="Tìm kiếm nhân viên..."
              onChange={(e) => handleSearch(e.target.value)}
              className="pl-9"
            />
          </div>
          <Button onClick={() => setShowModal(true)}>
            <Plus className="w-4 h-4 mr-2" />
            Thêm nhân viên
          </Button>
        </div>

        <Suspense fallback={null}>
          <StaffTable
            staff={filteredStaff}
            onEdit={handleEdit}
            onDelete={setDeletingStaff}
          />
        </Suspense>
      </div>

      <StaffModal
        staff={editingStaff}
        open={showModal}
        onOpenChange={(open) => {
          setShowModal(open);
          if (!open) setEditingStaff(null);
        }}
        onSave={handleSaveStaff}
      />

      <ConfirmDialog
        open={!!deletingStaff}
        onOpenChange={(open) => !open && setDeletingStaff(null)}
        title="Xóa nhân viên"
        description={`Bạn có chắc muốn xóa ${deletingStaff?.name}? Đây là thao tác không thể hoàn tác.`}
        onConfirm={handleDeleteStaff}
        confirmText="Delete"
        variant="destructive"
      />
    </div>
  );
};

export default StaffPage;
