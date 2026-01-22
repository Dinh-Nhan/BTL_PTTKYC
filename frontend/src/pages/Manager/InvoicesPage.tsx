import { invoices as initialInvoices, type Invoice } from "@/lib/mock-data";
import { toast } from "sonner";
import Header from "@/components/admin/layout/header";
import { Search } from "lucide-react";
import { Input } from "@/components/ui/input";
import InvoicesTable from "@/components/admin/invoices/invoices-table";
import InvoiceDetailsDrawer from "@/components/admin/invoices/invoice-details-drawer";
import ConfirmDialog from "@/components/admin/shared/confirm-dialog";
import { useState } from "react";

const InvoicesPage = () => {
  const [invoicesList, setInvoicesList] = useState<Invoice[]>(initialInvoices);
  const [filteredInvoices, setFilteredInvoices] =
    useState<Invoice[]>(initialInvoices);
  const [viewingInvoice, setViewingInvoice] = useState<Invoice | null>(null);
  const [payingInvoice, setPayingInvoice] = useState<Invoice | null>(null);
  const [refundingInvoice, setRefundingInvoice] = useState<Invoice | null>(
    null,
  );

  const handleSearch = (query: string) => {
    if (!query) {
      setFilteredInvoices(invoicesList);
      return;
    }
    const filtered = invoicesList.filter(
      (inv) =>
        inv.id.toLowerCase().includes(query.toLowerCase()) ||
        inv.customerName.toLowerCase().includes(query.toLowerCase()) ||
        inv.roomNumber.includes(query),
    );
    setFilteredInvoices(filtered);
  };

  const handlePayInvoice = () => {
    if (!payingInvoice) return;

    const updated = invoicesList.map((inv) =>
      inv.id === payingInvoice.id
        ? {
            ...inv,
            status: "paid" as const,
            paidAt: new Date().toISOString().split("T")[0],
          }
        : inv,
    );
    setInvoicesList(updated);
    setFilteredInvoices(
      updated.filter((inv) => filteredInvoices.some((f) => f.id === inv.id)),
    );
    setPayingInvoice(null);
    toast.success("Payment processed successfully");
  };

  const handleRefundInvoice = () => {
    if (!refundingInvoice) return;

    const updated = invoicesList.map((inv) =>
      inv.id === refundingInvoice.id
        ? { ...inv, status: "refunded" as const }
        : inv,
    );
    setInvoicesList(updated);
    setFilteredInvoices(
      updated.filter((inv) => filteredInvoices.some((f) => f.id === inv.id)),
    );
    setRefundingInvoice(null);
    toast.success("Refund processed successfully");
  };

  return (
    <div className="flex flex-col h-full">
      <Header title="Invoice Management" />
      <div className="flex-1 p-6 space-y-4">
        <div className="relative max-w-sm">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <Input
            placeholder="Search by ID, customer, or room..."
            onChange={(e) => handleSearch(e.target.value)}
            className="pl-9"
          />
        </div>

        <InvoicesTable
          invoices={filteredInvoices}
          onView={setViewingInvoice}
          onPay={setPayingInvoice}
          onRefund={setRefundingInvoice}
        />
      </div>

      <InvoiceDetailsDrawer
        invoice={viewingInvoice}
        open={!!viewingInvoice}
        onOpenChange={(open) => !open && setViewingInvoice(null)}
      />

      <ConfirmDialog
        open={!!payingInvoice}
        onOpenChange={(open) => !open && setPayingInvoice(null)}
        title="Process Payment"
        description={`Process payment of $${payingInvoice?.amount} for invoice ${payingInvoice?.id}?`}
        onConfirm={handlePayInvoice}
        confirmText="Process Payment"
      />

      <ConfirmDialog
        open={!!refundingInvoice}
        onOpenChange={(open) => !open && setRefundingInvoice(null)}
        title="Process Refund"
        description={`Process refund of $${refundingInvoice?.amount} for invoice ${refundingInvoice?.id}? This action cannot be undone.`}
        onConfirm={handleRefundInvoice}
        confirmText="Process Refund"
        variant="destructive"
      />
    </div>
  );
};

export default InvoicesPage;
