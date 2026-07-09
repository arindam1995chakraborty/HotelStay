import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class SearchComponent {
  form = inject(FormBuilder).group({
    destination: ['', Validators.required],
    checkIn: ['', Validators.required],
    checkOut: ['', Validators.required],
    roomType: ['']
  });

  protected readonly title = signal('Search');
  private router = inject(Router);

  submit() {
    if (this.form.invalid) return;
    const v = this.form.value as any;
    const checkIn = new Date(v.checkIn);
    const checkOut = new Date(v.checkOut);
    if (checkOut <= checkIn) return alert('checkOut must be after checkIn');

    const queryParams: any = {
      destination: v.destination,
      checkIn: checkIn.toISOString().slice(0,10),
      checkOut: checkOut.toISOString().slice(0,10)
    };
    if (v.roomType) queryParams.roomType = v.roomType;

    this.router.navigate(['/results'], { queryParams });
  }
}
