import { currentUser, type User, type UserRole } from "@/lib/mock-data";
import { createContext, useContext, useState, type ReactNode } from "react";

interface AuthContextType {
  user: User;
  isAdmin: boolean;
  setUserRole: (role: UserRole) => void;
  logout: () => void;
}

// Tạo context với giá trị mặc định
const AuthContext = createContext<AuthContextType | undefined>(undefined);

const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User>(currentUser);

  // Kiểm tra xem có phải là admin không
  const isAdmin = user.role === "admin";

  const setUserRole = (role: UserRole) => {
    setUser((prev) => ({
      ...prev,
      role,
      name: role === "admin" ? "John Admin" : "Jane Staff",
    }));
  };

  const logout = () => {
    console.log("Logging out...");
  };

  return (
    <AuthContext.Provider value={{ user, isAdmin, setUserRole, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthProvider;

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth phải được sử dụng bên trong AuthProvider");
  }
  return context;
};
