import Sidebar from "@/components/admin/layout/sidebar";
import AuthProvider from "@/contexts/auth-context";
import { Toaster } from "sonner";

const Admin = ({ children }: { children: React.ReactNode }) => {
  return (
    <AuthProvider>
      <div className="flex h-screen overflow-hidden bg-background">
        <Sidebar />
        <main className="flex-1 overflow-auto">{children}</main>
      </div>
      <Toaster position="top-right" />
    </AuthProvider>
  );
};

export default Admin;
