import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { DashboardSummary } from '../../models/dashboard.model';
import { ApiService } from '../../core/api';

@Component({
  selector: 'app-dashboard',
 standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent implements OnInit {
  s: DashboardSummary | null = null;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getSummary().subscribe({
      next: (res) => (this.s = res),
      error: () => (this.s = null),
    });
  }
}
