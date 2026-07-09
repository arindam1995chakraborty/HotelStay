import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import type { SearchResult } from '../models/search-result';
import type { ReservationRequest } from '../models/reservation-request';
import type { ReservationResponse } from '../models/reservation-response';
import type { RoomType } from '../models/enums';

@Injectable({ providedIn: 'root' })
export class HotelApiService {
  private http = inject(HttpClient);
  private base = environment.apiBaseUrl.replace(/\/$/, '');

  search(destination: string, checkIn: Date, checkOut: Date, roomType?: RoomType): Observable<SearchResult[]> {
    const params = new HttpParams({ fromObject: {
      destination,
      checkIn: this.toIsoDate(checkIn),
      checkOut: this.toIsoDate(checkOut),
      ...(roomType ? { roomType } : {})
    }});

    return this.http.get<any[]>(`${this.base}/hotels/search`, { params }).pipe(
      map(items => (items || []).map(it => this.normalizeSearchResult(it))),
      catchError(this.handleError)
    );
  }

  reserve(request: ReservationRequest): Observable<ReservationResponse> {
    return this.http.post<any>(`${this.base}/hotels/reserve`, request).pipe(
      map(r => {
        console.debug('Raw reserve response', r);
        const payload = this.extractPayload(r);
        return this.normalizeReservationResponse(payload);
      }),
      catchError(this.handleError)
    );
  }

  getReservation(reference: string): Observable<ReservationResponse> {
    const url = `${this.base}/hotels/reservation/${encodeURIComponent(reference)}`;
    console.debug('Requesting reservation URL', url);
    return this.http.get<any>(url, { observe: 'response' }).pipe(
      map((resp: HttpResponse<any>) => {
        console.debug('HTTP getReservation response status', resp.status, 'headers', resp.headers.keys());
        const body = resp.body;
        console.debug('Raw getReservation response body', body);
        const payload = this.extractPayload(body);
        try {
          return this.normalizeReservationResponse(payload);
        } catch (ex) {
          console.error('Error normalizing reservation response', ex, payload);
          throw ex;
        }
      }),
      catchError(this.handleError)
    );
  }

  private toIsoDate(d: Date) {
    return d.toISOString().slice(0, 10);
  }

  private handleError(err: HttpErrorResponse) {
    // If backend returns ProblemDetails, expose it via error.error
    const payload = err.error ?? { title: err.statusText || 'Error', detail: err.message };
    return throwError(() => payload);
  }

  private normalizeSearchResult(it: any): SearchResult {
    return {
      provider: it.provider ?? it.Provider,
      roomId: it.roomId ?? it.RoomId,
      roomType: it.roomType ?? it.RoomType,
      perNightRate: (it.perNightRate ?? it.PerNightRate) as number,
      totalPrice: (it.totalPrice ?? it.TotalPrice) as number,
      cancellationPolicy: it.cancellationPolicy ?? it.CancellationPolicy,
      rating: (it.rating ?? it.Rating) as number
    };
  }

  private normalizeReservationResponse(r: any): ReservationResponse {
    if (!r) return r;
    return {
      reference: r.reference ?? r.Reference,
      provider: r.provider ?? r.Provider,
      guestName: r.guestName ?? r.GuestName,
      totalPrice: (r.totalPrice ?? r.TotalPrice) as number,
      cancellationPolicy: r.cancellationPolicy ?? r.CancellationPolicy,
      checkIn: r.checkIn ?? r.CheckIn,
      checkOut: r.checkOut ?? r.CheckOut,
      destination: r.destination ?? r.Destination
    };
  }

  private extractPayload(r: any) {
    if (!r) return r;
    // Some servers may wrap the response (e.g., { response: { ... } })
    if (r.response) return r.response;
    if (r.value) return r.value;
    return r;
  }
}
