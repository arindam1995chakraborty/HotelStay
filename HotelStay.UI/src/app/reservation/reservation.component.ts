import { Component, OnInit, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HotelApiService } from '../services/hotel-api.service';
import type { ReservationRequest } from '../models/reservation-request';

@Component({
  standalone: true,
  selector: 'app-reservation',
  templateUrl: './reservation.component.html',
  styleUrls: ['./reservation.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, RouterModule]
})
export class ReservationComponent implements OnInit {
  form = inject(FormBuilder).group({
    guestName: ['', Validators.required],
    documentType: ['Passport', Validators.required],
    documentNumber: ['', Validators.required]
  });

  provider = '';
  roomId = '';
  destination = '';
  checkIn = '';
  checkOut = '';
  perNightRate: number | null = null;
  cancellationPolicy = '';
  submitting = false;
  error: any = null;

  // Client-side known city lists (mirror server SeedData for fast validation)
  private readonly INTERNATIONAL_CITIES = ['Paris','Tokyo','Sydney','London','Berlin'];
  private readonly DOMESTIC_CITIES = ['Seattle','Portland','Kolkata','New York','San Francisco'];

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(HotelApiService);

  ngOnInit(): void {
    const qp = this.route.snapshot.queryParamMap;
    this.provider = qp.get('provider') ?? '';
    this.roomId = qp.get('roomId') ?? '';
    this.destination = qp.get('destination') ?? '';
    this.checkIn = qp.get('checkIn') ?? '';
    this.checkOut = qp.get('checkOut') ?? '';
    const pn = qp.get('perNightRate');
    if (pn) this.perNightRate = Number(pn);
    this.cancellationPolicy = qp.get('cancellationPolicy') ?? '';
  }

  // Check client-side document validity according to destination
  isDocumentValid(): boolean {
    const v = this.form.value as any;
    const dest = this.destination ?? '';
    if (!v || !v.documentType) return false;
    if (this.INTERNATIONAL_CITIES.includes(dest)) {
      return v.documentType === 'Passport';
    }
    return ["Passport","NationalID"].includes(v.documentType);
  }

  async submit() {
    if (this.form.invalid) return;
    // client-side document enforcement
    const v = this.form.value as any;
    if (!this.isDocumentValid()) {
      this.error = { title: 'Invalid document', detail: this.INTERNATIONAL_CITIES.includes(this.destination) ? 'International destinations require a Passport' : 'Domestic destinations require NationalID (or Passport)' };
      return;
    }
    this.submitting = true;
    this.error = null;
    const req: ReservationRequest = {
      guestName: v.guestName,
      documentType: v.documentType,
      documentNumber: v.documentNumber,
      destination: this.destination,
      checkIn: this.checkIn,
      checkOut: this.checkOut,
      provider: this.provider,
      roomId: this.roomId
    };

    try {
      const res = await firstValueFrom(this.api.reserve(req));
      console.debug('Reserve normalized response', res);
      if (res && res.reference) {
        this.router.navigate(['/confirmation', res.reference]);
      } else {
        this.error = { title: 'Reservation failed', detail: 'Empty response from server' };
      }
    } catch (e) {
      console.error('Reserve error', e);
      this.error = e;
    } finally {
      this.submitting = false;
    }
  }
}
