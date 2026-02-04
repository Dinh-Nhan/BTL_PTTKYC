// src/types/booking.ts

export default interface Booking {
  bookingId: number;
  checkInDatetime: string;
  checkOutDatetime: string;
  depositAmount: number;
  status: string;

  roomResponse: {
    roomNumber: string;
  };

  clientResponse?: {
    fullName: string;
  };
}
