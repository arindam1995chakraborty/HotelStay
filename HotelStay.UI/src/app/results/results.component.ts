import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HotelApiService } from '../services/hotel-api.service';
import type { SearchResult } from '../models/search-result';

@Component({
  standalone: true,
  selector: 'app-results',
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.scss'],
  imports: [CommonModule, RouterModule]
})
export class ResultsComponent implements OnInit, OnDestroy {
  results: SearchResult[] = [];
  loading = false;
  error: any = null;
  sortAsc = true;
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(HotelApiService);

  private routeSub?: Subscription;
  private apiSub?: Subscription;
  private cd = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.routeSub = this.route.queryParamMap.subscribe(qm => {
      const destination = qm.get('destination') ?? '';
      const checkIn = qm.get('checkIn');
      const checkOut = qm.get('checkOut');
      const roomType = qm.get('roomType') ?? undefined;
      if (!destination || !checkIn || !checkOut) return;

      // cancel any in-flight API request
      if (this.apiSub) {
        this.apiSub.unsubscribe();
        this.apiSub = undefined;
      }

      this.loading = true;
      this.error = null;
      const ci = new Date(checkIn!);
      const co = new Date(checkOut!);

      this.apiSub = this.api.search(destination, ci, co, roomType as any).subscribe({
        next: (res) => {
          console.log('Search response', res);
          this.results = res ?? [];
          this.sortResults();
          // ensure loading cleared when results arrive
          this.loading = false;
          // force change detection in case it's needed
          try { this.cd.detectChanges(); } catch {}
        },
        error: (e) => {
          console.error('Search error', e);
          this.error = e;
          this.loading = false;
        },
        complete: () => {
          this.loading = false;
          console.log('Search complete', this.loading);
        }
      });
    });
  }

  ngOnDestroy(): void {
    this.routeSub?.unsubscribe();
    this.apiSub?.unsubscribe();
  }

  sortResults() {
    this.results.sort((a,b) => this.sortAsc ? a.totalPrice - b.totalPrice : b.totalPrice - a.totalPrice);
  }

  toggleSort() {
    this.sortAsc = !this.sortAsc;
    this.sortResults();
  }

  reserve(item: SearchResult) {
    const qp = {
      provider: item.provider,
      roomId: item.roomId,
      destination: this.route.snapshot.queryParamMap.get('destination') ?? '',
      checkIn: this.route.snapshot.queryParamMap.get('checkIn') ?? '',
      checkOut: this.route.snapshot.queryParamMap.get('checkOut') ?? ''
    };
    this.router.navigate(['/reserve'], { queryParams: qp });
  }
}
