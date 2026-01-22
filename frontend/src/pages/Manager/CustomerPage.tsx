import CustomersTable from "@/components/admin/customers/customers-table";
import Header from "@/components/admin/layout/header";
import ConfirmDialog from "@/components/admin/shared/confirm-dialog";
import { Input } from "@/components/ui/input";
import { useAuth } from "@/contexts/auth-context";
import { customers as initialCustomers, type Customer } from "@/lib/mock-data";
import { Search } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

const CustomerPage = () => {
  const { isAdmin } = useAuth();
  const [customersList, setCustomersList] =
    useState<Customer[]>(initialCustomers);
  const [filteredCustomers, setFilteredCustomers] =
    useState<Customer[]>(initialCustomers);
  const [deletingCustomer, setDeletingCustomer] = useState<Customer | null>(
    null,
  );

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

  const handleDeleteCustomer = () => {
    if (!deletingCustomer) return;

    const updated = customersList.filter((c) => c.id !== deletingCustomer.id);
    setCustomersList(updated);
    setFilteredCustomers(updated);
    setDeletingCustomer(null);
    toast.success("Customer deleted successfully");
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
