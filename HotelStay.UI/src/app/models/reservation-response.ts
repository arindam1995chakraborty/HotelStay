export interface ReservationResponse {
  reference: string;
  provider: string;
  guestName: string;
  totalPrice: number;
  cancellationPolicy: string;
  checkIn: string; // yyyy-MM-dd
  checkOut: string; // yyyy-MM-dd
  destination: string;
}
