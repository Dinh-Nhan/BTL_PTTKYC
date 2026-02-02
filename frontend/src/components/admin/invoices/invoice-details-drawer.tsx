import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import type { Invoice } from "@/lib/mock-data";

interface InvoiceDetailsDrawerProps {
  invoice: Invoice | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const statusStyles = {
  paid: "bg-success/10 text-success border-success/20",
  unpaid: "bg-warning/10 text-warning border-warning/20",
  refunded: "bg-muted text-muted-foreground border-muted",
};

const InvoiceDetailsDrawer = ({
  invoice,
  open,
  onOpenChange,
}: InvoiceDetailsDrawerProps) => {
  if (!invoice) return null;
  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="sm:max-w-md">
        <SheetHeader>
          <SheetTitle className="flex items-center gap-3">
            Invoice {invoice.id}
            <Badge variant="outline" className={statusStyles[invoice.status]}>
              {invoice.status.charAt(0).toUpperCase() + invoice.status.slice(1)}
            </Badge>
          </SheetTitle>
        </SheetHeader>

        <div className="mt-6 space-y-6">
          {/* Invoice Summary */}
          <div className="p-4 rounded-lg bg-muted/50">
            <div className="text-center">
              <p className="text-sm text-muted-foreground">Total Amount</p>
              <p className="text-3xl font-bold text-foreground">
                ${invoice.amount}
              </p>
            </div>
          </div>

          <Separator />

          {/* Details */}
          <div className="space-y-4">
            <h4 className="font-medium text-sm text-muted-foreground uppercase tracking-wide">
              Invoice Details
            </h4>

            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Booking ID</span>
                <span className="font-medium">{invoice.bookingId}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Customer</span>
                <span className="font-medium">{invoice.customerName}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Room</span>
                <span className="font-medium">{invoice.roomNumber}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Created</span>
                <span className="font-medium">{invoice.createdAt}</span>
              </div>
              {invoice.paidAt && (
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Paid At</span>
                  <span className="font-medium">{invoice.paidAt}</span>
                </div>
              )}
            </div>
          </div>

          <Separator />

          {/* Payment Breakdown */}
          <div className="space-y-4">
            <h4 className="font-medium text-sm text-muted-foreground uppercase tracking-wide">
              Payment Breakdown
            </h4>

            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Room Charges</span>
                <span className="font-medium">${invoice.amount}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-muted-foreground">Tax (10%)</span>
                <span className="font-medium">
                  ${(invoice.amount * 0.1).toFixed(2)}
                </span>
              </div>
              <Separator />
              <div className="flex justify-between text-lg">
                <span className="font-semibold">Grand Total</span>
                <span className="font-bold">
                  ${(invoice.amount * 1.1).toFixed(2)}
                </span>
              </div>
            </div>
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
};

export default InvoiceDetailsDrawer;
