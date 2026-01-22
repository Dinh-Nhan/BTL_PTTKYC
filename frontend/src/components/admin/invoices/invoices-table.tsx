import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { formatVND } from "@/lib/format";
import type { Invoice } from "@/lib/mock-data";
import { CreditCard, Eye, RotateCcw } from "lucide-react";

interface InvoicesTableProps {
  invoices: Invoice[];
  onView: (invoice: Invoice) => void;
  onPay: (invoice: Invoice) => void;
  onRefund: (invoice: Invoice) => void;
}

const statusStyles = {
  paid: "bg-success/10 text-success border-success/20",
  unpaid: "bg-warning/10 text-warning border-warning/20",
  refunded: "bg-muted text-muted-foreground border-muted",
};

const InvoicesTable = ({
  invoices,
  onView,
  onPay,
  onRefund,
}: InvoicesTableProps) => {
  return (
    <Card className="border shadow-sm">
      <CardContent className="p-0">
        <Table>
          <TableHeader>
            <TableRow className="bg-muted/50">
              <TableHead>ID hóa đơn</TableHead>
              <TableHead>ID đặt phòng</TableHead>
              <TableHead>Khách hàng</TableHead>
              <TableHead>Phòng</TableHead>
              <TableHead>Tổng tiền</TableHead>
              <TableHead>Trạng thái</TableHead>
              <TableHead>Đã tạo</TableHead>
              <TableHead className="text-right">Hành động</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {invoices.length === 0 ? (
              <TableRow>
                <TableCell
                  colSpan={8}
                  className="text-center py-8 text-muted-foreground"
                >
                  Không tìm thấy hóa đơn
                </TableCell>
              </TableRow>
            ) : (
              invoices.map((invoice) => (
                <TableRow key={invoice.id}>
                  <TableCell className="font-medium">{invoice.id}</TableCell>
                  <TableCell>{invoice.bookingId}</TableCell>
                  <TableCell>{invoice.customerName}</TableCell>
                  <TableCell>{invoice.roomNumber}</TableCell>
                  <TableCell className="font-medium">
                    {formatVND(invoice.amount)}
                  </TableCell>
                  <TableCell>
                    <Badge
                      variant="outline"
                      className={statusStyles[invoice.status]}
                    >
                      {invoice.status.charAt(0).toUpperCase() +
                        invoice.status.slice(1)}
                    </Badge>
                  </TableCell>
                  <TableCell>{invoice.createdAt}</TableCell>
                  <TableCell className="text-right">
                    <div className="flex items-center justify-end gap-1">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => onView(invoice)}
                      >
                        <Eye className="w-4 h-4" />
                      </Button>
                      {invoice.status === "unpaid" && (
                        <Button
                          variant="ghost"
                          size="sm"
                          className="text-success hover:text-success"
                          onClick={() => onPay(invoice)}
                        >
                          <CreditCard className="w-4 h-4" />
                        </Button>
                      )}
                      {invoice.status === "paid" && (
                        <Button
                          variant="ghost"
                          size="sm"
                          className="text-muted-foreground hover:text-foreground"
                          onClick={() => onRefund(invoice)}
                        >
                          <RotateCcw className="w-4 h-4" />
                        </Button>
                      )}
                    </div>
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

export default InvoicesTable;
