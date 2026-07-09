import type { RoomType } from './enums';

export interface SearchResult {
  provider: string;
  roomId: string;
  roomType: RoomType;
  perNightRate: number;
  totalPrice: number;
  cancellationPolicy: string;
  rating: number;
}
