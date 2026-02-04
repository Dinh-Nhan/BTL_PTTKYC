// Mock data for Hotel Booking Management System

export type UserRole = "admin" | "staff";

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  avatar?: string;
}

export interface Room {
  id: string;
  number: string;
  type: "single" | "double" | "suite" | "deluxe";
  status: "available" | "booked" | "maintenance";
  price: number;
  floor: number;
  amenities: string[];
}

export interface Booking {
  id: string;
  roomId: string;
  roomNumber: string;
  customerName: string;
  customerId: string;
  checkIn: string;
  checkOut: string;
  status: "pending" | "confirmed" | "cancelled";
  totalAmount: number;
  deposit: number;
  createdAt: string;
}

export interface Invoice {
  id: string;
  bookingId: string;
  customerName: string;
  roomNumber: string;
  amount: number;
  status: "paid" | "unpaid" | "refunded";
  createdAt: string;
  paidAt?: string;
}

export interface Staff {
  id: string;
  name: string;
  email: string;
  password: string;
  phone: string;
  birth: string;
  role: "0" | "1";
  gender: "0" | "1";
  status: "0" | "1";
  joinedAt: string;
}

export interface Customer {
  id: string;
  name: string;
  email: string;
  phone: string;
  idNumber: string;
  totalBookings: number;
  createdAt: string;
}

// Current user (simulated)
export const currentUser: User = {
  id: "1",
  name: "John Admin",
  email: "john@hotel.com",
  role: "admin",
};

// Mock rooms data
export const rooms: Room[] = [
  {
    id: "1",
    number: "101",
    type: "single",
    status: "available",
    price: 80,
    floor: 1,
    amenities: ["WiFi", "TV", "AC"],
  },
  {
    id: "2",
    number: "102",
    type: "single",
    status: "booked",
    price: 80,
    floor: 1,
    amenities: ["WiFi", "TV", "AC"],
  },
  {
    id: "3",
    number: "103",
    type: "double",
    status: "available",
    price: 120,
    floor: 1,
    amenities: ["WiFi", "TV", "AC", "Mini Bar"],
  },
  {
    id: "4",
    number: "201",
    type: "double",
    status: "booked",
    price: 120,
    floor: 2,
    amenities: ["WiFi", "TV", "AC", "Mini Bar"],
  },
  {
    id: "5",
    number: "202",
    type: "suite",
    status: "maintenance",
    price: 250,
    floor: 2,
    amenities: ["WiFi", "TV", "AC", "Mini Bar", "Jacuzzi"],
  },
  {
    id: "6",
    number: "203",
    type: "suite",
    status: "available",
    price: 250,
    floor: 2,
    amenities: ["WiFi", "TV", "AC", "Mini Bar", "Jacuzzi"],
  },
  {
    id: "7",
    number: "301",
    type: "deluxe",
    status: "booked",
    price: 350,
    floor: 3,
    amenities: ["WiFi", "TV", "AC", "Mini Bar", "Jacuzzi", "Balcony"],
  },
  {
    id: "8",
    number: "302",
    type: "deluxe",
    status: "available",
    price: 350,
    floor: 3,
    amenities: ["WiFi", "TV", "AC", "Mini Bar", "Jacuzzi", "Balcony"],
  },
  {
    id: "9",
    number: "303",
    type: "single",
    status: "available",
    price: 80,
    floor: 3,
    amenities: ["WiFi", "TV", "AC"],
  },
  {
    id: "10",
    number: "401",
    type: "double",
    status: "booked",
    price: 120,
    floor: 4,
    amenities: ["WiFi", "TV", "AC", "Mini Bar"],
  },
];

