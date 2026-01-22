import { Button } from "@/components/ui/button";
import { useAuth } from "@/contexts/auth-context";
import { cn } from "@/lib/utils";
import { Link, useLocation } from "react-router-dom";
import {
  BedDouble,
  CalendarDays,
  ChevronLeft,
  ChevronRight,
  Hotel,
  LayoutDashboard,
  Receipt,
  UserCog,
  Users,
} from "lucide-react";
import { useState } from "react";

const menuItems = [
  { href: "/", label: "Tổng quan", icon: LayoutDashboard, adminOnly: false },
  {
    href: "/rooms",
    label: "Quản lý phòng",
    icon: BedDouble,
    adminOnly: false,
  },
  {
    href: "/bookings",
    label: "Quản lý đặt phòng",
    icon: CalendarDays,
    adminOnly: false,
  },
  {
    href: "/invoices",
    label: "Quản lý hóa đơn",
    icon: Receipt,
    adminOnly: false,
  },
  {
    href: "/staff",
    label: "Quản lý nhân viên",
    icon: UserCog,
    adminOnly: true,
  },
  {
    href: "/customers",
    label: "Quản lý khách hàng",
    icon: Users,
    adminOnly: false,
  },
];

const Sidebar = () => {
  const [collapsed, setCollapsed] = useState(false);
  const { isAdmin } = useAuth();
  const { pathname } = useLocation();

  const filteredItems = menuItems.filter((item) => !item.adminOnly || isAdmin);

  return (
    <aside
      className={cn(
        "flex flex-col bg-card border-r border-border transition-all duration-300 h-screen",
        collapsed ? "w-16" : "w-64",
      )}
    >
      {/* Logo */}
      <div className="flex items-center gap-3 p-4 border-b border-border">
        <div className="flex items-center justify-center w-10 h-10 rounded-lg bg-primary text-primary-foreground">
          <Hotel className="w-5 h-5" />
        </div>
        {!collapsed && (
          <div className="flex flex-col">
            <span className="font-semibold text-foreground">
              Khách sạn ngàn sao
            </span>
            <span className="text-xs text-muted-foreground">Quản lý</span>
          </div>
        )}
      </div>

      {/* Navigation */}
      <nav className="flex-1 p-3 space-y-1">
        {filteredItems.map((item) => {
          const isActive = pathname === item.href;

          return (
            <Link
              key={item.href}
              to={item.href}
              className={cn(
                "flex items-center gap-3 px-3 py-2.5 rounded-lg transition-colors",
                isActive
                  ? "bg-primary text-primary-foreground"
                  : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
              )}
            >
              <item.icon className="w-5 h-5 shrink-0" />
              {!collapsed && (
                <span className="text-sm font-medium">{item.label}</span>
              )}
            </Link>
          );
        })}
      </nav>

      {/* Collapse Toggle */}
      <div className="p-3 border-t border-border">
        <Button
          variant="ghost"
          size="sm"
          className="w-full justify-center"
          onClick={() => setCollapsed(!collapsed)}
        >
          {collapsed ? (
            <ChevronRight className="w-4 h-4" />
          ) : (
            <>
              <ChevronLeft className="w-4 h-4 mr-2" />
              <span>Collapse</span>
            </>
          )}
        </Button>
      </div>
    </aside>
  );
};

export default Sidebar;
