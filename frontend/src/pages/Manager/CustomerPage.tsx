import CustomersTable from "@/components/admin/customers/customers-table";
import Header from "@/components/admin/layout/header";
import ConfirmDialog from "@/components/admin/shared/confirm-dialog";
import { Input } from "@/components/ui/input";
import { useAuth } from "@/contexts/auth-context";
import { customers as initialCustomers, type Customer } from "@/lib/mock-data";
import { Search } from "lucide-react";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import clientApi from "@/api/clientApi"



const CustomerPage = () => {
  const { isAdmin } = useAuth();
  const [customersList, setCustomersList] =
    useState<Customer[]>(initialCustomers);
  const [filteredCustomers, setFilteredCustomers] =
    useState<Customer[]>(initialCustomers);
  const [deletingCustomer, setDeletingCustomer] = useState<Customer | null>(
    null,
  );

  useEffect(() => {
    const fetchClient = async () => {
      try {
        const res = await clientApi.getAll()

        const clients = res.data.result;
        setCustomersList(clients);
        setFilteredCustomers(clients);
      } catch (error) {
        console.error("Không lấy được danh sách khách hàng: ", error);
        toast.error("Không lấy được danh sách khách hàng");
      }
    }

    fetchClient();
  }, [])

  const handleSearch = (query: string) => {
    if (!query) {
      setFilteredCustomers(customersList);
      return;
    }
    const filtered = customersList.filter(
      (c) =>
        c.name.toLowerCase().includes(query.toLowerCase()) ||
        c.email.toLowerCase().includes(query.toLowerCase()) ||
        c.phone.includes(query),
    );
    setFilteredCustomers(filtered);
  };

  const handleDeleteCustomer = async () => {
    if (!deletingCustomer) return;

    try {
      await clientApi.deleteClient(deletingCustomer.id);

      const updated = customersList.filter((s) => s.id !== deletingCustomer.id)
      setCustomersList(updated);
      setFilteredCustomers(updated);
      setDeletingCustomer(null);
      toast.success("Customer deleted successfully");
    } catch (error) {
      console.log("Xoá khách hàng không thành công: ", error);
      toast.error("Xoá khách hàng không thành công");
    }
  };

  return (
    <div className="flex flex-col h-full">
      <Header title="Customer Management" />
      <div className="flex-1 p-6 space-y-4">
        <div className="relative max-w-sm">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <Input
            placeholder="Search by name, email, or phone..."
            onChange={(e) => handleSearch(e.target.value)}
            className="pl-9"
          />
        </div>

        <CustomersTable
          customers={filteredCustomers}
          onDelete={isAdmin ? setDeletingCustomer : undefined}
          isAdmin={isAdmin}
        />
      </div>

      <ConfirmDialog
        open={!!deletingCustomer}
        onOpenChange={(open) => !open && setDeletingCustomer(null)}
        title="Delete Customer"
        description={`Are you sure you want to delete ${deletingCustomer?.name}? This action cannot be undone.`}
        onConfirm={handleDeleteCustomer}
        confirmText="Delete"
        variant="destructive"
      />
    </div>
  );
};

export default CustomerPage;