// Mock bookings data
export const bookings: Booking[] = [
  {
    id: "B001",
    roomId: "2",
    roomNumber: "102",
    customerName: "Alice Johnson",
    customerId: "C001",
    checkIn: "2026-01-18",
    checkOut: "2026-01-20",
    status: "confirmed",
    totalAmount: 160,
    deposit: 80,
    createdAt: "2026-01-15",
  },
  {
    id: "B002",
    roomId: "4",
    roomNumber: "201",
    customerName: "Bob Smith",
    customerId: "C002",
    checkIn: "2026-01-17",
    checkOut: "2026-01-22",
    status: "confirmed",
    totalAmount: 600,
    deposit: 300,
    createdAt: "2026-01-14",
  },
  {
    id: "B003",
    roomId: "7",
    roomNumber: "301",
    customerName: "Carol White",
    customerId: "C003",
    checkIn: "2026-01-18",
    checkOut: "2026-01-21",
    status: "pending",
    totalAmount: 1050,
    deposit: 500,
    createdAt: "2026-01-16",
  },
  {
    id: "B004",
    roomId: "10",
    roomNumber: "401",
    customerName: "David Brown",
    customerId: "C004",
    checkIn: "2026-01-19",
    checkOut: "2026-01-23",
    status: "confirmed",
    totalAmount: 480,
    deposit: 240,
    createdAt: "2026-01-17",
  },
  {
    id: "B005",
    roomId: "1",
    roomNumber: "101",
    customerName: "Eva Green",
    customerId: "C005",
    checkIn: "2026-01-25",
    checkOut: "2026-01-28",
    status: "pending",
    totalAmount: 240,
    deposit: 0,
    createdAt: "2026-01-18",
  },
  {
    id: "B006",
    roomId: "3",
    roomNumber: "103",
    customerName: "Frank Lee",
    customerId: "C006",
    checkIn: "2026-01-10",
    checkOut: "2026-01-12",
    status: "cancelled",
    totalAmount: 240,
    deposit: 120,
    createdAt: "2026-01-08",
  },
];

// Mock invoices data
export const invoices: Invoice[] = [
  {
    id: "INV001",
    bookingId: "B001",
    customerName: "Alice Johnson",
    roomNumber: "102",
    amount: 160,
    status: "paid",
    createdAt: "2026-01-15",
    paidAt: "2026-01-18",
  },
  {
    id: "INV002",
    bookingId: "B002",
    customerName: "Bob Smith",
    roomNumber: "201",
    amount: 600,
    status: "unpaid",
    createdAt: "2026-01-14",
  },
  {
    id: "INV003",
    bookingId: "B003",
    customerName: "Carol White",
    roomNumber: "301",
    amount: 1050,
    status: "unpaid",
    createdAt: "2026-01-16",
  },
  {
    id: "INV004",
    bookingId: "B004",
    customerName: "David Brown",
    roomNumber: "401",
    amount: 480,
    status: "paid",
    createdAt: "2026-01-17",
    paidAt: "2026-01-19",
  },
  {
    id: "INV005",
    bookingId: "B006",
    customerName: "Frank Lee",
    roomNumber: "103",
    amount: 120,
    status: "refunded",
    createdAt: "2026-01-08",
    paidAt: "2026-01-10",
  },
];

// Mock customers data
export const customers: Customer[] = [
  {
    id: "C001",
    name: "Alice Johnson",
    email: "alice@email.com",
    phone: "+1 555-1001",
    idNumber: "ID12345678",
    totalBookings: 3,
    createdAt: "2024-06-15",
  },
  {
    id: "C002",
    name: "Bob Smith",
    email: "bob@email.com",
    phone: "+1 555-1002",
    idNumber: "ID23456789",
    totalBookings: 5,
    createdAt: "2023-11-20",
  },
  {
    id: "C003",
    name: "Carol White",
    email: "carol@email.com",
    phone: "+1 555-1003",
    idNumber: "ID34567890",
    totalBookings: 1,
    createdAt: "2025-12-01",
  },
  {
    id: "C004",
    name: "David Brown",
    email: "david@email.com",
    phone: "+1 555-1004",
    idNumber: "ID45678901",
    totalBookings: 2,
    createdAt: "2025-08-10",
  },
  {
    id: "C005",
    name: "Eva Green",
    email: "eva@email.com",
    phone: "+1 555-1005",
    idNumber: "ID56789012",
    totalBookings: 1,
    createdAt: "2026-01-05",
  },
  {
    id: "C006",
    name: "Frank Lee",
    email: "frank@email.com",
    phone: "+1 555-1006",
    idNumber: "ID67890123",
    totalBookings: 4,
    createdAt: "2024-04-22",
  },
];

// Dashboard stats
export const dashboardStats = {
  totalRooms: rooms.length,
  availableRooms: rooms.filter((r) => r.status === "available").length,
  bookingsToday: bookings.filter(
    (b) => b.checkIn === "2026-01-18" && b.status !== "cancelled",
  ).length,
  monthlyRevenue: invoices
    .filter((i) => i.status === "paid")
    .reduce((acc, i) => acc + i.amount, 0),
};

// Weekly bookings data for chart
export const weeklyBookings = [
  { day: "Thứ Hai", bookings: 4 },
  { day: "Thứ Ba", bookings: 6 },
  { day: "Thứ Tư", bookings: 5 },
  { day: "Thứ Năm", bookings: 8 },
  { day: "Thứ Sáu", bookings: 12 },
  { day: "Thứ Bảy", bookings: 15 },
  { day: "Chủ nhật", bookings: 10 },
];
