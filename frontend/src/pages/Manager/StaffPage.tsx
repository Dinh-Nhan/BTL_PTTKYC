import { useAuth } from "@/contexts/auth-context";
import type { Staff } from "@/lib/mock-data";
import { Suspense, useState } from "react";
import { staffMembers as initialStaff } from "@/lib/mock-data";
import { toast } from "sonner";
import Header from "@/components/admin/layout/header";
import { Plus, Search } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import StaffTable from "@/components/admin/staff/staff-table";
import StaffModal from "@/components/admin/staff/staff-modal";
import ConfirmDialog from "@/components/admin/shared/confirm-dialog";
import { redirect } from "react-router-dom";

const StaffPage = () => {
  const { isAdmin } = useAuth();
  const [staffList, setStaffList] = useState<Staff[]>(initialStaff);
  const [filteredStaff, setFilteredStaff] = useState<Staff[]>(initialStaff);
  const [showModal, setShowModal] = useState(false);
  const [editingStaff, setEditingStaff] = useState<Staff | null>(null);
  const [deletingStaff, setDeletingStaff] = useState<Staff | null>(null);

  // Only admin can access this page
  if (!isAdmin) {
    redirect("/");
  }

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

  const handleSaveStaff = (staff: Staff) => {
    if (editingStaff) {
      // Update existing
      const updated = staffList.map((s) => (s.id === staff.id ? staff : s));
      setStaffList(updated);
      setFilteredStaff(updated);
      toast.success("Staff updated successfully");
    } else {
      // Add new
      const newStaff = { ...staff, id: `S${String(Date.now()).slice(-4)}` };
      setStaffList((prev) => [newStaff, ...prev]);
      setFilteredStaff((prev) => [newStaff, ...prev]);
      toast.success("Staff added successfully");
    }
    setShowModal(false);
    setEditingStaff(null);
  };

  const handleDeleteStaff = () => {
    if (!deletingStaff) return;

    const updated = staffList.filter((s) => s.id !== deletingStaff.id);
    setStaffList(updated);
    setFilteredStaff(updated);
    setDeletingStaff(null);
    toast.success("Staff deleted successfully");
  };

  const handleEdit = (staff: Staff) => {
    setEditingStaff(staff);
    setShowModal(true);
  };

  return (
    <div className="flex flex-col h-full">
      <Header title="Staff Management" />
      <div className="flex-1 p-6 space-y-4">
        <div className="flex flex-col sm:flex-row gap-3 justify-between">
          <div className="relative max-w-sm flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              placeholder="Search staff..."
              onChange={(e) => handleSearch(e.target.value)}
              className="pl-9"
            />
          </div>
          <Button onClick={() => setShowModal(true)}>
            <Plus className="w-4 h-4 mr-2" />
            Add Staff
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
        title="Delete Staff"
        description={`Are you sure you want to delete ${deletingStaff?.name}? This action cannot be undone.`}
        onConfirm={handleDeleteStaff}
        confirmText="Delete"
        variant="destructive"
      />
    </div>
  );
};

export default StaffPage;
