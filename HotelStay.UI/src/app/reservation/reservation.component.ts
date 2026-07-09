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
  submitting = false;
  error: any = null;

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
  }

  async submit() {
    if (this.form.invalid) return;
    this.submitting = true;
    this.error = null;
    const v = this.form.value as any;
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
