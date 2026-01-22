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
import type { Customer } from "@/lib/mock-data";
import { Trash2 } from "lucide-react";

interface CustomersTableProps {
  customers: Customer[];
  onDelete?: (customer: Customer) => void;
  isAdmin: boolean;
}

const CustomersTable = ({
  customers,
  onDelete,
  isAdmin,
}: CustomersTableProps) => {
  return (
    <Card className="border shadow-sm">
      <CardContent className="p-0">
        <Table>
          <TableHeader>
            <TableRow className="bg-muted/50">
              <TableHead>Name</TableHead>
              <TableHead>Email</TableHead>
              <TableHead>Phone</TableHead>
              <TableHead>ID Number</TableHead>
              <TableHead>Total Bookings</TableHead>
              <TableHead>Member Since</TableHead>
              {isAdmin && <TableHead className="text-right">Actions</TableHead>}
            </TableRow>
          </TableHeader>
          <TableBody>
            {customers.length === 0 ? (
              <TableRow>
                <TableCell
                  colSpan={isAdmin ? 7 : 6}
                  className="text-center py-8 text-muted-foreground"
                >
                  No customers found
                </TableCell>
              </TableRow>
            ) : (
              customers.map((customer) => (
                <TableRow key={customer.id}>
                  <TableCell className="font-medium">{customer.name}</TableCell>
                  <TableCell>{customer.email}</TableCell>
                  <TableCell>{customer.phone}</TableCell>
                  <TableCell className="font-mono text-sm">
                    {customer.idNumber}
                  </TableCell>
                  <TableCell>
                    <span className="inline-flex items-center justify-center w-8 h-8 rounded-full bg-primary/10 text-primary font-medium text-sm">
                      {customer.totalBookings}
                    </span>
                  </TableCell>
                  <TableCell>{customer.createdAt}</TableCell>
                  {isAdmin && (
                    <TableCell className="text-right">
                      <Button
                        variant="ghost"
                        size="sm"
                        className="text-destructive hover:text-destructive"
                        onClick={() => onDelete?.(customer)}
                      >
                        <Trash2 className="w-4 h-4" />
                      </Button>
                    </TableCell>
                  )}
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  );
};

export default CustomersTable;
