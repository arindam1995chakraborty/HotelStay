export interface ReservationRequest {
  guestName: string;
  documentType: string;
  documentNumber: string;
  destination: string;
  checkIn: string; // yyyy-MM-dd
  checkOut: string; // yyyy-MM-dd
  provider: string;
  roomId: string;
}
