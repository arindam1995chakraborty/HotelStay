import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { HotelApiService } from '../services/hotel-api.service';
import type { ReservationResponse } from '../models/reservation-response';

@Component({
  standalone: true,
  selector: 'app-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.scss'],
  imports: [CommonModule]
})
export class ConfirmationComponent implements OnInit {
  reference = '';
  loading = false;
  error: any = null;
  model?: ReservationResponse;
  private timeoutId: any;

  private route = inject(ActivatedRoute);
  private api = inject(HotelApiService);
  private cd = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.reference = this.route.snapshot.paramMap.get('reference') ?? '';
    if (!this.reference) return;
    this.loading = true;
    // safety timeout: clear loader after 10s if nothing happens
    this.timeoutId = setTimeout(() => {
      if (this.loading) {
        console.warn('getReservation timeout');
        this.error = { title: 'Timeout', detail: 'Reservation request timed out' };
        this.loading = false;
      }
    }, 10000);

    this.api.getReservation(this.reference).subscribe({
      next: (r) => {
        console.debug('Normalized reservation', r);
        this.model = r;
        this.loading = false;
        clearTimeout(this.timeoutId);
        // force change detection in case it's needed
        try { this.cd.detectChanges(); } catch {}
      },
      error: (e) => {
        console.error('getReservation error', e);
        this.error = e;
        this.loading = false;
        clearTimeout(this.timeoutId);
        try { this.cd.detectChanges(); } catch {}
      }
    });
  }

  ngOnDestroy(): void {
    if (this.timeoutId) clearTimeout(this.timeoutId);
  }
}
